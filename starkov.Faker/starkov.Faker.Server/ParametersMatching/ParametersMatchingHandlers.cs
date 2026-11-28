using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{
  partial class ParametersMatchingEntityTypePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> EntityTypeFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var isTask = _obj.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Task;
      return query.Where(t => isTask ?
                         t.IsTask.HasValue && t.IsTask.Value :
                         !t.IsTask.HasValue || t.IsTask.HasValue && !t.IsTask.Value);
    }
  }

  partial class ParametersMatchingServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsNeedCreateVersion = false;
      _obj.IsNeedStartTask = false;
    }
  }

}