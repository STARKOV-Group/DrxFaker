using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;
using System.Text.RegularExpressions;
using CommonLibrary;

namespace starkov.Faker.Client
{
  partial class ParametersMatchingFunctions
  {
    
    /// <summary>
    /// Показ диалога для изменения данных в табличной части
    /// </summary>
    public void ShowDialogForChangeParameters()
    {
      var dialog = Dialogs.CreateInputDialog(starkov.Faker.ParametersMatchings.Resources.DialogChangeData);
      
      #region Данные для диалога
      var propInfo = Functions.Module.Remote.GetPropertiesType(_obj.DatabookType?.DatabookTypeGuid ?? _obj.DocumentType?.DocumentTypeGuid);
      #endregion
      
      #region Поля диалога
      var localizedValuesField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldLocalizedValue, false)
        .From(_obj.Parameters.Select(_ => _.LocalizedPropertyName).OrderBy(_ => _).ToArray());
      var propertyNameField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldPropertyName, false)
        .From(_obj.Parameters.Select(_ => _.PropertyName).OrderBy(_ => _).ToArray());
      
      var isUnique = propInfo.Select(_ => _.LocalizedName).Count() == propInfo.Select(_ => _.LocalizedName).Distinct().Count();
      if (isUnique)
      {
        localizedValuesField.IsRequired = true;
        propertyNameField.IsEnabled = false;
      }
      else
      {
        propertyNameField.IsRequired = true;
        localizedValuesField.IsEnabled = false;
      }
      #endregion
      
      #region Обработчики свойств
      dialog.SetOnRefresh((arg) =>
                          {
                            if (!isUnique)
                              arg.AddInformation(starkov.Faker.ParametersMatchings.Resources.DialogInfoLocalizedPropertyNotUnique);
                          });
      
      propertyNameField.SetOnValueChanged((arg) =>
                                          {
                                            if (!string.IsNullOrEmpty(arg.NewValue))
                                              localizedValuesField.Value = _obj.Parameters.FirstOrDefault(_ => _.PropertyName == arg.NewValue)?.LocalizedPropertyName;
                                          });
      
      localizedValuesField.SetOnValueChanged((arg) =>
                                             {
                                               if (!string.IsNullOrEmpty(arg.NewValue))
                                                 propertyNameField.Value = _obj.Parameters.FirstOrDefault(_ => _.LocalizedPropertyName == arg.NewValue)?.PropertyName;
                                             });
      #endregion
      
      #region Кнопки диалога
      var changeBtn = dialog.Buttons.AddCustom(starkov.Faker.ParametersMatchings.Resources.DialogButtonChange);
      dialog.Buttons.AddCancel();
      
      if (dialog.Show() == changeBtn)
        ShowDialogForSelectParameters(_obj.Parameters.FirstOrDefault(_ => isUnique ?
                                                                     _.LocalizedPropertyName == localizedValuesField.Value :
                                                                     _.PropertyName == propertyNameField.Value)?.Id);
      #endregion
    }

    /// <summary>
    /// Показ диалога для выбора данных
    /// </summary>
    /// <param name="rowId">Номер строки</param>
    public void ShowDialogForSelectParameters(int? rowId)
    {
      var dialog = Dialogs.CreateInputDialog(starkov.Faker.ParametersMatchings.Resources.DialogDataInput);
      
      #region Данные для диалога
      var parameterRow = _obj.Parameters.FirstOrDefault(_ => _.Id == rowId.GetValueOrDefault());
      var propInfo = Functions.Module.Remote.GetPropertiesType(_obj.DatabookType?.DatabookTypeGuid ?? _obj.DocumentType?.DocumentTypeGuid)
        .Where(_ => rowId.HasValue ?
               parameterRow.PropertyName == _.Name :
               !_obj.Parameters.Select(p => p.PropertyName).Contains(_.Name));
      #endregion
      
      #region Поля диалога
      var propertyNameField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldPropertyName, true).From(propInfo.Select(_ => _.Name).OrderBy(_ => _).ToArray());
      var isLocalizedValues = dialog.AddBoolean(starkov.Faker.ParametersMatchings.Resources.DialogFieldUseLocalizedValue, false);
      var parameterField = dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldFillOption, true);
      var personalValuesField = new List<object>();
      
      var isUnique = propInfo.Select(_ => _.LocalizedName).Count() == propInfo.Select(_ => _.LocalizedName).Distinct().Count();
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
                            
                            var selectedPropInfo = propInfo.FirstOrDefault(_ => isLocalizedValues.Value.GetValueOrDefault() ?
                                                                           _.LocalizedName == propertyNameField.Value :
                                                                           _.Name == propertyNameField.Value);
                            var customType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
                            
                            if (customType == Constants.Module.CustomType.Date &&
                                (personalValuesField[0] as IDateDialogValue).Value.GetValueOrDefault() > (personalValuesField[1] as IDateDialogValue).Value.GetValueOrDefault(Calendar.SqlMaxValue))
                              arg.AddError(starkov.Faker.ParametersMatchings.Resources.DialogErrorDateFromGreaterDateTo);
                            else if (customType == Constants.Module.CustomType.Numeric &&
                                     (personalValuesField[0] as IIntegerDialogValue).Value.GetValueOrDefault() > (personalValuesField[1] as IIntegerDialogValue).Value.GetValueOrDefault(int.MaxValue))
                              arg.AddError(starkov.Faker.ParametersMatchings.Resources.DialogErrorValueFromGreaterValueTo);
                          });
      
      propertyNameField.SetOnValueChanged((arg) =>
                                          {
                                            HideDialogControl(ref personalValuesField);
                                            
                                            if (string.IsNullOrEmpty(arg.NewValue))
                                            {
                                              parameterField.From(Array.Empty<string>());
                                              parameterField.IsEnabled = false;
                                            }
                                            else
                                            {
                                              var selectedPropInfo = propInfo.FirstOrDefault(_ => isLocalizedValues.Value.GetValueOrDefault() ?
                                                                                             _.LocalizedName == propertyNameField.Value :
                                                                                             _.Name == propertyNameField.Value);
                                              
                                              var parameters = new List<string>();
                                              if (selectedPropInfo != null)
                                                parameters = Functions.ParametersMatching.GetMatchingTypeToParameters(selectedPropInfo.Type) ?? parameters;
                                              parameterField.From(parameters.ToArray());
                                              parameterField.IsEnabled = true;
                                            }
                                          });
      
      isLocalizedValues.SetOnValueChanged((arg) =>
                                          {
                                            if (arg.NewValue.GetValueOrDefault())
                                              propertyNameField.From(propInfo.Select(_ => _.LocalizedName).OrderBy(_ => _).ToArray());
                                            else
                                              propertyNameField.From(propInfo.Select(_ => _.Name).OrderBy(_ => _).ToArray());
                                            
                                            parameterField.Value = null;
                                          });
      
      parameterField.SetOnValueChanged((arg) =>
                                       {
                                         HideDialogControl(ref personalValuesField);
                                         
                                         var selectedPropInfo = propInfo.FirstOrDefault(_ => isLocalizedValues.Value.GetValueOrDefault() ?
                                                                                        _.LocalizedName == propertyNameField.Value :
                                                                                        _.Name == propertyNameField.Value);
                                         if (selectedPropInfo == null)
                                           return;
                                         
                                         ShowDialogControlsByParameter(dialog, arg.NewValue, selectedPropInfo, ref personalValuesField);
                                       });
      #endregion
      
      #region Заполнение данных
      if (isLocalizedValues.Value != isLocalizedValues.IsVisible)
        isLocalizedValues.Value = isLocalizedValues.IsVisible;
      
      if (parameterRow != null)
      {
        var selectedPropInfo = propInfo.FirstOrDefault();
        propertyNameField.Value = selectedPropInfo.Name;
        propertyNameField.IsEnabled = false;
        
        if (!string.IsNullOrEmpty(parameterRow.FillOption))
        {
          parameterField.Value = parameterRow.FillOption;
          FillDialogControlFromTable(parameterRow, ref personalValuesField);
        }
      }
      #endregion
      
      #region Кнопки диалога
      if (dialog.Show() == DialogButtons.Ok)
      {
        var selectedPropInfo = propInfo.FirstOrDefault(_ => isLocalizedValues.Value.GetValueOrDefault() ?
                                                       _.LocalizedName == propertyNameField.Value :
                                                       _.Name == propertyNameField.Value);
        
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

    #region Работа с контролами диалога
    
    /// <summary>
    /// Скрыть контролы диалога
    /// </summary>
    /// <param name="controls">Контролы</param>
    private void HideDialogControl(ref List<object> controls)
    {
      foreach (var control in controls)
      {
        (control as IDialogControl).IsVisible = false;
        (control as IDialogControl).IsRequired = false;
      }
      controls.Clear();
    }
    
    /// <summary>
    /// Вывод контролов в соответствии с вариантом заполнения
    /// </summary>
    /// <param name="dialog">Диалог</param>
    /// <param name="selectedValue">Выбранное значение</param>
    /// <param name="selectedPropInfo">Структура с информацией о свойстве</param>
    /// <param name="personalValuesField">Список контролов</param>
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
                                  .From(selectedPropInfo.EnumCollection.Select(_ => _.LocalizedName).ToArray()));
        else
          personalValuesField.Add(dialog.AddSelect(starkov.Faker.ParametersMatchings.Resources.DialogFieldValue, true)
                                  .From(Functions.Module.GetEntitiyNamesByType(selectedPropInfo.PropertyGuid,
                                                                               _obj.DocumentType?.DocumentTypeGuid).ToArray()));
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
    /// Заполнить генерируемые контролы диалога значениями из таблицы
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <param name="controls">Контролы</param>
    public virtual void FillDialogControlFromTable(Faker.IParametersMatchingParameters parameterRow, ref List<object> controls)
    {
      if (!string.IsNullOrEmpty(parameterRow.ChosenValue))
      {
        var controlType = Functions.ParametersMatching.GetMatchingControlTypeToCustomType(parameterRow.PropertyType, controls[0]);
        controls[0].GetType().GetProperty("Value").SetValue(controls[0], GetValueInSelectedType(controlType, parameterRow.PropertyTypeGuid, parameterRow.ChosenValue));
      }
      else if (!string.IsNullOrEmpty(parameterRow.ValueFrom) && !string.IsNullOrEmpty(parameterRow.ValueTo))
      {
        var controlType = Functions.ParametersMatching.GetMatchingControlTypeToCustomType(parameterRow.PropertyType, controls[0]);
        controls[0].GetType().GetProperty("Value").SetValue(controls[0], GetValueInSelectedType(controlType, parameterRow.PropertyTypeGuid, parameterRow.ValueFrom));
        controls[1].GetType().GetProperty("Value").SetValue(controls[1], GetValueInSelectedType(controlType, parameterRow.PropertyTypeGuid, parameterRow.ValueTo));
      }
    }
    
    /// <summary>
    /// Получить значение в указанном типе
    /// </summary>
    /// <param name="customType">Обобщенный тип</param>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <param name="convertedValue">Значение которое нужно преобразовать</param>
    /// <returns>Преобразованное значение</returns>
    public virtual object GetValueInSelectedType(string customType, string typeGuid, string convertedValue)
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
        result = Functions.Module.GetEntitiyNamesByType(typeGuid, _obj.DocumentType?.DocumentTypeGuid).FirstOrDefault(_ => _ == convertedValue);
      else
        result = convertedValue;
      
      return result;
    }
    
    /// <summary>
    /// Получить значение из контрола диалога в виде строки
    /// </summary>
    /// <param name="control">Контрол</param>
    /// <param name="customType">Обобщенный тип</param>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <returns>Значение из контрола в виде строки</returns>
    public virtual string GetValueFromDialogControl(object control, string customType, string typeGuid)
    {
      var result = string.Empty;
      if (control == null)
        return result;
      
      if (customType == Constants.Module.CustomType.Enumeration || customType == Constants.Module.CustomType.Navigation)
        result = (control as IDialogControl<string>).Value;
      else if (control is IDateDialogValue)
        result = (control as IDateDialogValue).Value.GetValueOrDefault().ToShortDateString();
      else if (control is IBooleanDialogValue)
        result = (control as IBooleanDialogValue).Value.GetValueOrDefault().ToString();
      else if (control is IIntegerDialogValue)
        result = (control as IIntegerDialogValue).Value.GetValueOrDefault().ToString();
      else if (control is IStringDialogValue)
        result = (control as IStringDialogValue).Value;
      else if (control is IDropDownDialogValue)
        result = (control as IDropDownDialogValue).Value;
      
      return result;
    }
    
    #endregion
  }
}