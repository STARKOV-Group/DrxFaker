using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Bogus;
using Sungero.Domain.Shared;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace starkov.Faker.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// АО для запуска множества АО по генерации сущностей.
    /// </summary>
    /// <param name="args"></param>
    public virtual void CreateAndExecuteAsyncHandlers(starkov.Faker.Server.AsyncHandlerInvokeArgs.CreateAndExecuteAsyncHandlersInvokeArgs args)
    {
      Logger.Debug("Start async handler CreateAndExecuteAsyncHandlers");
      args.Retry = false;
      
      Functions.Module.CreateAsyncForGenerateEntities(args.Count, args.DatabookId, args.UserId);
      
      Logger.Debug("End async handler CreateAndExecuteAsyncHandlers");
    }

    /// <summary>
    /// Асинхронный обработчик для запуска задач.
    /// </summary>
    public virtual void TasksStart(starkov.Faker.Server.AsyncHandlerInvokeArgs.TasksStartInvokeArgs args)
    {
      Logger.DebugFormat("Start async handler TasksStart");
      args.Retry = false;
      
      var errors = new System.Text.StringBuilder();
      var stopWatch = new Stopwatch();
      stopWatch.Start();
      
      var ids = args.TaskIds.Split(Constants.Module.Separator).Select(x => long.Parse(x)).ToList();
      var tasks = Sungero.Workflow.Tasks.GetAll(t => ids.Contains(t.Id));
      foreach (var task in tasks)
      {
        try
        {
          task.Start();
        }
        catch (Exception ex)
        {
          errors.AppendLine(starkov.Faker.Resources.Error_StartTaskFormat(task.Id, ex.Message));
          Logger.ErrorFormat("TasksStart error: {0}\r\n   StackTrace: {1}", ex.Message, ex.StackTrace);
        }
      }
      
      stopWatch.Stop();
      var ts = stopWatch.Elapsed;
      var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                      ts.Hours, ts.Minutes, ts.Seconds,
                                      ts.Milliseconds / 10);
      
      if (Functions.ModuleSetup.GetModuleSetup()?.IsDisableNotifications.GetValueOrDefault() == false && errors.Length != 0)
      {
        var administrators = Roles.Administrators.RecipientLinks.Select(l => l.Member);
        var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(starkov.Faker.Resources.NoticeSubject_InfoAboutStartTasks, administrators.ToArray());
        notice.ActiveText = errors.ToString();
        notice.Start();
      }
      
      Logger.DebugFormat("End async handler TasksStart, elapsed time {0}", elapsedTime);
    }

    /// <summary>
    /// Асинхронный обработчик для генерации сущностей.
    /// </summary>
    public virtual void EntitiesGeneration(starkov.Faker.Server.AsyncHandlerInvokeArgs.EntitiesGenerationInvokeArgs args)
    {
      Logger.DebugFormat("Start async handler EntitiesGeneration");
      args.Retry = false;
      
      var databook = ParametersMatchings.GetAll(p => p.Id == args.DatabookId).FirstOrDefault();
      if (databook == null)
      {
        Logger.DebugFormat("EntitiesGeneration error: No ParametersMatching databooks with id {0}", args.DatabookId);
        return;
      }
      var propertiesStructure = Functions.Module.GetPropertiesType(databook.EntityType?.EntityTypeGuid ?? databook.DocumentType?.DocumentTypeGuid);
      
      var stopWatch = new Stopwatch();
      stopWatch.Start();
      
      var isDisableNotify = Functions.ModuleSetup.GetModuleSetup()?.IsDisableNotifications.GetValueOrDefault() ?? false;
      var attachments = new List<IEntity>();
      var loginNames = new List<string>();
      var errors = new List<string>();
      var createdEntityCount = 0;
      long firstEntityId = 0;
      var taskIds = new System.Text.StringBuilder();
      var maxLoginNamesNumber = Functions.ModuleSetup.GetLoginNamesNumber();
      var maxAttachmentsNumber = Functions.ModuleSetup.GetAttachmentsNumber();
      
      Sungero.Content.IElectronicDocumentVersions documentVersion = null;
      var isNeedCreateVersion = databook.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Document && databook.IsNeedCreateVersion.GetValueOrDefault();
      if (isNeedCreateVersion)
        documentVersion = Functions.ModuleSetup.GetDocumentWithVersion()?.LastVersion;
      
      var cache = new Dictionary<string, IEntity>();
      for (var i = 0; i < args.Count; i++)
      {
        try
        {
          using (Sungero.Domain.Session session = new Sungero.Domain.Session(true, false))
          {
            #region Создание учетных записей
            if (databook.EntityType?.EntityTypeGuid == Constants.Module.Guids.Login.ToString())
            {
              CreateLogin(databook, propertiesStructure, maxLoginNamesNumber, ref loginNames);
              createdEntityCount++;
              
              continue;
            }
            #endregion
            
            var finalTypeGuid = databook.EntityType?.EntityTypeGuid ?? databook.DocumentType?.DocumentTypeGuid;
            var entity = Functions.Module.CreateEntityByTypeGuid(finalTypeGuid);
            var entityProperties = entity.GetType().GetProperties();
            
            #region Заполнение свойств сущности
            foreach (var parametersRow in databook.Parameters.Where(p => p.FillOption != Constants.Module.FillOptions.Common.NullValue))
            {
              try
              {
                var property = entityProperties.FirstOrDefault(info => info.Name == parametersRow.PropertyName);
                if (property == null)
                  continue;
                
                var parameterStruct = Structures.Module.ParameterInfo.Create(parametersRow.ParametersMatching,
                                                                             parametersRow.PropertyName,
                                                                             parametersRow.PropertyTypeGuid,
                                                                             parametersRow.PropertyType,
                                                                             parametersRow.FillOption,
                                                                             parametersRow.ChosenValue,
                                                                             parametersRow.ValueFrom,
                                                                             parametersRow.ValueTo);
                var propertyValue = GetPropertyValue(parameterStruct,
                                                     propertiesStructure,
                                                     cache,
                                                     parametersRow.StringPropLength,
                                                     property);
                property.SetValue(entity, propertyValue);
              }
              catch (Exception ex)
              {
                var err = starkov.Faker.Resources.ErrorText_SetValToPropertyFormat(parametersRow.PropertyName, ex.Message);
                if (!errors.Contains(err))
                  errors.Add(err);
                
                Logger.ErrorFormat("EntitiesGeneration error caused by setting value in property {0}: {1}\r\n   StackTrace: {2}",
                                   parametersRow.PropertyName,
                                   ex.Message,
                                   ex.StackTrace);
              }
            }
            #endregion
            
            #region Заполнение коллекций сущности
            var collectionStructure = Functions.Module.GetCollectionPropertiesType(databook.EntityType?.EntityTypeGuid ?? databook.DocumentType?.DocumentTypeGuid);
            foreach (var collectionName in databook.CollectionParameters.Select(r => r.CollectionName).Distinct())
            {
              var rowCount = databook.CollectionParameters.FirstOrDefault(r => r.CollectionName == collectionName).RowCount;
              var parameters = databook.CollectionParameters
                .Where(r => r.CollectionName == collectionName)
                .Where(r => r.FillOption != Constants.Module.FillOptions.Common.NullValue);
              var collectionInfo = entityProperties.FirstOrDefault(info => info.Name == collectionName);
              if (collectionInfo == null)
                continue;
              
              var collection = collectionInfo.GetValue(entity, null);
              if (collection == null)
                continue;
              
              for (var number = 0; number < rowCount; number++)
              {
                var newLine = collection.GetType().GetMethod(Constants.Module.Collections.AddMethod, new Type[0]).Invoke(collection, null);
                foreach (var parametersRow in parameters)
                {
                  try
                  {
                    var property = newLine.GetType().GetProperties().FirstOrDefault(p => p.Name == parametersRow.PropertyName);
                    if (property == null)
                      continue;
                    
                    var parameterStruct = Structures.Module.ParameterInfo.Create(parametersRow.ParametersMatching,
                                                                                 parametersRow.PropertyName,
                                                                                 parametersRow.PropertyTypeGuid,
                                                                                 parametersRow.PropertyType,
                                                                                 parametersRow.FillOption,
                                                                                 parametersRow.ChosenValue,
                                                                                 parametersRow.ValueFrom,
                                                                                 parametersRow.ValueTo);
                    var propertyValue = GetPropertyValue(parameterStruct,
                                                         collectionStructure.FirstOrDefault(s => s.Name == collectionName).Properties,
                                                         cache,
                                                         parametersRow.StringPropLength,
                                                         property);
                    property.SetValue(newLine, propertyValue);
                  }
                  catch (Exception ex)
                  {
                    var err = starkov.Faker.Resources.ErrorText_SetValToPropertyFormat(parametersRow.PropertyName, ex.Message);
                    if (!errors.Contains(err))
                      errors.Add(err);
                    
                    Logger.ErrorFormat("EntitiesGeneration error caused by setting value in property {0}: {1}\r\n   StackTrace: {2}",
                                       parametersRow.PropertyName,
                                       ex.Message,
                                       ex.StackTrace);
                  }
                }
                
                var lineEntity = Functions.Module.CastToEntity(newLine);
                foreach (var linePropertyState in lineEntity.State.Properties.Where(p => p.IsRequired == true))
                  linePropertyState.IsRequired = false;
              }
            }
            #endregion
            
            #region Заполнение вложений задачи
            if (databook.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Task)
            {
              foreach (var attachmentRow in databook.AttachmentParameters.Where(p => p.FillOption != Constants.Module.FillOptions.Common.NullValue))
              {
                var attachmentGroupInfo = entityProperties.FirstOrDefault(info => info.Name == attachmentRow.AttachmentName);
                if (attachmentGroupInfo == null)
                  continue;
                
                var attachmentGroup = attachmentGroupInfo.GetValue(entity, null);
                if (attachmentGroup == null)
                  continue;
                
                var attachmentInfo = attachmentGroup.GetType().GetProperties().FirstOrDefault(_ => _.Name == Constants.Module.Attachments.PropertyAll);
                if (attachmentInfo == null)
                  break;
                
                var attachment = attachmentInfo.GetValue(attachmentGroup, null);
                if (attachment == null)
                  continue;
                
                var parameterStruct = Structures.Module.ParameterInfo.Create(attachmentRow.ParametersMatching,
                                                                             attachmentRow.AttachmentName,
                                                                             attachmentRow.PropertyTypeGuid,
                                                                             attachmentRow.PropertyType,
                                                                             attachmentRow.FillOption,
                                                                             attachmentRow.ChosenValue,
                                                                             string.Empty,
                                                                             string.Empty);
                
                for (var number = 0; number < attachmentRow.Count.GetValueOrDefault(); number++)
                {
                  try
                  {
                    var propertyValue = GetPropertyValue(parameterStruct,
                                                         propertiesStructure,
                                                         cache,
                                                         null,
                                                         attachmentInfo);
                    attachment.GetType().GetMethod(Constants.Module.Attachments.AddMethod).Invoke(attachment, new object[1] { propertyValue });
                  }
                  catch (Exception ex)
                  {
                    var err = starkov.Faker.Resources.ErrorText_SetAttachmentIntoGroupFormat(attachmentRow.AttachmentName, ex.Message);
                    if (!errors.Contains(err))
                      errors.Add(err);
                    
                    Logger.ErrorFormat("EntitiesGeneration error caused by setting attachments {0}: {1}\r\n   StackTrace: {2}",
                                       attachmentRow.AttachmentName,
                                       ex.Message,
                                       ex.StackTrace);
                  }
                }
              }
            }
            #endregion
            
            #region Создание версии документа
            if (isNeedCreateVersion)
            {
              try
              {
                CreateDocumentVersion(entity, documentVersion);
              }
              catch (Exception ex)
              {
                if (!errors.Contains(ex.Message))
                  errors.Add(ex.Message);
                
                Logger.ErrorFormat("EntitiesGeneration error caused by creating version: {0}\r\n   StackTrace: {1}", ex.Message, ex.StackTrace);
              }
            }
            #endregion
            
            #region Снятие признака обязательности свойств
            foreach (var propertyState in entity.State.Properties.Where(p => p.IsRequired == true))
              propertyState.IsRequired = false;
            #endregion
            
            try
            {
              entity.Save();
              if (!isDisableNotify && attachments.Count < maxAttachmentsNumber)
                attachments.Add(entity);
              
              createdEntityCount++;
              if (firstEntityId == 0)
                firstEntityId = entity.Id;
              
              if (databook.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Task && databook.IsNeedStartTask.GetValueOrDefault())
                taskIds.AppendFormat("{0}{1}", entity.Id, Constants.Module.Separator);
            }
            catch (Exception ex)
            {
              if (!errors.Contains(ex.Message))
                errors.Add(ex.Message);
              
              Logger.ErrorFormat("EntitiesGeneration error caused by saving entity: {0}\r\n   StackTrace: {1}", ex.Message, ex.StackTrace);
            }
          }
        }
        catch (Exception ex)
        {
          if (!errors.Contains(ex.Message))
            errors.Add(ex.Message);
          
          Logger.ErrorFormat("EntitiesGeneration error: {0}\r\n   StackTrace: {1}", ex.Message, ex.StackTrace);
        }
      }
      
      stopWatch.Stop();
      var ts = stopWatch.Elapsed;
      var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                      ts.Hours, ts.Minutes, ts.Seconds,
                                      ts.Milliseconds / 10);
      
      #region Старт задач
      if (databook.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Task &&
          databook.IsNeedStartTask.GetValueOrDefault() &&
          taskIds.Length != 0)
      {
        var async = AsyncHandlers.TasksStart.Create();
        async.TaskIds = taskIds.Remove(taskIds.Length - 1, 1).ToString();
        async.ExecuteAsync();
      }
      #endregion
      
      #region Отправка уведомления администраторам
      if (!isDisableNotify)
        SendNoticeToAdministrators(databook,
                                   createdEntityCount,
                                   args.Count,
                                   firstEntityId,
                                   elapsedTime,
                                   loginNames,
                                   errors,
                                   attachments);
      #endregion
      
      Logger.DebugFormat("End async handler EntitiesGeneration by databook with id {0}. Count of created entity {1}, elapsed time {2}", databook.Id, createdEntityCount, elapsedTime);
    }
    
    /// <summary>
    /// Получить значение для заполнения свойства.
    /// </summary>
    /// <param name="parameterRow">Структура с параметрами.</param>
    /// <param name="propertiesStructure">Список с информацией о свойствах.</param>
    /// <param name="cache">Кэш.</param>
    /// <param name="stringPropLength">Максимальная длина строки.</param>
    /// <param name="property">Информация о свойстве.</param>
    /// <returns>Сгенерированное значение.</returns>
    public virtual object GetPropertyValue(Structures.Module.ParameterInfo parameterStructure,
                                           List<starkov.Faker.Structures.Module.PropertyInfo> propertiesStructure,
                                           System.Collections.Generic.Dictionary<string, IEntity> cache,
                                           int? stringPropLength,
                                           System.Reflection.PropertyInfo property)
    {
      var propertyValue = Functions.Module.GetPropertyValueByParameters(parameterStructure, propertiesStructure, cache);
      if (Functions.Module.CompareObjectWithType(propertyValue, typeof(string)) && stringPropLength.HasValue)
      {
        var str = propertyValue.ToString();
        if (str.Length > stringPropLength.Value)
          propertyValue = str.Substring(0, stringPropLength.Value);
      }
      else if (Equals(property.PropertyType, typeof(double)) || Equals(property.PropertyType, typeof(double?)))
        propertyValue = Convert.ToDouble(propertyValue);
      else if (Equals(property.PropertyType, typeof(long)) || Equals(property.PropertyType, typeof(long?)))
        propertyValue = Convert.ToInt64(propertyValue);
      
      return propertyValue;
    }
    
    /// <summary>
    /// Создать учетную запись.
    /// </summary>
    /// <param name="databook">Справочник соответствие заполняемых параметров сущности.</param>
    /// <param name="propertiesStructure">Список с информацией о реквизитах учетной записи.</param>
    /// <param name="maxLoginNamesNumber">Максимальное кол-во выводимых наименований учетных записей.</param>
    /// <param name="loginNames">Список наименований учетных записей.</param>
    public virtual void CreateLogin(Faker.IParametersMatching databook, List<Structures.Module.PropertyInfo> propertiesStructure, int maxLoginNamesNumber, ref List<string> loginNames)
    {
      if (databook == null)
        return;
      
      var parametersRow = databook.Parameters.FirstOrDefault(p => p.PropertyName == Constants.Module.PropertyNames.LoginName);
      var parameterStruct = Structures.Module.ParameterInfo.Create(parametersRow.ParametersMatching,
                                                                   parametersRow.PropertyName,
                                                                   parametersRow.PropertyTypeGuid,
                                                                   parametersRow.PropertyType,
                                                                   parametersRow.FillOption,
                                                                   parametersRow.ChosenValue,
                                                                   parametersRow.ValueFrom,
                                                                   parametersRow.ValueTo);
      var login = Functions.Module.GetPropertyValueByParameters(parameterStruct, propertiesStructure).ToString();
      var password = databook.Parameters.FirstOrDefault(p => p.PropertyName == Constants.Module.PropertyNames.Password).ChosenValue;
      Sungero.Company.PublicFunctions.Module.CreateLogin(login, password);
      
      if (loginNames.Count < maxLoginNamesNumber)
        loginNames.Add(login);
    }
    
    /// <summary>
    /// Создать версию документа.
    /// </summary>
    /// <param name="entity">Документ.</param>
    /// <param name="documentVersion">Версия документа.</param>
    public virtual void CreateDocumentVersion(IEntity entity, Sungero.Content.IElectronicDocumentVersions documentVersion)
    {
      var document = Sungero.Docflow.OfficialDocuments.As(entity);
      if (document == null || documentVersion == null)
        return;
      
      var version = documentVersion.PublicBody?.Size != 0 ?
        documentVersion.PublicBody :
        documentVersion.Body;
      using (var stream = version.Read())
      {
        stream.Seek(0, SeekOrigin.Begin);
        document.CreateVersionFrom(stream, documentVersion.AssociatedApplication.Extension);
      }
    }
    
    /// <summary>
    /// Отправка уведомления администраторам.
    /// </summary>
    /// <param name="databook">Справочник соответствие заполняемых параметров сущности.</param>
    /// <param name="createdEntityCount">Кол-во созданных сущностей.</param>
    /// <param name="desiredEntityCount">Ожидаемое кол-во сущностей.</param>
    /// <param name="firstEntityId">ИД первой сгенерированной сущности.</param>
    /// <param name="elapsedTime">Затраченное на генерацию время.</param>
    /// <param name="loginNames">Список наименований учетных записей.</param>
    /// <param name="errors">Список ошибок при генерации.</param>
    /// <param name="attachments">Список сгенерированных сущностей.</param>
    public virtual void SendNoticeToAdministrators(Faker.IParametersMatching databook,
                                                   int createdEntityCount,
                                                   int desiredEntityCount,
                                                   long firstEntityId,
                                                   string elapsedTime,
                                                   List<string> loginNames,
                                                   List<string> errors,
                                                   List<IEntity> attachments)
    {
      var administrators = Roles.Administrators.RecipientLinks.Select(l => l.Member);
      var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(starkov.Faker.Resources.NoticeSubjectFormat(createdEntityCount, desiredEntityCount, databook.Name),
                                                                administrators.ToArray());
      
      foreach (var attachment in attachments)
        task.Attachments.Add(attachment);
      
      task.ActiveText = CreateNoticeTextToAdministrators(firstEntityId, elapsedTime, loginNames, errors);
      task.Start();
    }
    
    /// <summary>
    /// Создание текста уведомления для администраторов.
    /// </summary>
    /// <param name="firstEntityId">ИД первой сгенерированной сущности.</param>
    /// <param name="elapsedTime">Затраченное на генерацию время.</param>
    /// <param name="loginNames">Список наименований учетных записей.</param>
    /// <param name="errors">Список ошибок при генерации.</param>
    /// <returns>Текст уведомления для администраторов.</returns>
    public virtual string CreateNoticeTextToAdministrators(long firstEntityId,
                                                           string elapsedTime,
                                                           List<string> loginNames,
                                                           List<string> errors)
    {
      var sb = new System.Text.StringBuilder(starkov.Faker.Resources.InfoText_TimeSpentToCreatEntitiesFormat(elapsedTime));
      if (firstEntityId != 0)
        sb.Append(starkov.Faker.Resources.InfoText_FirstEntryIDFormat(firstEntityId));
      if (loginNames.Any())
        sb.Append(starkov.Faker.Resources.InfoText_LoginNamesFormat(string.Join(", ", loginNames)));
      if (errors.Any())
        sb.Append(starkov.Faker.Resources.InfoText_ErrorsFormat(string.Join("\n", errors)));
      
      return sb.ToString();
    }
  }
}