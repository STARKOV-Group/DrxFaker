using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.EntityType;
using Sungero.Domain.Shared;

namespace starkov.Faker
{
  partial class EntityTypeClientHandlers
  {

    public virtual void EntityTypeGuidValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      var guid = Guid.Empty;
      if (!Guid.TryParse(e.NewValue, out guid))
      {
        e.AddError(starkov.Faker.EntityTypes.Resources.ErrorInvalidGuid);
        return;
      }
      
      var type = TypeExtension.GetTypeByGuid(guid);
      if (type == null)
      {
        e.AddError(starkov.Faker.EntityTypes.Resources.ErrorGuidNotMatchAnyEntity);
        return;
      }
      
      var entityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(guid);
      //Если entityMetadata является абстрактным типом
      if (entityMetadata == null  || entityMetadata.IsAbstract)
      {
        e.AddError(starkov.Faker.EntityTypes.Resources.ErrorTypeIsAbstract);
        return;
      }
      
      _obj.Name = entityMetadata.GetDisplayName();
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.EntityType.SetProperties(_obj);
    }

  }
}