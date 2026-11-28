using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ModuleSetup;

namespace starkov.Faker.Server
{
  partial class ModuleSetupFunctions
  {

    /// <summary>
    /// Получить первую запись справочника Настройка модуля.
    /// </summary>
    /// <returns>Запись справочника Настройка модуля.</returns>
    [Remote(IsPure = true)]
    public static IModuleSetup GetModuleSetup()
    {
      return Faker.ModuleSetups.GetAll().FirstOrDefault();
    }
    
    /// <summary>
    /// Получить кол-во выводимых наименований учетных записей.
    /// </summary>
    /// <returns>Кол-во выводимых наименований учетных записей.</returns>
    public static int GetLoginNamesNumber()
    {
      var databook = GetModuleSetup();
      if (databook == null)
        return 0;
      
      return databook.IsShowAllLoginNames.GetValueOrDefault() ? Int32.MaxValue : databook.LoginNamesNumber.GetValueOrDefault();
    }
    
    /// <summary>
    /// Получить кол-во сущностей вкладываемых в уведомление.
    /// </summary>
    /// <returns>Кол-во сущностей вкладываемых в уведомление.</returns>
    public static int GetAttachmentsNumber()
    {
      var databook = GetModuleSetup();
      if (databook == null)
        return 0;
      
      return databook.IsAttachAllEntities.GetValueOrDefault() ? Int32.MaxValue : databook.AttachmentsNumber.GetValueOrDefault();
    }
    
    /// <summary>
    /// Получить документ, использующийся при генерации версий документов.
    /// </summary>
    /// <returns>Документ, использующийся при генерации версий документов.</returns>
    public static Sungero.Docflow.IOfficialDocument GetDocumentWithVersion()
    {
      var databook = GetModuleSetup();
      if (databook == null)
        return null;
      
      return databook.DocumentWithVersion;
    }

  }
}