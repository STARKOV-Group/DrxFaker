using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;
using Sungero.Domain.Shared;
using PdfSharp.Pdf;
using System.IO;

namespace starkov.Faker.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      FillDatabookTypes();
      
      var databook = CreateModuleSetup();
      CreateDocWithVersionForModuleSetup(databook);
    }
    
    /// <summary>
    /// Заполнить типы справочников.
    /// </summary>
    public static void FillDatabookTypes()
    {
      InitializationLogger.Debug("Init: Fill databook types");
      
      //Метаданные базового типа
      var databookEntryType = typeof(Sungero.CoreEntities.IDatabookEntry);
      var baseMetadata = databookEntryType.GetEntityMetadata();
      
      var databookTypes = DatabookTypes.GetAll();
      var typesGuidList = GetSelectGuids();
      foreach (var typeGuid in typesGuidList)
      {
        try
        {
          var entityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(typeGuid);
          //Если baseMetadata не является предком для entityMetadata или entityMetadata является абстрактным типом
          if (entityMetadata == null || !baseMetadata.IsAncestorFor(entityMetadata) || entityMetadata.IsAbstract)
            continue;
          
          var type = TypeExtension.GetTypeByGuid(typeGuid);
          var finalEntityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(type.GetFinalType());
          
          var databookWithOldType = databookTypes.FirstOrDefault(t => t.DatabookTypeGuid == typeGuid.ToString());
          var databookWithFinalType = databookTypes.FirstOrDefault(t => t.DatabookTypeGuid == finalEntityMetadata.NameGuid.ToString());
          var databookWithAncestorType = databookTypes.FirstOrDefault(t => t.AncestorGuids.Any(anc => anc.Guid == typeGuid.ToString()));
          
          if (finalEntityMetadata == null || Equals(finalEntityMetadata.NameGuid, typeGuid))
          {
            if (databookWithAncestorType != null)
            {
              databookWithAncestorType.DatabookTypeGuid = typeGuid.ToString();
              databookWithAncestorType.AncestorGuids.Remove(databookWithAncestorType.AncestorGuids.FirstOrDefault(anc => anc.Guid == typeGuid.ToString()));
              databookWithAncestorType.Save();
            }
            else if (databookWithOldType == null)
            {
              var newType = DatabookTypes.Create();
              newType.Name = entityMetadata.GetDisplayName();
              newType.DatabookTypeGuid = typeGuid.ToString();
              newType.Save();
            }
            continue;
          }
          
          if (databookWithAncestorType != null && databookWithAncestorType.DatabookTypeGuid != finalEntityMetadata.NameGuid.ToString())
          {
            databookWithAncestorType.DatabookTypeGuid = finalEntityMetadata.NameGuid.ToString();
            databookWithAncestorType.Save();
          }
          else if (databookWithFinalType != null && !databookWithFinalType.AncestorGuids.Any(anc => Equals(anc.Guid, typeGuid.ToString())))
          {
            databookWithFinalType.AncestorGuids.AddNew().Guid = typeGuid.ToString();
            databookWithFinalType.Save();
          }
          else if (databookWithOldType != null)
          {
            databookWithOldType.DatabookTypeGuid = finalEntityMetadata.NameGuid.ToString();
            databookWithOldType.AncestorGuids.AddNew().Guid = typeGuid.ToString();
            databookWithOldType.Save();
          }
          else if (databookWithAncestorType == null && databookWithFinalType == null && databookWithOldType == null)
          {
            var newType = DatabookTypes.Create();
            newType.Name = finalEntityMetadata.GetDisplayName();
            newType.DatabookTypeGuid = finalEntityMetadata.NameGuid.ToString();
            newType.AncestorGuids.AddNew().Guid = typeGuid.ToString();
            newType.Save();
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Faker init fill databook types error: {0}", ex.Message);
        }
      }
    }
    
    /// <summary>
    /// Получить список Guid-ов объектов для заполнения.
    /// </summary>
    /// <returns>Список Guid.</returns>
    public static List<Guid> GetSelectGuids()
    {
      return new List<Guid>
      {
        Constants.Module.Guids.BusinessUnit,
        Constants.Module.Guids.Company,
        Constants.Module.Guids.Department,
        Constants.Module.Guids.Employee,
        Constants.Module.Guids.JobTitle,
        Constants.Module.Guids.Login,
        Constants.Module.Guids.Person
      };
    }
    
    /// <summary>
    /// Создать запись справочника Настройка модуля.
    /// </summary>
    /// <returns>Справочник Настройка модуля.</returns>
    public static Faker.IModuleSetup CreateModuleSetup()
    {
      InitializationLogger.Debug("Init: Create module setup");
      
      var databook = Faker.ModuleSetups.GetAll().FirstOrDefault();
      if (databook == null)
      {
        databook = Faker.ModuleSetups.Create();
        databook.Name = starkov.Faker.Resources.ModuleSetupsDatabookName;
        databook.LoginNamesNumber = 30;
        databook.AttachmentsNumber = 30;
        databook.Save();
      }
      
      return databook;
    }
    
    /// <summary>
    /// Создать документ, который будет использоваться для генрации версий документов.
    /// </summary>
    /// <param name="databook">Справочник Настройка модуля.</param>
    public static void CreateDocWithVersionForModuleSetup(Faker.IModuleSetup databook)
    {
      InitializationLogger.Debug("Init: Create document with version for module setup");
      
      if (databook == null || databook.DocumentWithVersion != null)
        return;
      
      var document = Sungero.Docflow.SimpleDocuments.Create();
      document.Name = starkov.Faker.Resources.DocumentWithVersionName;
      
      var emptyPdf = new PdfDocument();
      emptyPdf.AddPage();
      
      using (var stream = new MemoryStream())
      {
        emptyPdf.Save(stream, false);
        stream.Seek(0, SeekOrigin.Begin);
        document.CreateVersionFrom(stream, Constants.Module.DocumentFormats.Pdf);
      }
      
      emptyPdf.Dispose();
      
      foreach (var propState in document.State.Properties)
        propState.IsRequired = false;
      document.Save();
      
      databook.DocumentWithVersion = document;
      databook.Save();
    }
  }
}
