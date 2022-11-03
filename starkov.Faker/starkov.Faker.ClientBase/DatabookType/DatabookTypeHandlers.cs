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
        e.AddError("Недопустимый формат Guid");
        return;
      }
      
      var type = TypeExtension.GetTypeByGuid(guid);
      if (type == null)
      {
        e.AddError("Данный Guid не соответствует ни одному типу сущности");
        return;
      }
      
      var metadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(guid);
      if (metadata.IsAbstract)
      {
        e.AddError("Данный тип является абстрактным");
        return;
      }
      
      _obj.Name = metadata.GetDisplayName();
    }

  }
}