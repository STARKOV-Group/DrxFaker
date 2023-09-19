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
      var propertiesStructure = Functions.Module.GetPropertiesType(databook.DatabookType?.DatabookTypeGuid ?? databook.DocumentType?.DocumentTypeGuid);
      
      var stopWatch = new Stopwatch();
      stopWatch.Start();
      
      var attachments = new List<IEntity>();
      var loginNames = new List<string>();
      var errors = new List<string>();
      var createdEntityCount = 0;
      long firstEntityId = 0;
      var maxLoginNamesNumber = Functions.ModuleSetup.GetLoginNamesNumber();
      var maxAttachmentsNumber = Functions.ModuleSetup.GetAttachmentsNumber();
      
      Sungero.Content.IElectronicDocumentVersions documentVersion = null;
      var isNeedCreateVersion = databook.EntityType == Faker.ParametersMatching.EntityType.Document && databook.IsNeedCreateVersion.GetValueOrDefault();
      if (isNeedCreateVersion)
        documentVersion = Functions.ModuleSetup.GetDocumentWithVersion()?.LastVersion;
      
      var cache = new Dictionary<string, IEntity>();
      for (var i = 0; i < args.Count; i++)
      {
        try
        {
          #region Создание учетных записей
          if (databook.DatabookType?.DatabookTypeGuid == Constants.Module.Guids.Login.ToString())
          {
            CreateLogin(databook, propertiesStructure, maxLoginNamesNumber, ref loginNames);
            createdEntityCount++;
            
            continue;
          }
          #endregion
          
          var finalTypeGuid = databook.DatabookType?.DatabookTypeGuid ?? databook.DocumentType?.DocumentTypeGuid;
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
              
              var propertyValue = Functions.Module.GetPropertyValueByParameters(parametersRow, propertiesStructure, cache);
              if (Functions.Module.CompareObjectWithType(propertyValue, typeof(string)) && parametersRow.StringPropLength.HasValue)
              {
                var str = propertyValue.ToString();
                if (str.Length > parametersRow.StringPropLength.Value)
                  propertyValue = str.Substring(0, parametersRow.StringPropLength.Value);
              }
              else if (Equals(property.PropertyType, typeof(double)) || Equals(property.PropertyType, typeof(double?)))
                propertyValue = Convert.ToDouble(propertyValue);
              else if (Equals(property.PropertyType, typeof(long)) || Equals(property.PropertyType, typeof(long?)))
                propertyValue = Convert.ToInt64(propertyValue);
              
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
            if (attachments.Count < maxAttachmentsNumber)
              attachments.Add(entity);
            
            createdEntityCount++;
            if (firstEntityId == 0)
              firstEntityId = entity.Id;
          }
          catch (Exception ex)
          {
            if (!errors.Contains(ex.Message))
              errors.Add(ex.Message);
            
            Logger.ErrorFormat("EntitiesGeneration error caused by saving entity: {0}\r\n   StackTrace: {1}", ex.Message, ex.StackTrace);
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
      
      #region Отправка уведомления администраторам
      SendNoticeToAdministrators(databook,
                                 createdEntityCount,
                                 args.Count,
                                 firstEntityId,
                                 elapsedTime,
                                 loginNames,
                                 errors,
                                 attachments);
      #endregion
      
      Logger.DebugFormat("End async handler EntitiesGeneration");
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
      
      var login = Functions.Module.GetPropertyValueByParameters(databook.Parameters.FirstOrDefault(p => p.PropertyName == Constants.Module.PropertyNames.LoginName), propertiesStructure).ToString();
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