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

    public virtual void DatabookTypeGuidValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      var guid = Guid.Empty;
      if (!Guid.TryParse(e.NewValue, out guid))
      {
        e.AddError(starkov.Faker.DatabookTypes.Resources.ErrorInvalidGuid);
        return;
      }
      
      var type = TypeExtension.GetTypeByGuid(guid);
      if (type == null)
      {
        e.AddError(starkov.Faker.DatabookTypes.Resources.ErrorGuidNotMatchAnyEntity);
        return;
      }
      
      //Метаданные базового типа
      var databookEntryType = typeof(Sungero.CoreEntities.IDatabookEntry);
      var baseMetadata = databookEntryType.GetEntityMetadata();
      var entityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(guid);
      
      //Если baseMetadata не является предком для entityMetadata или entityMetadata является абстрактным типом
      if (entityMetadata == null || !baseMetadata.IsAncestorFor(entityMetadata) || entityMetadata.IsAbstract)
      {
        e.AddError(starkov.Faker.DatabookTypes.Resources.ErrorTypeIsAbstract);
        return;
      }
      
      _obj.Name = entityMetadata.GetDisplayName();
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.DatabookType.SetProperties(_obj);
    }

  }
}