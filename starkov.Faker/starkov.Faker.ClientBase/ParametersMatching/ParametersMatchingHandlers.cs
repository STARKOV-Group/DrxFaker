using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{
  partial class ParametersMatchingClientHandlers
  {

    public virtual void DocumentTypeValueInput(starkov.Faker.Client.ParametersMatchingDocumentTypeValueInputEventArgs e)
    {
      if (e.NewValue == null || Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.Name = e.NewValue.Name;
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      FillRequiredPropsIntoParameters(e.NewValue.DocumentTypeGuid);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public virtual void DatabookTypeValueInput(starkov.Faker.Client.ParametersMatchingDatabookTypeValueInputEventArgs e)
    {
      if (e.NewValue == null || Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.Name = e.NewValue.Name;
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      FillRequiredPropsIntoParameters(e.NewValue.DatabookTypeGuid);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public virtual void EntityTypeValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      _obj.DatabookType = null;
      _obj.DocumentType = null;
      _obj.IsNeedCreateVersion = false;
      _obj.Parameters.Clear();
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      AvailabilityRequisites();
    }

    /// <summary>
    /// Управление доступностью реквизитов
    /// </summary>
    private void AvailabilityRequisites()
    {
      var prop = _obj.State.Properties;
      var isDocument = _obj.EntityType == EntityType.Document;
      var isDataBook = _obj.EntityType == EntityType.DataBook;
      
      prop.DocumentType.IsVisible = isDocument;
      prop.DatabookType.IsVisible = isDataBook;
      prop.IsNeedCreateVersion.IsVisible = isDocument;
    }
    
    /// <summary>
    ///Заполнение обязательных свойств в табличную часть параметров
    /// </summary>
    /// <param name="typeGuid">Guid ипа сущности</param>
    private void FillRequiredPropsIntoParameters(string typeGuid)
    {
      _obj.Parameters.Clear();
      var propInfo = Functions.Module.Remote.GetPropertiesType(typeGuid);
      foreach (var prop in propInfo.Where(_ => _.IsRequired))
      {
        var newRow = _obj.Parameters.AddNew();
        newRow.PropertyName = prop.Name;
        newRow.LocalizedPropertyName = prop.LocalizedName;
        newRow.PropertyType = Functions.ParametersMatching.GetMatchingTypeToCustomType(prop.Type);
        newRow.PropertyTypeGuid = prop.PropertyGuid;
        newRow.IsRequired = prop.IsRequired;
        newRow.StringPropLength = prop.MaxStringLength;
      }
    }
  }
}