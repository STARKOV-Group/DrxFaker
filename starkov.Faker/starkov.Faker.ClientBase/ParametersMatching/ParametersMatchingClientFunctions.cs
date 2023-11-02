using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;
using System.Text.RegularExpressions;
using CommonLibrary;
using Sungero.Domain.Shared;

namespace starkov.Faker.Client
{
  partial class ParametersMatchingFunctions
  {
    
    #region Диалог для коллекции CollectionParameters
    
    /// <summary>
    /// Показ диалога для выбора данных.
    /// </summary>
    /// <param name="rowId">Номер строки.</param>
    public void ShowDialogForSelectAttachments(int? rowId)
    {
      var dialog = Dialogs.CreateInputDialog(starkov.Faker.ParametersMatchings.Resources.DialogDataInput);
      
      #region Данные для диалога
      var parameterRow = _obj.AttachmentParameters.FirstOrDefault(p => p.Id == rowId.GetValueOrDefault());
      var selectedAttachmentNames = _obj.AttachmentParameters.Select(p => p.AttachmentName);
      var propInfo = Functions.Module.Remote.GetAttachmentPropertiesType(_obj.EntityType?.EntityTypeGuid)
        .Where(i => rowId.HasValue ? parameterRow.AttachmentName == i.Name : !selectedAttachmentNames.Contains(i.Name));
      
      if (!propInfo.Any())
      {
        if (parameterRow == null)
          Dialogs.ShowMessage(starkov.Faker.ParametersMatchings.Resources.DialogInfoNoAvailableParams, MessageType.Information);
        else
          Dialogs.ShowMessage(starkov.Faker.ParametersMatchings.Resources.DialogErrorNoPropertyFormat(parameterRow.LocalizedAttachmentName), MessageType.Error);
        return;
      }
      #endregion
      
      #region Поля диалога
      var localizedValuesField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldLocalizedValue, false)
        .From(propInfo.Select(i => i.LocalizedName).OrderBy(i => i).ToArray());
      var propertyNameField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldAttachmentName, true)
        .From(propInfo.Select(i => i.Name).OrderBy(i => i).ToArray());
      var isLocalizedValues = dialog.AddBoolean(starkov.Faker.ParametersMatchings.Resources.DialogFieldUseLocalizedValue, false);
      var parameterField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldFillOption, true)
        .From(new string[] {
                Constants.Module.FillOptions.Common.NullValue,
                Constants.Module.FillOptions.Common.FixedValue,
                Constants.Module.FillOptions.Common.RandomValue
              });
      var personalValuesField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldValue, false, Sungero.Content.ElectronicDocuments.Null);
      
      var isUnique = propInfo.Select(i => i.LocalizedName).Count() == propInfo.Select(i => i.LocalizedName).Distinct().Count();
      isLocalizedValues.IsVisible = isUnique && parameterRow == null;
      parameterField.IsEnabled = parameterRow != null;
      personalValuesField.IsVisible = false;
      #endregion
      
      #region Обработчики свойств
      dialog.SetOnRefresh((arg) =>
                          {
                            if (!isUnique)
                              arg.AddInformation(starkov.Faker.ParametersMatchings.Resources.DialogInfoLocalizedPropertyNotUnique);
                          });
      
      propertyNameField.SetOnValueChanged((arg) =>
                                          {
                                            var isFilledVal = !string.IsNullOrEmpty(arg.NewValue);
                                            if (isFilledVal && arg.NewValue != arg.OldValue)
                                              localizedValuesField.Value = propInfo.FirstOrDefault(i => i.Name == arg.NewValue)?.LocalizedName;
                                            parameterField.IsEnabled = isFilledVal;
                                            personalValuesField.Value = null;
                                          });
      
      localizedValuesField.SetOnValueChanged((arg) =>
                                             {
                                               if (!string.IsNullOrEmpty(arg.NewValue) && arg.NewValue != arg.OldValue)
                                                 propertyNameField.Value = propInfo.FirstOrDefault(i => i.LocalizedName == arg.NewValue)?.Name;
                                             });
      
      isLocalizedValues.SetOnValueChanged((arg) =>
                                          {
                                            var isLocalazied = arg.NewValue.GetValueOrDefault();
                                            localizedValuesField.IsRequired = isLocalazied;
                                            localizedValuesField.IsEnabled = isLocalazied;
                                            propertyNameField.IsRequired = !isLocalazied;
                                            propertyNameField.IsEnabled = !isLocalazied;
                                            
                                            propertyNameField.Value = null;
                                            localizedValuesField.Value = null;
                                          });
      
      parameterField.SetOnValueChanged((arg) =>
                                       {
                                         var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyNameField.Value);
                                         if (selectedPropInfo == null)
                                           return;
                                         
                                         var isFixedVal = arg.NewValue == Constants.Module.FillOptions.Common.FixedValue;
                                         if (isFixedVal)
                                         {
                                           var documents = Functions.Module.Remote.GetEntitiesByTypeGuid(selectedPropInfo.PropertyGuid, string.Empty)
                                             .Where(e => Sungero.Content.ElectronicDocuments.Is(e))
                                             .Cast<Sungero.Content.IElectronicDocument>();
                                           personalValuesField.From(documents);
                                         }
                                         
                                         personalValuesField.IsVisible = isFixedVal;
                                         personalValuesField.IsRequired = isFixedVal;
                                         personalValuesField.Value = null;
                                       });
      #endregion
      
      #region Заполнение данных
      isLocalizedValues.Value = isLocalizedValues.IsVisible;
      
      if (parameterRow != null)
      {
        var selectedPropInfo = propInfo.FirstOrDefault();
        propertyNameField.Value = selectedPropInfo.Name;
        propertyNameField.IsEnabled = false;
        localizedValuesField.IsEnabled = false;
        
        if (!string.IsNullOrEmpty(parameterRow.FillOption))
        {
          parameterField.Value = parameterRow.FillOption;
          if (!string.IsNullOrEmpty(parameterRow.ChosenValue))
          {
            var id = Functions.Module.GetIdFromEntitiyName(parameterRow.ChosenValue);
            var selectedDoc = Sungero.Content.ElectronicDocuments.As(Functions.Module.Remote.GetEntitiesByTypeGuid(parameterRow.PropertyTypeGuid).FirstOrDefault(e => e.Id == id));
            personalValuesField.Value = selectedDoc;
          }
        }
      }
      
      if (!isUnique)
      {
        propertyNameField.IsRequired = true;
        localizedValuesField.IsEnabled = false;
      }
      #endregion
      
      #region Кнопки диалога
      if (dialog.Show() == DialogButtons.Ok)
      {
        var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyNameField.Value);
        
        var newRow = rowId.HasValue ? parameterRow : _obj.AttachmentParameters.AddNew();
        newRow.AttachmentName = selectedPropInfo.Name;
        newRow.LocalizedAttachmentName = selectedPropInfo.LocalizedName;
        newRow.PropertyType = Constants.Module.CustomType.Navigation;
        newRow.PropertyTypeGuid = selectedPropInfo.PropertyGuid;
        newRow.Limit = selectedPropInfo.LimitCount;
        newRow.FillOption = parameterField.Value;
        newRow.IsRequired = selectedPropInfo.IsRequired;
        newRow.ChosenValue = personalValuesField.Value != null ? 
          starkov.Faker.ParametersMatchings.Resources.EnityNameFormat(personalValuesField.Value.DisplayValue, personalValuesField.Value.Id) :
          null;
      }
      #endregion
    }
    
    #endregion
    
    #region Диалог для коллекции CollectionParameters
    
    /// <summary>
    /// Показ диалога для выбора данных.
    /// </summary>
    /// <param name="rowId">Номер строки.</param>
    /// <param name="isFillValue">Признак заполнения значений.</param>
    public void ShowDialogForSelectCollectionParameters(int? rowId, bool isFillValue)
    {
      var dialog = Dialogs.CreateInputDialog(starkov.Faker.ParametersMatchings.Resources.DialogDataInput);
      
      #region Данные для диалога
      var parameterRow = _obj.CollectionParameters.FirstOrDefault(p => p.Id == rowId.GetValueOrDefault());
      var selectedCollectionNames = _obj.CollectionParameters.Select(p => p.CollectionName);
      var propInfo = Functions.Module.Remote.GetCollectionPropertiesType(_obj.EntityType?.EntityTypeGuid ?? _obj.DocumentType?.DocumentTypeGuid)
        .Where(i => rowId.HasValue ? parameterRow.CollectionName == i.Name : !selectedCollectionNames.Contains(i.Name));
      
      if (!propInfo.Any())
      {
        if (parameterRow == null)
          Dialogs.ShowMessage(starkov.Faker.ParametersMatchings.Resources.DialogInfoNoAvailableParams, MessageType.Information);
        else
          Dialogs.ShowMessage(starkov.Faker.ParametersMatchings.Resources.DialogErrorNoPropertyFormat(parameterRow.LocalizedPropertyName), MessageType.Error);
        return;
      }
      #endregion
      
      #region Поля диалога
      var localizedCollectionField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldLocalizedCollectionValue, false)
        .From(propInfo.Select(i => i.LocalizedName).OrderBy(i => i).ToArray());
      var propertyCollectionField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldCollectionName, true)
        .From(propInfo.Select(i => i.Name).OrderBy(i => i).ToArray());
      var isLocalizedCollectionValues = dialog.AddBoolean(starkov.Faker.ParametersMatchings.Resources.DialogFieldUseLocalizedValue, false);
      
      var localizedValuesField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldLocalizedValue, false)
        .From(propInfo.SelectMany(i => i.Properties.Select(p => p.LocalizedName)).OrderBy(i => i).ToArray());
      var propertyNameField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldPropertyName, true)
        .From(propInfo.SelectMany(i => i.Properties.Select(p => p.Name)).OrderBy(i => i).ToArray());
      var parameterField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldFillOption, true);
      var personalValuesField = new List<object>();
      
      var isUnique = propInfo.Select(i => i.LocalizedName).Count() == propInfo.Select(i => i.LocalizedName).Distinct().Count();
      isLocalizedCollectionValues.IsVisible = isUnique && parameterRow == null;
      parameterField.IsEnabled = parameterRow != null;
      
      if (!rowId.HasValue)
      {
        localizedValuesField.IsVisible = false;
        propertyNameField.IsVisible = false;
        parameterField.IsVisible = false;
      }
      else
      {
        isLocalizedCollectionValues.IsVisible = false;
        localizedCollectionField.IsEnabled = false;
        propertyCollectionField.IsEnabled = false;
        propertyNameField.IsEnabled = false;
        localizedValuesField.IsEnabled = false;
      }
      #endregion
      
      #region Обработчики свойств
      dialog.SetOnRefresh((arg) =>
                          {
                            if (!isUnique)
                              arg.AddInformation(starkov.Faker.ParametersMatchings.Resources.DialogInfoLocalizedCollectionNotUnique);
                            
                            if (string.IsNullOrEmpty(propertyNameField.Value) || personalValuesField.Count != 2)
                              return;
                            
                            var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyCollectionField.Value).Properties.FirstOrDefault(p => p.Name == propertyNameField.Value);
                            var customType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
                            
                            var error = ValidateControls(customType, personalValuesField);
                            if (!string.IsNullOrEmpty(error))
                              arg.AddError(error);
                          });
      
      propertyNameField.SetOnValueChanged((arg) =>
                                          {
                                            HideDialogControl(ref personalValuesField);
                                            
                                            if (string.IsNullOrEmpty(arg.NewValue))
                                            {
                                              parameterField.From(Array.Empty<string>());
                                              parameterField.IsEnabled = false;
                                            }
                                            else if (arg.NewValue != arg.OldValue)
                                            {
                                              var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyCollectionField.Value).Properties.FirstOrDefault(p => p.Name == propertyNameField.Value);
                                              var parameters = new List<string>();
                                              if (selectedPropInfo != null)
                                                parameters = Functions.ParametersMatching.GetMatchingTypeToParameters(selectedPropInfo.Type) ?? parameters;
                                              
                                              parameterField.From(parameters.ToArray());
                                              parameterField.IsEnabled = true;
                                              
                                              localizedValuesField.Value = propInfo.FirstOrDefault(i => i.Name == propertyCollectionField.Value).Properties.FirstOrDefault(p => p.Name == propertyNameField.Value)?.LocalizedName;
                                            }
                                          });
      
      propertyCollectionField.SetOnValueChanged((arg) =>
                                                {
                                                  if (!string.IsNullOrEmpty(arg.NewValue) && arg.NewValue != arg.OldValue)
                                                    localizedCollectionField.Value = propInfo.FirstOrDefault(i => i.Name == arg.NewValue)?.LocalizedName;
                                                });
      
      localizedCollectionField.SetOnValueChanged((arg) =>
                                                 {
                                                   if (!string.IsNullOrEmpty(arg.NewValue) && arg.NewValue != arg.OldValue)
                                                     propertyCollectionField.Value = propInfo.FirstOrDefault(i => i.LocalizedName == arg.NewValue)?.Name;
                                                 });
      
      isLocalizedCollectionValues.SetOnValueChanged((arg) =>
                                                    {
                                                      var isLocalazied = arg.NewValue.GetValueOrDefault();
                                                      localizedCollectionField.IsRequired = isLocalazied;
                                                      localizedCollectionField.IsEnabled = isLocalazied;
                                                      propertyCollectionField.IsRequired = !isLocalazied;
                                                      propertyCollectionField.IsEnabled = !isLocalazied;
                                                      
                                                      propertyCollectionField.Value = null;
                                                      localizedCollectionField.Value = null;
                                                    });
      
      parameterField.SetOnValueChanged((arg) =>
                                       {
                                         HideDialogControl(ref personalValuesField);
                                         
                                         var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyCollectionField.Value).Properties.FirstOrDefault(p => p.Name == propertyNameField.Value);
                                         if (selectedPropInfo == null)
                                           return;
                                         
                                         ShowDialogControlsByParameter(dialog, arg.NewValue, selectedPropInfo, ref personalValuesField);
                                       });
      #endregion
      
      #region Заполнение данных
      isLocalizedCollectionValues.Value = isLocalizedCollectionValues.IsVisible;
      
      if (parameterRow != null)
      {
        propertyCollectionField.Value = propInfo.FirstOrDefault().Name;
        propertyNameField.Value = propInfo.FirstOrDefault().Properties.FirstOrDefault(p => p.Name == parameterRow.PropertyName).Name;
        
        if (!string.IsNullOrEmpty(parameterRow.FillOption))
        {
          parameterField.Value = parameterRow.FillOption;
          FillDialogControlFromTable(parameterRow, ref personalValuesField);
        }
      }
      
      if (!isUnique)
      {
        propertyNameField.IsRequired = true;
        localizedValuesField.IsEnabled = false;
      }
      #endregion
      
      #region Кнопки диалога
      if (dialog.Show() == DialogButtons.Ok)
      {
        var newRow = parameterRow;
        if (!rowId.HasValue)
        {
          foreach (var selectedPropInfo in propInfo.FirstOrDefault(i => i.Name == propertyCollectionField.Value).Properties)
          {
            newRow = _obj.CollectionParameters.AddNew();
            newRow.CollectionName = propertyCollectionField.Value;
            newRow.LocalizedCollectionName = localizedCollectionField.Value;
            newRow.PropertyName = selectedPropInfo.Name;
            newRow.LocalizedPropertyName = selectedPropInfo.LocalizedName;
            newRow.PropertyType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
            newRow.PropertyTypeGuid = selectedPropInfo.PropertyGuid;
            newRow.StringPropLength = selectedPropInfo.MaxStringLength;
          }
          return;
        }
        newRow.FillOption = parameterField.Value;
        newRow.ChosenValue = null;
        newRow.ValueFrom = null;
        newRow.ValueTo = null;
        
        if (personalValuesField.Count == 1)
        {
          newRow.ChosenValue = GetValueFromDialogControl(personalValuesField[0],
                                                         newRow.PropertyType,
                                                         newRow.PropertyTypeGuid);
        }
        else if (personalValuesField.Count == 2)
        {
          newRow.ValueFrom = GetValueFromDialogControl(personalValuesField[0],
                                                       newRow.PropertyType,
                                                       newRow.PropertyTypeGuid);
          newRow.ValueTo = GetValueFromDialogControl(personalValuesField[1],
                                                     newRow.PropertyType,
                                                     newRow.PropertyTypeGuid);
        }
      }
      #endregion
    }
    
    #endregion
    
    #region Диалог для коллекции Parameters
    
    /// <summary>
    /// Показ диалога для выбора данных.
    /// </summary>
    /// <param name="rowId">Номер строки.</param>
    public void ShowDialogForSelectParameters(int? rowId)
    {
      var dialog = Dialogs.CreateInputDialog(starkov.Faker.ParametersMatchings.Resources.DialogDataInput);
      
      #region Данные для диалога
      var parameterRow = _obj.Parameters.FirstOrDefault(p => p.Id == rowId.GetValueOrDefault());
      var selectedPropertyNames = _obj.Parameters.Select(p => p.PropertyName);
      var propInfo = Functions.Module.Remote.GetPropertiesType(_obj.EntityType?.EntityTypeGuid ?? _obj.DocumentType?.DocumentTypeGuid)
        .Where(i => rowId.HasValue ? parameterRow.PropertyName == i.Name : !selectedPropertyNames.Contains(i.Name));
      
      if (!propInfo.Any())
      {
        if (parameterRow == null)
          Dialogs.ShowMessage(starkov.Faker.ParametersMatchings.Resources.DialogInfoNoAvailableParams, MessageType.Information);
        else
          Dialogs.ShowMessage(starkov.Faker.ParametersMatchings.Resources.DialogErrorNoPropertyFormat(parameterRow.LocalizedPropertyName), MessageType.Error);
        return;
      }
      #endregion
      
      #region Поля диалога
      var localizedValuesField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldLocalizedValue, false)
        .From(propInfo.Select(i => i.LocalizedName).OrderBy(i => i).ToArray());
      var propertyNameField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldPropertyName, true)
        .From(propInfo.Select(i => i.Name).OrderBy(i => i).ToArray());
      var isLocalizedValues = dialog.AddBoolean(starkov.Faker.ParametersMatchings.Resources.DialogFieldUseLocalizedValue, false);
      var parameterField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldFillOption, true);
      var personalValuesField = new List<object>();
      
      var isUnique = propInfo.Select(i => i.LocalizedName).Count() == propInfo.Select(i => i.LocalizedName).Distinct().Count();
      isLocalizedValues.IsVisible = isUnique && parameterRow == null;
      parameterField.IsEnabled = parameterRow != null;
      #endregion
      
      #region Обработчики свойств
      dialog.SetOnRefresh((arg) =>
                          {
                            if (!isUnique)
                              arg.AddInformation(starkov.Faker.ParametersMatchings.Resources.DialogInfoLocalizedPropertyNotUnique);
                            
                            if (string.IsNullOrEmpty(propertyNameField.Value) || personalValuesField.Count != 2)
                              return;
                            
                            var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyNameField.Value);
                            var customType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
                            
                            var error = ValidateControls(customType, personalValuesField);
                            if (!string.IsNullOrEmpty(error))
                              arg.AddError(error);
                          });
      
      propertyNameField.SetOnValueChanged((arg) =>
                                          {
                                            HideDialogControl(ref personalValuesField);
                                            
                                            if (string.IsNullOrEmpty(arg.NewValue))
                                            {
                                              parameterField.From(Array.Empty<string>());
                                              parameterField.IsEnabled = false;
                                            }
                                            else if (arg.NewValue != arg.OldValue)
                                            {
                                              var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyNameField.Value);
                                              var parameters = new List<string>();
                                              if (selectedPropInfo != null)
                                                parameters = Functions.ParametersMatching.GetMatchingTypeToParameters(selectedPropInfo.Type) ?? parameters;
                                              
                                              parameterField.From(parameters.ToArray());
                                              parameterField.IsEnabled = true;
                                              
                                              localizedValuesField.Value = propInfo.FirstOrDefault(i => i.Name == arg.NewValue)?.LocalizedName;
                                            }
                                          });
      
      localizedValuesField.SetOnValueChanged((arg) =>
                                             {
                                               if (!string.IsNullOrEmpty(arg.NewValue) && arg.NewValue != arg.OldValue)
                                                 propertyNameField.Value = propInfo.FirstOrDefault(i => i.LocalizedName == arg.NewValue)?.Name;
                                             });
      
      isLocalizedValues.SetOnValueChanged((arg) =>
                                          {
                                            var isLocalazied = arg.NewValue.GetValueOrDefault();
                                            localizedValuesField.IsRequired = isLocalazied;
                                            localizedValuesField.IsEnabled = isLocalazied;
                                            propertyNameField.IsRequired = !isLocalazied;
                                            propertyNameField.IsEnabled = !isLocalazied;
                                            
                                            propertyNameField.Value = null;
                                            localizedValuesField.Value = null;
                                          });
      
      parameterField.SetOnValueChanged((arg) =>
                                       {
                                         HideDialogControl(ref personalValuesField);
                                         
                                         var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyNameField.Value);
                                         if (selectedPropInfo == null)
                                           return;
                                         
                                         ShowDialogControlsByParameter(dialog, arg.NewValue, selectedPropInfo, ref personalValuesField);
                                       });
      #endregion
      
      #region Заполнение данных
      isLocalizedValues.Value = isLocalizedValues.IsVisible;
      
      if (parameterRow != null)
      {
        var selectedPropInfo = propInfo.FirstOrDefault();
        propertyNameField.Value = selectedPropInfo.Name;
        propertyNameField.IsEnabled = false;
        localizedValuesField.IsEnabled = false;
        
        if (!string.IsNullOrEmpty(parameterRow.FillOption))
        {
          parameterField.Value = parameterRow.FillOption;
          FillDialogControlFromTable(parameterRow, ref personalValuesField);
        }
      }
      
      if (!isUnique)
      {
        propertyNameField.IsRequired = true;
        localizedValuesField.IsEnabled = false;
      }
      #endregion
      
      #region Кнопки диалога
      if (dialog.Show() == DialogButtons.Ok)
      {
        var selectedPropInfo = propInfo.FirstOrDefault(i => i.Name == propertyNameField.Value);
        
        var newRow = rowId.HasValue ? parameterRow : _obj.Parameters.AddNew();
        newRow.PropertyName = selectedPropInfo.Name;
        newRow.LocalizedPropertyName = selectedPropInfo.LocalizedName;
        newRow.PropertyType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
        newRow.PropertyTypeGuid = selectedPropInfo.PropertyGuid;
        newRow.StringPropLength = selectedPropInfo.MaxStringLength;
        newRow.FillOption = parameterField.Value;
        newRow.ChosenValue = null;
        newRow.ValueFrom = null;
        newRow.ValueTo = null;
        
        if (personalValuesField.Count == 1)
          newRow.ChosenValue = GetValueFromDialogControl(personalValuesField[0],
                                                         newRow.PropertyType,
                                                         newRow.PropertyTypeGuid);
        else if (personalValuesField.Count == 2)
        {
          newRow.ValueFrom = GetValueFromDialogControl(personalValuesField[0],
                                                       newRow.PropertyType,
                                                       newRow.PropertyTypeGuid);
          newRow.ValueTo = GetValueFromDialogControl(personalValuesField[1],
                                                     newRow.PropertyType,
                                                     newRow.PropertyTypeGuid);
        }
      }
      #endregion
    }
    
    #endregion

    #region Работа с контролами диалога
    
    /// <summary>
    /// Проверка контролов на валидность.
    /// </summary>
    /// <param name="customType">Обобщенный тип.</param>
    /// <param name="personalValuesField">Список контролов.</param>
    /// <returns>Если все корректно, то пустая строка, иначе строка с ошибками.</returns>
    public virtual string ValidateControls(string customType, List<object> personalValuesField)
    {
      var error = string.Empty;
      
      if (customType == Constants.Module.CustomType.Date &&
          Functions.Module.CastToDateDialogValue(personalValuesField[0])?.Value.GetValueOrDefault() >
          Functions.Module.CastToDateDialogValue(personalValuesField[1])?.Value.GetValueOrDefault(Calendar.SqlMaxValue))
        error = starkov.Faker.ParametersMatchings.Resources.DialogErrorDateFromGreaterDateTo;
      
      if (customType == Constants.Module.CustomType.Numeric &&
          Functions.Module.CastToIntegerDialogValue(personalValuesField[0])?.Value.GetValueOrDefault() >
          Functions.Module.CastToIntegerDialogValue(personalValuesField[1])?.Value.GetValueOrDefault(int.MaxValue))
        error = starkov.Faker.ParametersMatchings.Resources.DialogErrorValueFromGreaterValueTo;
      
      return error;
    }
    
    /// <summary>
    /// Проверка на необходимость выбора значения для варианта заполнения.
    /// </summary>
    /// <param name="selectedValue">Вариант заполнения.</param>
    public virtual bool IsNeedSelectValue(string fillOption)
    {
      var options = new List<string>() {
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.Date.Period,
        Constants.Module.FillOptions.Numeric.NumberWithLength,
        Constants.Module.FillOptions.Numeric.NumberRange,
        Constants.Module.FillOptions.String.FirstName,
        Constants.Module.FillOptions.String.LastName,
        Constants.Module.FillOptions.String.FullName
      };
      return options.Contains(fillOption);
    }
    
    /// <summary>
    /// Скрыть контролы диалога.
    /// </summary>
    /// <param name="controls">Контролы.</param>
    private void HideDialogControl(ref List<object> controls)
    {
      foreach (var control in controls)
      {
        var castedControl = Functions.Module.CastToDialogControl(control);
        if (castedControl == null)
          continue;
        
        castedControl.IsVisible = false;
        castedControl.IsRequired = false;
      }
      controls.Clear();
    }
    
    /// <summary>
    /// Вывод контролов в соответствии с вариантом заполнения.
    /// </summary>
    /// <param name="dialog">Диалог.</param>
    /// <param name="selectedValue">Выбранное значение.</param>
    /// <param name="selectedPropInfo">Структура с информацией о свойстве.</param>
    /// <param name="personalValuesField">Список контролов.</param>
    public virtual void ShowDialogControlsByParameter(CommonLibrary.IInputDialog dialog,
                                                      string selectedValue,
                                                      Faker.Structures.Module.PropertyInfo selectedPropInfo,
                                                      ref List<object> personalValuesField)
    {
      if (selectedValue == Constants.Module.FillOptions.Common.FixedValue)
      {
        var customType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
        if (customType == Constants.Module.CustomType.Date)
          personalValuesField.Add(dialog.AddDate(starkov.Faker.ParametersMatchings.Resources.DialogFieldDate, true));
        else if (customType == Constants.Module.CustomType.Bool)
          personalValuesField.Add(dialog.AddBoolean(starkov.Faker.ParametersMatchings.Resources.DialogFieldBooleanValue, true));
        else if (customType == Constants.Module.CustomType.Numeric)
          personalValuesField.Add(dialog.AddInteger(starkov.Faker.ParametersMatchings.Resources.DialogFieldNumber, true));
        else if (customType == Constants.Module.CustomType.String)
          personalValuesField.Add(dialog.AddString(starkov.Faker.ParametersMatchings.Resources.DialogFieldString, true));
        else if (customType == Constants.Module.CustomType.Enumeration)
          personalValuesField.Add(dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldEnumeration, true)
                                  .From(selectedPropInfo.EnumCollection.Select(i => i.LocalizedName).ToArray()));
        else if (customType == Constants.Module.CustomType.Navigation)
        {
          var guidType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(selectedPropInfo.PropertyGuid));
          var addSelectMethod = typeof(Sungero.Core.ExtensionInputDialog).GetMethods().FirstOrDefault(_ => _.Name == Constants.Module.Dialogs.AddSelect).MakeGenericMethod(guidType);
          var field = addSelectMethod.Invoke(null, new object[4] { dialog, starkov.Faker.ParametersMatchings.Resources.DialogFieldValue.ToString(), true, null });
          personalValuesField.Add(field);
        }
      }
      else if (selectedValue == Constants.Module.FillOptions.Date.Period)
      {
        personalValuesField.AddRange(new List<IDateDialogValue>() {
                                       dialog.AddDate(starkov.Faker.ParametersMatchings.Resources.DialogFieldDateFrom, true),
                                       dialog.AddDate(starkov.Faker.ParametersMatchings.Resources.DialogFieldDateTo, true)
                                     });
      }
      else if (selectedValue == Constants.Module.FillOptions.Numeric.NumberWithLength)
      {
        personalValuesField.AddRange(new List<IIntegerDialogValue>() {
                                       dialog.AddInteger(starkov.Faker.ParametersMatchings.Resources.DialogFieldNumberLength, true)
                                     });
      }
      else if (selectedValue == Constants.Module.FillOptions.Numeric.NumberRange)
      {
        personalValuesField.AddRange(new List<IIntegerDialogValue>() {
                                       dialog.AddInteger(starkov.Faker.ParametersMatchings.Resources.DialogFieldFrom, true),
                                       dialog.AddInteger(starkov.Faker.ParametersMatchings.Resources.DialogFieldBy, true)
                                     });
      }
      else if (selectedValue == Constants.Module.FillOptions.String.FirstName ||
               selectedValue == Constants.Module.FillOptions.String.LastName ||
               selectedValue == Constants.Module.FillOptions.String.FullName)
      {
        personalValuesField.AddRange(new List<IDropDownDialogValue>() {
                                       dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldSex, false)
                                         .From(Enum.GetNames(typeof(Bogus.DataSets.Name.Gender)))
                                     });
      }
    }
    
    /// <summary>
    /// Заполнить генерируемые контролы диалога значениями из таблицы параметров.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <param name="controls">Контролы.</param>
    public virtual void FillDialogControlFromTable(Faker.IParametersMatchingParameters parameterRow, ref List<object> controls)
    {
      var parameterStruct = Structures.Module.ParameterInfo.Create(parameterRow.ParametersMatching,
                                                                   parameterRow.PropertyName,
                                                                   parameterRow.PropertyTypeGuid,
                                                                   parameterRow.PropertyType,
                                                                   parameterRow.FillOption,
                                                                   parameterRow.ChosenValue,
                                                                   parameterRow.ValueFrom,
                                                                   parameterRow.ValueTo);
      FillDialogControlFromTable(parameterStruct, ref controls);
    }
    
    /// <summary>
    /// Заполнить генерируемые контролы диалога значениями из таблицы коллекций.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <param name="controls">Контролы.</param>
    public virtual void FillDialogControlFromTable(Faker.IParametersMatchingCollectionParameters parameterRow, ref List<object> controls)
    {
      var parameterStruct = Structures.Module.ParameterInfo.Create(parameterRow.ParametersMatching,
                                                                   parameterRow.PropertyName,
                                                                   parameterRow.PropertyTypeGuid,
                                                                   parameterRow.PropertyType,
                                                                   parameterRow.FillOption,
                                                                   parameterRow.ChosenValue,
                                                                   parameterRow.ValueFrom,
                                                                   parameterRow.ValueTo);
      FillDialogControlFromTable(parameterStruct, ref controls);
    }
    
    /// <summary>
    /// Заполнить генерируемые контролы диалога значениями из структуры.
    /// </summary>
    /// <param name="parameterRow">Структура с параметрами.</param>
    /// <param name="controls">Контролы.</param>
    public virtual void FillDialogControlFromTable(Faker.Structures.Module.ParameterInfo parameterRow, ref List<object> controls)
    {
      if (!string.IsNullOrEmpty(parameterRow.ChosenValue))
      {
        var controlType = Functions.ParametersMatching.GetMatchingControlTypeToCustomType(parameterRow.PropertyType, controls[0]);
        controls[0].GetType().GetProperty(Constants.Module.PropertyNames.Value).SetValue(controls[0], GetValueInSelectedType(controlType, parameterRow.ChosenValue, parameterRow.PropertyTypeGuid));
      }
      else if (!string.IsNullOrEmpty(parameterRow.ValueFrom) && !string.IsNullOrEmpty(parameterRow.ValueTo))
      {
        var controlType = Functions.ParametersMatching.GetMatchingControlTypeToCustomType(parameterRow.PropertyType, controls[0]);
        controls[0].GetType().GetProperty(Constants.Module.PropertyNames.Value).SetValue(controls[0], GetValueInSelectedType(controlType, parameterRow.ValueFrom, parameterRow.PropertyTypeGuid));
        controls[1].GetType().GetProperty(Constants.Module.PropertyNames.Value).SetValue(controls[1], GetValueInSelectedType(controlType, parameterRow.ValueTo, parameterRow.PropertyTypeGuid));
      }
    }
    
    /// <summary>
    /// Получить значение в указанном типе.
    /// </summary>
    /// <param name="customType">Обобщенный тип.</param>
    /// <param name="convertedValue">Значение которое нужно преобразовать.</param>
    /// <param name="propertyTypeGuid">Guid типа свойства.</param>
    /// <returns>Преобразованное значение.</returns>
    public virtual object GetValueInSelectedType(string customType, string convertedValue, string propertyTypeGuid)
    {
      DateTime date;
      bool logic;
      int num;
      object result = null;
      
      if (customType == Constants.Module.CustomType.Date && Calendar.TryParseDate(convertedValue, out date))
        result = date;
      else if (customType == Constants.Module.CustomType.Bool && bool.TryParse(convertedValue, out logic))
        result = logic;
      else if (customType == Constants.Module.CustomType.Numeric && int.TryParse(convertedValue, out num))
        result = num;
      else if (customType == Constants.Module.CustomType.Navigation)
      {
        var id = Functions.Module.GetIdFromEntitiyName(convertedValue);
        result = Functions.Module.Remote.GetEntitiesByTypeGuid(propertyTypeGuid).FirstOrDefault(e => e.Id == id);
      }
      else
        result = convertedValue;
      
      return result;
    }
    
    /// <summary>
    /// Получить значение из контрола диалога в виде строки.
    /// </summary>
    /// <param name="control">Контрол.</param>
    /// <param name="customType">Обобщенный тип.</param>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <returns>Значение из контрола в виде строки.</returns>
    public virtual string GetValueFromDialogControl(object control, string customType, string typeGuid)
    {
      var result = string.Empty;
      if (control == null)
        return result;
      
      if (customType == Constants.Module.CustomType.Enumeration)
        result = Functions.Module.CastToDialogControlString(control)?.Value;
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.DateDialogControl)))
        result = Functions.Module.CastToDateDialogValue(control)?.Value.GetValueOrDefault().ToShortDateString();
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.BooleanDialogControl)))
        result = Functions.Module.CastToBooleanDialogValue(control)?.Value.GetValueOrDefault().ToString();
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.IntegerDialogControl)))
        result = Functions.Module.CastToIntegerDialogValue(control)?.Value.GetValueOrDefault().ToString();
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.StringDialogControl)))
        result = Functions.Module.CastToStringDialogValue(control)?.Value;
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.DropDownDialogControl)))
        result = Functions.Module.CastToDropDownDialogValue(control)?.Value;
      else if (customType == Constants.Module.CustomType.Navigation)
      {
        var entValue = Functions.Module.CastToEntity(control.GetType().GetProperty(Constants.Module.PropertyNames.Value).GetValue(control));
        if (entValue != null)
          result = starkov.Faker.ParametersMatchings.Resources.EnityNameFormat(entValue.DisplayValue, entValue.Id);
      }
      
      return result ?? string.Empty;
    }
    
    /// <summary>
    /// Получить обобщенный тип по типу контрола.
    /// </summary>
    /// <param name="customType">Обобщенное наименование типа свойства.</param>
    /// <param name="control">Контрол.</param>
    /// <returns>Обобщенное наименование типа.</returns>
    public static string GetMatchingControlTypeToCustomType(string customType, object control)
    {
      if (customType == Constants.Module.CustomType.Enumeration || customType == Constants.Module.CustomType.Navigation)
        return customType;
      
      if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.DateDialogControl)))
        customType = Constants.Module.CustomType.Date;
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.BooleanDialogControl)))
        customType = Constants.Module.CustomType.Bool;
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.IntegerDialogControl)))
        customType = Constants.Module.CustomType.Numeric;
      else if (Functions.Module.CompareObjectWithType(control, typeof(Sungero.WebAPI.Dialogs.StringDialogControl)))
        customType = Constants.Module.CustomType.String;
      
      return customType;
    }
    
    #endregion
  }
}