using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;
using Sungero.Domain.Shared;
using PdfSharp.Pdf;
using System.IO;
using Sungero.Company;
using Sungero.Parties;

namespace starkov.Faker.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      FillDatabookTypes();
      FillParametersMatching();
      
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
    /// Заполнить справочник "Соответствие заполняемых параметров сущности".
    /// </summary>
    public static void FillParametersMatching()
    {
      if (ParametersMatchings.GetAll().Any())
        return;
      
      InitializationLogger.Debug("Init: Fill parameters matching");
      
      #region Создание записи для Нашей организации
      var businessUnitProp = BusinessUnits.Info.Properties;
      var businessUnitDict = new Dictionary<string, string>() {
        { businessUnitProp.Status.Name, Constants.Module.FillOptions.Common.NullValue },
        { businessUnitProp.Name.Name, Constants.Module.FillOptions.String.CompanyName }
      };
      
      CreateParametersMatchingByGUID(Constants.Module.Guids.BusinessUnit, businessUnitDict);
      #endregion
      
      #region Создание записи для Подразделения
      var departmentProp = Departments.Info.Properties;
      var departmentDict = new Dictionary<string, string>() {
        { departmentProp.Status.Name, Constants.Module.FillOptions.Common.NullValue },
        { departmentProp.Name.Name, Constants.Module.FillOptions.String.Department }
      };
      
      CreateParametersMatchingByGUID(Constants.Module.Guids.Department, departmentDict);
      #endregion
      
      #region Создание записи для Должности
      var jobTitleProp = JobTitles.Info.Properties;
      var jobTitleDict = new Dictionary<string, string>() {
        { jobTitleProp.Status.Name, Constants.Module.FillOptions.Common.NullValue },
        { jobTitleProp.Name.Name, Constants.Module.FillOptions.String.JobTitle }
      };
      
      CreateParametersMatchingByGUID(Constants.Module.Guids.JobTitle, jobTitleDict);
      #endregion
      
      #region Создание записи для Учетной записи
      var loginProp = Logins.Info.Properties;
      var loginDict = new Dictionary<string, string>() {
        { loginProp.Status.Name, Constants.Module.FillOptions.Common.NullValue },
        { loginProp.LoginName.Name, Constants.Module.FillOptions.String.Login },
        { Constants.Module.PropertyNames.Password, "11111" }
      };
      
      CreateParametersMatchingByGUID(Constants.Module.Guids.Login, loginDict);
      #endregion
      
      #region Создание записи для Персоны
      var personProp = People.Info.Properties;
      var personDict = new Dictionary<string, string>() {
        { personProp.Status.Name, Constants.Module.FillOptions.Common.NullValue },
        { personProp.Name.Name, Constants.Module.FillOptions.Common.NullValue },
        { personProp.LastName.Name, Constants.Module.FillOptions.String.LastName },
        { personProp.FirstName.Name, Constants.Module.FillOptions.String.FirstName }
      };
      
      CreateParametersMatchingByGUID(Constants.Module.Guids.Person, personDict);
      #endregion
      
      #region Создание записи для Сотрудника
      var employeeProp = Employees.Info.Properties;
      var employeeDict = new Dictionary<string, string>() {
        { employeeProp.Status.Name, Constants.Module.FillOptions.Common.NullValue },
        { employeeProp.Name.Name, Constants.Module.FillOptions.Common.NullValue },
        { employeeProp.Person.Name, Constants.Module.FillOptions.Common.RandomValue },
        { employeeProp.Department.Name, Constants.Module.FillOptions.Common.RandomValue }
      };
      
      CreateParametersMatchingByGUID(Constants.Module.Guids.Employee, employeeDict);
      #endregion
    }
    
    #region Функции для работы с справочником "Соответствие заполняемых параметров сущности"
    
    /// <summary>
    /// Создать запись справочника "Соответствие заполняемых параметров сущности".
    /// </summary>
    /// <param name="databookGuid">GUID справочника.</param>
    /// <param name="dict">Словарь с наименованием свойства и вариантом его заполнения.</param>
    private static void CreateParametersMatchingByGUID(Guid databookGuid, Dictionary<string, string> dict)
    {
      try
      {
        var stringGuid = databookGuid.ToString();
        var databook = ParametersMatchings.Create();
        databook.EntityType = Faker.ParametersMatching.EntityType.DataBook;
        databook.DatabookType = DatabookTypes.GetAll(t => t.DatabookTypeGuid == stringGuid || t.AncestorGuids.Any(a => a.Guid == stringGuid)).FirstOrDefault();
        databook.Name = databook.DatabookType.Name;
        
        ((Sungero.Domain.Shared.IExtendedEntity)databook).Params[Constants.ParametersMatching.ParamsForChangeCollection] = true;
        var propInfo = Functions.Module.GetPropertiesType(stringGuid);
        foreach (var prop in propInfo.Where(i => i.IsRequired))
        {
          var newRow = databook.Parameters.AddNew();
          newRow.PropertyName = prop.Name;
          newRow.LocalizedPropertyName = prop.LocalizedName;
          newRow.PropertyType = Functions.ParametersMatching.GetMatchingTypeToCustomType(prop.Type);
          newRow.PropertyTypeGuid = prop.PropertyGuid;
          newRow.IsRequired = prop.IsRequired;
          newRow.StringPropLength = prop.MaxStringLength;
        }
        
        foreach (var row in dict)
          FillOptionInCollection(databook, row.Key, row.Value);
        
        foreach (var emptyRow in databook.Parameters.Where(r => string.IsNullOrEmpty(r.FillOption)))
          emptyRow.FillOption = Constants.Module.FillOptions.Common.NullValue;
        
        databook.Save();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Init: CreateParametersMatchingByGUID error ", ex);
      }
    }
    
    /// <summary>
    /// Заполнить свойство "Вариант заполнения" в строке коллекции параметров.
    /// </summary>
    /// <param name="databook">Справочник.</param>
    /// <param name="propertyName">Наименование свойства.</param>
    /// <param name="fillOption">Вариант заполнения.</param>
    private static void FillOptionInCollection(IParametersMatching databook, string propertyName, string fillOption)
    {
      var row = databook.Parameters.FirstOrDefault(r => r.PropertyName == propertyName);
      if (row != null)
        row.FillOption = fillOption;
    }
    
    #endregion
    
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
