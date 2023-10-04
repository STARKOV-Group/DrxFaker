using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{
  partial class ParametersMatchingCollectionParametersClientHandlers
  {
    public virtual void CollectionParametersRowCountValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue.Value <= 0)
        e.AddError(Faker.Resources.ErrorNegativeNumber);
      
      foreach (var row in _obj.ParametersMatching.CollectionParameters.Where(r => r.CollectionName == _obj.CollectionName))
        row.RowCount = e.NewValue;
    }
  }

  partial class ParametersMatchingClientHandlers
  {
    public virtual void DocumentTypeValueInput(starkov.Faker.Client.ParametersMatchingDocumentTypeValueInputEventArgs e)
    {
      if (e.NewValue == null || Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.Name = e.NewValue.Name;
      AvailabilityRequisites();
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      FillRequiredPropsIntoParameters(e.NewValue.DocumentTypeGuid);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public virtual void EntityTypeValueInput(starkov.Faker.Client.ParametersMatchingEntityTypeValueInputEventArgs e)
    {
      if (e.NewValue == null || Equals(e.NewValue, e.OldValue))
        return;
      
      _obj.Name = e.NewValue.Name;
      AvailabilityRequisites();
      
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      FillRequiredPropsIntoParameters(e.NewValue.EntityTypeGuid);
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public virtual void SelectorEntityTypeValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      e.Params.AddOrUpdate(Constants.ParametersMatching.ParamsForChangeCollection, true);
      _obj.EntityType = null;
      _obj.DocumentType = null;
      _obj.IsNeedCreateVersion = false;
      _obj.IsNeedStartTask = false;
      _obj.Parameters.Clear();
      _obj.CollectionParameters.Clear();
      e.Params.Remove(Constants.ParametersMatching.ParamsForChangeCollection);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      AvailabilityRequisites();
    }

    /// <summary>
    /// Управление доступностью реквизитов.
    /// </summary>
    private void AvailabilityRequisites()
    {
      var prop = _obj.State.Properties;
      var isDocument = _obj.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Document;
      var isDataBook = _obj.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.DataBook;
      var isTask = _obj.SelectorEntityType == Faker.ParametersMatching.SelectorEntityType.Task;
      
      prop.DocumentType.IsVisible = isDocument;
      prop.EntityType.IsVisible = isDataBook || isTask;
      prop.IsNeedCreateVersion.IsVisible = isDocument;
      prop.IsNeedStartTask.IsVisible = isTask;
      prop.Name.IsEnabled = _obj.EntityType != null || _obj.DocumentType != null;
      
      //Столбцы коллекций
      prop.Parameters.Properties.ChosenValue.IsVisible = _obj.Parameters.Any(r => !string.IsNullOrEmpty(r.ChosenValue));
      prop.Parameters.Properties.ValueFrom.IsVisible = _obj.Parameters.Any(r => !string.IsNullOrEmpty(r.ValueFrom));
      prop.Parameters.Properties.ValueTo.IsVisible = _obj.Parameters.Any(r => !string.IsNullOrEmpty(r.ValueTo));
      
      prop.CollectionParameters.Properties.ChosenValue.IsVisible = _obj.CollectionParameters.Any(r => !string.IsNullOrEmpty(r.ChosenValue));
      prop.CollectionParameters.Properties.ValueFrom.IsVisible = _obj.CollectionParameters.Any(r => !string.IsNullOrEmpty(r.ValueFrom));
      prop.CollectionParameters.Properties.ValueTo.IsVisible = _obj.CollectionParameters.Any(r => !string.IsNullOrEmpty(r.ValueTo));
    }
    
    /// <summary>
    ///Заполнение обязательных свойств в табличную часть параметров.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    private void FillRequiredPropsIntoParameters(string typeGuid)
    {
      _obj.Parameters.Clear();
      var propInfo = Functions.Module.Remote.GetPropertiesType(typeGuid);
      foreach (var prop in propInfo.Where(i => i.IsRequired))
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