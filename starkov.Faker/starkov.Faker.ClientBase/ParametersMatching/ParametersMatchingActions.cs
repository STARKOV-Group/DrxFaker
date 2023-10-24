using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker.Client
{
  partial class ParametersMatchingAttachmentParametersActions
  {
    public virtual void ChangeSelectedDataInAttachments(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      Functions.ParametersMatching.ShowDialogForSelectAttachments(_obj.ParametersMatching, _obj.Id);
    }

    public virtual bool CanChangeSelectedDataInAttachments(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }
  }

  internal static class ParametersMatchingAttachmentParametersStaticActions
  {
    public static void AddDataInAttachments(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = ParametersMatchings.As(e.RootEntity);
      if (obj.EntityType == null)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.ErrorToAddAttachmentFillEntityType);
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      Functions.ParametersMatching.ShowDialogForSelectAttachments(obj, null);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public static bool CanAddDataInAttachments(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }
  }

  partial class ParametersMatchingCollectionParametersActions
  {
    public virtual void DeleteDataInCollectionParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      foreach (var row in _obj.ParametersMatching.CollectionParameters.Where(r => r.CollectionName == _obj.CollectionName).ToList())
        _obj.ParametersMatching.CollectionParameters.Remove(row);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public virtual bool CanDeleteDataInCollectionParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }

    public virtual void ChangeSelectedDataInCollectionParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      Functions.ParametersMatching.ShowDialogForSelectCollectionParameters(_obj.ParametersMatching, _obj.Id, !string.IsNullOrEmpty(_obj.FillOption));
    }

    public virtual bool CanChangeSelectedDataInCollectionParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }
  }

  internal static class ParametersMatchingCollectionParametersStaticActions
  {
    public static void AddDataInCollectionParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = ParametersMatchings.As(e.RootEntity);
      if (obj.EntityType == null && obj.DocumentType == null)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.ErrorToAddDataFillEntityType);
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      Functions.ParametersMatching.ShowDialogForSelectCollectionParameters(obj, null, false);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public static bool CanAddDataInCollectionParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }
  }

  partial class ParametersMatchingParametersActions
  {
    public virtual void ChangeSelectedDataInParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      Functions.ParametersMatching.ShowDialogForSelectParameters(_obj.ParametersMatching, _obj.Id);
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
      if (obj.EntityType == null && obj.DocumentType == null)
        throw AppliedCodeException.Create(starkov.Faker.ParametersMatchings.Resources.ErrorToAddDataFillEntityType);
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      Functions.ParametersMatching.ShowDialogForSelectParameters(obj, null);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }
  }
}