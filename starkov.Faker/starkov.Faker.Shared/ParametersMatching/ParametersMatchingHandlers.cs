using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{
  partial class ParametersMatchingParametersSharedCollectionHandlers
  {

    public virtual void ParametersDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      bool isAllow;
      if (_deleted.IsRequired.GetValueOrDefault() &&
         (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow))
        throw AppliedCodeException.Create("Нельзя удалять обязательные для заполнения значения");
    }

    public virtual void ParametersAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      bool isAllow;
      if (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow)
        throw AppliedCodeException.Create("Добавлять значения можно только по соответствующей кнопке");
    }
  }

  partial class ParametersMatchingSharedHandlers
  {

  }
}