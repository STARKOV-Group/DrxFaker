using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.EntityType;
using Sungero.Domain.Shared;

namespace starkov.Faker
{
  partial class EntityTypeSharedHandlers
  {

    public virtual void EntityTypeGuidChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      var entityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(Guid.Parse(e.NewValue));
      var taskType = typeof(Sungero.Workflow.ITask);
      var taskMetadata = taskType.GetEntityMetadata();
      _obj.IsTask = taskMetadata.IsAncestorFor(entityMetadata);
    }
  }
}