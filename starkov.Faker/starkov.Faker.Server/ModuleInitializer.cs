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
        {"Наша организация", "eff95720-181f-4f7d-892d-dec034c7b2ab"},
        {"Подразделение", "61b1c19f-26e2-49a5-b3d3-0d3618151e12"},
        {"Учетная запись", "55f542e9-4645-4f8d-999e-73cc71df62fd"},
        {"Персона", "f5509cdc-ac0c-4507-a4d3-61d7a0a9b6f6"},
        {"Сотрудник", "b7905516-2be5-4931-961c-cb38d5677565"},
        {"Должность", "4a37aec4-764c-4c14-8887-e1ecafa5b4c5"}
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
