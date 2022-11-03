using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker.Client
{

  internal static class ParametersMatchingParametersStaticActions
  {

    public static bool CanAddDataInParameters(Sungero.Domain.Client.CanExecuteChildCollectionActionArgs e)
    {
      return true;
    }

    public static void AddDataInParameters(Sungero.Domain.Client.ExecuteChildCollectionActionArgs e)
    {
      var obj = e.RootEntity as IParametersMatching;
      if (obj.DatabookType == null && obj.DocumentType == null)
        throw AppliedCodeException.Create("Для добавления данных заполните \"Тип справочника\" или \"Тип документа\"");
      
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
      var obj = e.RootEntity as IParametersMatching;
      if (obj.DatabookType == null && obj.DocumentType == null)
        throw AppliedCodeException.Create("Для изменения данных заполните \"Тип справочника\" или \"Тип документа\"");
      
      Functions.ParametersMatching.ShowDialogForChangeParameters(obj);
    }
  }
}