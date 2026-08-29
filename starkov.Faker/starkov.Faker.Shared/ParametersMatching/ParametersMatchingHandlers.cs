using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{
  partial class ParametersMatchingAttachmentParametersSharedCollectionHandlers
  {

    public virtual void AttachmentParametersDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      bool isAllow;
      if (_deleted.IsRequired.GetValueOrDefault() &&
         (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow))
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.Error_DeleteReqValue);
    }
  }

  partial class ParametersMatchingCollectionParametersSharedCollectionHandlers
  {

    public virtual void CollectionParametersDeleted(Sungero.Domain.Shared.CollectionPropertyDeletedEventArgs e)
    {
      bool isAllow;
      if (!e.Params.TryGetValue(Constants.ParametersMatching.ParamsForChangeCollection, out isAllow) || !isAllow)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.Error_WrongWayToDelete);
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
  }

  partial class ParametersMatchingSharedHandlers
  {

  }
}