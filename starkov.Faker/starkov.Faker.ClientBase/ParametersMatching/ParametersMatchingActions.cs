using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker.Client
{
  partial class ParametersMatchingParametersActions
  {
    public virtual void ChangeSelectedDataInParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      Functions.ParametersMatching.ShowDialogForSelectParameters(_obj.ParametersMatching, _obj.Id, !string.IsNullOrEmpty(_obj.FillOption));
    }

    public virtual bool CanChangeSelectedDataInParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }
  }

  internal static class ParametersMatchingParametersStaticActions
  {

    public static bool CanAddDataInParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }

    public static void AddDataInParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = ParametersMatchings.As(e.RootEntity);
      if (obj.DatabookType == null && obj.DocumentType == null)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.ErrorToAddDataFillEntityType);
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      Functions.ParametersMatching.ShowDialogForSelectParameters(obj, null);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public static bool CanChangeDataInParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }

    public static void ChangeDataInParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = ParametersMatchings.As(e.RootEntity);
      if (obj.DatabookType == null && obj.DocumentType == null)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.ErrorToChangeDataFillEntityType);
      
      Functions.ParametersMatching.ShowDialogForSelectParameters(obj, null, false, true);
    }
  }
}