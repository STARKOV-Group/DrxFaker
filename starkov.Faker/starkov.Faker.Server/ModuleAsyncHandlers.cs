using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Bogus;
using PdfSharp.Pdf;
using Sungero.Domain.Shared;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace starkov.Faker.Server
{
  public class ModuleAsyncHandlers
  {

    /// <summary>
    /// Асинхронный обработчик для генерации сущностей
    /// </summary>
    public virtual void EntitiesGeneration(starkov.Faker.Server.AsyncHandlerInvokeArgs.EntitiesGenerationInvokeArgs args)
    {
      Logger.DebugFormat("Start async handler EntitiesGeneration");
      args.Retry = false;
      
      var databook = ParametersMatchings.GetAll(_ => _.Id == args.DatabookId).FirstOrDefault();
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
      var firstEntityId = 0;
      
      for (var i = 0; i < args.Count; i++)
      {
        try
        {
          using (Sungero.Domain.Session session = new Sungero.Domain.Session(true, false))
          {
            #region Создание учетных записей
            if (databook.DatabookType?.DatabookTypeGuid == Constants.Module.Guids.Login)
            {
              var login = Functions.Module.GetPropertyValueByParameters(databook.Parameters.FirstOrDefault(_ => _.PropertyName == Constants.Module.PropertyNames.LoginName), propertiesStructure) as string;
              var password = databook.Parameters.FirstOrDefault(_ => _.PropertyName == Constants.Module.PropertyNames.Password).ChosenValue;
              Sungero.Company.PublicFunctions.Module.CreateLogin(login, password);
              
              if (loginNames.Count <= 30)
                loginNames.Add(login);
              createdEntityCount++;
              
              session.Dispose();
              continue;
            }
            #endregion
            
            var finalTypeGuid = databook.DatabookType?.DatabookTypeGuid ?? databook.DocumentType?.DocumentTypeGuid;
            var entity = Functions.Module.CreateEntityByTypeGuid(finalTypeGuid);
            var entityProperties = entity.GetType().GetProperties();
            
            #region Заполнение свойств сущности
            foreach (var parametersRow in databook.Parameters.Where(_ => _.FillOption != Constants.Module.FillOptions.Common.NullValue))
            {
              try
              {
                var property = entityProperties.FirstOrDefault(_ => _.Name == parametersRow.PropertyName);
                if (property == null)
                  continue;
                
                var propertyValue = Functions.Module.GetPropertyValueByParameters(parametersRow, propertiesStructure);
                if (propertyValue is string && parametersRow.StringPropLength.HasValue)
                {
                  var str = propertyValue as string;
                  if (str.Length > parametersRow.StringPropLength.Value)
                    propertyValue = str.Substring(0, parametersRow.StringPropLength.Value);
                }
                else if (Equals(property.PropertyType, typeof(double)) || Equals(property.PropertyType, typeof(double?)))
                  propertyValue = Convert.ToDouble(propertyValue);
                
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
            if (databook.EntityType == Faker.ParametersMatching.EntityType.Document && databook.IsNeedCreateVersion.GetValueOrDefault())
            {
              try
              {
                var document = Sungero.Docflow.OfficialDocuments.As(entity);
                var emptyPdf = new PdfDocument();
                emptyPdf.AddPage();

                using (var stream = new MemoryStream())
                {
                  emptyPdf.Save(stream, false);
                  stream.Seek(0, SeekOrigin.Begin);
                  document.CreateVersionFrom(stream, "pdf");
                }
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
              if (attachments.Count <= 30)
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
            finally
            {
              session.Dispose();
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
      
      #region Отправка уведомления администраторам
      var administrators = Roles.Administrators.RecipientLinks.Select(_ => _.Member);
      var task = Sungero.Workflow.SimpleTasks.CreateWithNotices(starkov.Faker.Resources.NoticeSubjectFormat(createdEntityCount, args.Count, databook.Name),
                                                                administrators.ToArray());
      
      var text = string.Empty;
      if (firstEntityId != 0)
        text += starkov.Faker.Resources.InfoText_FirstEntryIDFormat(firstEntityId);
      text += starkov.Faker.Resources.InfoText_TimeSpentToCreatEntitiesFormat(string.IsNullOrEmpty(text) ? string.Empty : "\n", elapsedTime);
      if (loginNames.Any())
        text += starkov.Faker.Resources.InfoText_LoginNamesFormat(string.Join(", ", loginNames));
      if (errors.Any())
        text += starkov.Faker.Resources.InfoText_ErrorsFormat(string.Join("\n", errors));
      
      foreach (var attachment in attachments)
        task.Attachments.Add(attachment);
      
      task.ActiveText = text;
      task.Start();
      #endregion
      
      Logger.DebugFormat("End async handler EntitiesGeneration");
    }
  }
}