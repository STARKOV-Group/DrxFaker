using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ModuleSetup;

namespace starkov.Faker
{
  partial class ModuleSetupClientHandlers
  {

    public virtual void AsyncEntitiesNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value <= 0)
        e.AddError(Faker.Resources.ErrorNegativeNumber);
    }

    public virtual void LoginNamesNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.GetValueOrDefault() < 0)
        throw new AppliedCodeException(starkov.Faker.ModuleSetups.Resources.ErrorNegativeValue);
    }

    public virtual void AttachmentsNumberValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.GetValueOrDefault() < 0)
        throw new AppliedCodeException(starkov.Faker.ModuleSetups.Resources.ErrorNegativeValue);
    }

    public virtual void IsDisableNotificationsValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.ModuleSetup.SetProperties(_obj);
    }

    public virtual void IsSeparateAsyncValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.ModuleSetup.SetProperties(_obj);
      
      if (e.NewValue.GetValueOrDefault() && !_obj.AsyncEntitiesNumber.HasValue)
        _obj.AsyncEntitiesNumber = Faker.Constants.Module.BaseEntitiesCount;
    }

    public virtual void IsAttachAllEntitiesValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.ModuleSetup.SetProperties(_obj);
    }

    public virtual void IsShowAllLoginNamesValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      Functions.ModuleSetup.SetProperties(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.ModuleSetup.SetProperties(_obj);
    }

  }
}