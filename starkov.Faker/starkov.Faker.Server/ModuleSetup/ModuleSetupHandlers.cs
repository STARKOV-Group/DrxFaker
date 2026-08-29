using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ModuleSetup;

namespace starkov.Faker
{
  partial class ModuleSetupDocumentWithVersionPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> DocumentWithVersionFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(d => d.HasVersions);
    }
  }

  partial class ModuleSetupServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsShowAllLoginNames = false;
      _obj.IsAttachAllEntities = false;
      _obj.IsSeparateAsync = false;
      _obj.IsDisableNotifications = false;
    }
  }

}