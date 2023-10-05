using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker
{
  partial class ParametersMatchingAttachmentParametersClientHandlers
  {

    public virtual void AttachmentParametersCountValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (!e.NewValue.HasValue)
        return;
      
      if (e.NewValue.Value <= 0)
      {
        e.AddError(Faker.Resources.ErrorNegativeNumber);
        return;
      }
      
      if (_obj.Limit.HasValue && _obj.Limit.Value < e.NewValue.Value)
        e.AddError(starkov.Faker.ParametersMatchings.Resources.Error_GreaterThenLimitFormat(_obj.Limit));
    }
  }

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
      _obj.CollectionParameters.Clear();
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
      if (_obj.SelectorEntityType == SelectorEntityType.Task)
      {
        _obj.CollectionParameters.Clear();
        FillRequiredAttachments(e.NewValue.EntityTypeGuid);
      }
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
      _obj.AttachmentParameters.Clear();
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
      prop.AttachmentParameters.IsVisible = isTask;
      prop.Name.IsEnabled = _obj.EntityType != null || _obj.DocumentType != null;
      
      //Столбцы коллекций
      prop.Parameters.Properties.ChosenValue.IsVisible = _obj.Parameters.Any(r => !string.IsNullOrEmpty(r.ChosenValue));
      prop.Parameters.Properties.ValueFrom.IsVisible = _obj.Parameters.Any(r => !string.IsNullOrEmpty(r.ValueFrom));
      prop.Parameters.Properties.ValueTo.IsVisible = _obj.Parameters.Any(r => !string.IsNullOrEmpty(r.ValueTo));
      
      prop.CollectionParameters.Properties.ChosenValue.IsVisible = _obj.CollectionParameters.Any(r => !string.IsNullOrEmpty(r.ChosenValue));
      prop.CollectionParameters.Properties.ValueFrom.IsVisible = _obj.CollectionParameters.Any(r => !string.IsNullOrEmpty(r.ValueFrom));
      prop.CollectionParameters.Properties.ValueTo.IsVisible = _obj.CollectionParameters.Any(r => !string.IsNullOrEmpty(r.ValueTo));
      
      prop.AttachmentParameters.Properties.ChosenValue.IsVisible = _obj.AttachmentParameters.Any(r => !string.IsNullOrEmpty(r.ChosenValue));
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
    
    /// <summary>
    ///Заполнение информации об обязательных группах вложений.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    private void FillRequiredAttachments(string typeGuid)
    {
      _obj.AttachmentParameters.Clear();
      var propInfo = Functions.Module.Remote.GetAttachmentPropertiesType(typeGuid);
      foreach (var prop in propInfo.Where(i => i.IsRequired))
      {
        var newRow = _obj.AttachmentParameters.AddNew();
        newRow.AttachmentName = prop.Name;
        newRow.LocalizedAttachmentName = prop.LocalizedName;
        newRow.PropertyType = Constants.Module.CustomType.Navigation;
        newRow.PropertyTypeGuid = prop.PropertyGuid;
        newRow.Limit = prop.LimitCount;
        newRow.IsRequired = prop.IsRequired;
      }
    }
  }
}