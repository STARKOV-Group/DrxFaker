﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.DatabookType;

namespace starkov.Faker.Shared
{
  partial class DatabookTypeFunctions
  {
    /// <summary>
    /// Управление доступностью реквизитов.
    /// </summary>
    public virtual void SetProperties()
    {
      _obj.State.Properties.Name.IsEnabled = !string.IsNullOrEmpty(_obj.DatabookTypeGuid);
    }
  }
}