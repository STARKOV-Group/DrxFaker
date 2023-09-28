using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{

  partial class ParametersMatchingCollectionParametersSharedCollectionHandlers
  {

    public virtual void CollectionParametersDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      bool isAllow;
      if (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.Error_WrongWayToDelete);
    }

    public virtual void CollectionParametersAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      bool isAllow;
      if (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.Error_WrongWayToAddValue);
    }
  }

  partial class ParametersMatchingParametersSharedCollectionHandlers
  {

    public virtual void ParametersDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      bool isAllow;
      if (_deleted.IsRequired.GetValueOrDefault() &&
         (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow))
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.Error_DeleteReqValue);
    }

    public virtual void ParametersAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      bool isAllow;
      if (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.Error_WrongWayToAddValue);
    }
  }

  partial class ParametersMatchingSharedHandlers
  {

  }
}