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
      var isAllowNotify = !_obj.IsDisableNotifications.GetValueOrDefault();
      
      _obj.State.Properties.AttachmentsNumber.IsRequired = !_obj.IsAttachAllEntities.GetValueOrDefault() && isAllowNotify;
      _obj.State.Properties.LoginNamesNumber.IsRequired = !_obj.IsShowAllLoginNames.GetValueOrDefault() && isAllowNotify;
      _obj.State.Properties.DocumentWithVersion.IsRequired = true;
      _obj.State.Properties.AsyncEntitiesNumber.IsRequired = _obj.IsSeparateAsync.GetValueOrDefault();
      
      _obj.State.Properties.AttachmentsNumber.IsEnabled = !_obj.IsAttachAllEntities.GetValueOrDefault() && isAllowNotify;
      _obj.State.Properties.LoginNamesNumber.IsEnabled = !_obj.IsShowAllLoginNames.GetValueOrDefault() && isAllowNotify;
      _obj.State.Properties.IsShowAllLoginNames.IsEnabled = isAllowNotify;
      _obj.State.Properties.IsAttachAllEntities.IsEnabled = isAllowNotify;
      _obj.State.Properties.AsyncEntitiesNumber.IsEnabled = _obj.IsSeparateAsync.GetValueOrDefault();
    }
  }
}