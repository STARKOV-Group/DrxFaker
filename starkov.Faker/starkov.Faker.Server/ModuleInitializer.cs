using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace starkov.Faker.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      FillBaseDatabookTypes();
    }
    
    /// <summary>
    /// Заполнить базовые типы справочников.
    /// </summary>
    public static void FillBaseDatabookTypes()
    {
      InitializationLogger.Debug("Init: Fill databook types");
      
      var dict = new Dictionary<string, string>()
      {
        {"Наша организация", Constants.Module.Guids.BusinessUnit},
        {"Подразделение", Constants.Module.Guids.Department},
        {"Учетная запись", Constants.Module.Guids.Login},
        {"Персона", Constants.Module.Guids.Person},
        {"Сотрудник", Constants.Module.Guids.Employee},
        {"Должность", Constants.Module.Guids.JobTitle}
      };
      
      var currentGuids = DatabookTypes.GetAll().Select(_ => _.DatabookTypeGuid).AsEnumerable();
      foreach (var row in dict.Where(_ => !currentGuids.Contains(_.Value)))
      {
        var newItem = DatabookTypes.Create();
        newItem.Name = row.Key;
        newItem.DatabookTypeGuid = row.Value;
        newItem.Save();
      }
    }
  }

}
