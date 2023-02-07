using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ModuleSetup;

namespace starkov.Faker.Shared
{
  partial class ModuleSetupFunctions
  {

    /// <summary>
    /// Управление состоянием реквизитов.
    /// </summary>
    public virtual void SetProperties()
    {
      _obj.State.Properties.AttachmentsNumber.IsRequired = !_obj.IsAttachAllEntities.GetValueOrDefault();
      _obj.State.Properties.AttachmentsNumber.IsEnabled = !_obj.IsAttachAllEntities.GetValueOrDefault();
      _obj.State.Properties.LoginNamesNumber.IsRequired = !_obj.IsShowAllLoginNames.GetValueOrDefault();
      _obj.State.Properties.LoginNamesNumber.IsEnabled = !_obj.IsShowAllLoginNames.GetValueOrDefault();
      
      _obj.State.Properties.DocumentWithVersion.IsRequired = true;
    }
  }
}