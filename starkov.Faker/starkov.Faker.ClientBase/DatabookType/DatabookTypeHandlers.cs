using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.DatabookType;
using Sungero.Domain.Shared;

namespace starkov.Faker
{
  partial class DatabookTypeClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.Name.IsEnabled = !string.IsNullOrEmpty(_obj.DatabookTypeGuid);
    }

  }
}