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
      var dialog = Dialogs.CreateInputDialog("Изменение данных");
      
      #region Поля диалога
      var propertyNameField = dialog.AddSelect("Наименование свойства", true).From(_obj.Parameters.Select(_ => _.PropertyName).ToArray());
      var localizedValuesField = dialog.AddString("Локализованное значение", false);
      
      localizedValuesField.IsEnabled = false;
      #endregion
      
      #region Обработчики свойств
      propertyNameField.SetOnValueChanged((arg) =>
                                          {
                                            if (!string.IsNullOrEmpty(arg.NewValue))
                                              localizedValuesField.Value = _obj.Parameters.FirstOrDefault(_ => _.PropertyName == arg.NewValue)?.LocalizedPropertyName;
                                          });
      #endregion
      
      #region Кнопки диалога
      var changeBtn = dialog.Buttons.AddCustom("Изменить");
      dialog.Buttons.AddCancel();
      
      if (dialog.Show() == changeBtn)
        ShowDialogForSelectParameters(_obj.Parameters.FirstOrDefault(_ => _.PropertyName == propertyNameField.Value)?.Id);
      #endregion
    }

    /// <summary>
    /// Показ диалога для выбора данных
    /// </summary>
    /// <param name="rowId">Номер строки</param>
    public void ShowDialogForSelectParameters(int? rowId)
    {
      var dialog = Dialogs.CreateInputDialog("Ввод данных");
      
      #region Данные для диалога
      var parameterRow = _obj.Parameters.FirstOrDefault(_ => _.Id == rowId.GetValueOrDefault());
      var propInfo = Functions.Module.Remote.GetPropertiesType(_obj.DatabookType?.DatabookTypeGuid ?? _obj.DocumentType?.DocumentTypeGuid)
        .Where(_ => rowId.HasValue ?
               parameterRow.PropertyName == _.Name :
               !_obj.Parameters.Select(p => p.PropertyName).Contains(_.Name));
      #endregion
      
      #region Поля диалога
      var propertyNameField = dialog.AddSelect("Наименование свойства", true).From(propInfo.Select(_ => _.Name).ToArray());
      var isLocalizedValues = dialog.AddBoolean("Использовать локализованное значение", false);
      var parameterField = dialog.AddSelect("Вариант заполнения", true);
      var personalValuesField = new List<object>();
      
      var isUnique = propInfo.Select(_ => _.LocalizedName).Count() == propInfo.Select(_ => _.LocalizedName).Distinct().Count();
      isLocalizedValues.IsVisible = isUnique && parameterRow == null;
      parameterField.IsEnabled = parameterRow != null;
      
      if (parameterRow != null)
      {
        var selectedPropInfo = propInfo.FirstOrDefault();
        propertyNameField.Value = selectedPropInfo.Name;
        propertyNameField.IsEnabled = false;
        
        var parameters = Functions.ParametersMatching.GetMatchingTypeToParameters(selectedPropInfo.Type);
        parameterField.From(parameters.ToArray());
        
        if (!string.IsNullOrEmpty(parameterRow.FillOption))
        {
          parameterField.Value = parameterRow.FillOption;
          ShowDialogControlsByParameter(dialog, parameterField.Value, selectedPropInfo, ref personalValuesField);
          FillDialogControlFromTable(parameterRow, ref personalValuesField);
        }
      }
      #endregion
      
      #region Обработчики свойств
      dialog.SetOnRefresh((arg) =>
                          {
                            if (!isUnique)
                              arg.AddInformation("Локализованные значения свойств не уникальны");
                            
                            if (string.IsNullOrEmpty(propertyNameField.Value) || personalValuesField.Count != 2)
                              return;
                            
                            var selectedPropInfo = propInfo.FirstOrDefault(_ => isLocalizedValues.Value.GetValueOrDefault() ?
                                                                           _.LocalizedName == propertyNameField.Value :
                                                                           _.Name == propertyNameField.Value);
                            var customType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
                            
                            if (customType == Constants.Module.CustomType.Date &&
                                (personalValuesField[0] as IDateDialogValue).Value.GetValueOrDefault() > (personalValuesField[1] as IDateDialogValue).Value.GetValueOrDefault(Calendar.SqlMaxValue))
                              arg.AddError("Дата \"С\" не может быть больше чем дата \"По\"");
                            else if (customType == Constants.Module.CustomType.Numeric &&
                                     (personalValuesField[0] as IIntegerDialogValue).Value.GetValueOrDefault() > (personalValuesField[1] as IIntegerDialogValue).Value.GetValueOrDefault(int.MaxValue))
                              arg.AddError("Значение \"С\" не может быть больше чем значение \"По\"");
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
                                              propertyNameField.From(propInfo.Select(_ => _.LocalizedName).ToArray());
                                            else
                                              propertyNameField.From(propInfo.Select(_ => _.Name).ToArray());
                                            
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
    private void ShowDialogControlsByParameter(IInputDialog dialog,
                                               string selectedValue,
                                               Faker.Structures.Module.PropertyInfo selectedPropInfo,
                                               ref List<object> personalValuesField)
    {
      if (selectedValue == Constants.Module.Common.FixedValue)
      {
        var customType = Functions.ParametersMatching.GetMatchingTypeToCustomType(selectedPropInfo.Type);
        if (customType == Constants.Module.CustomType.Date)
          personalValuesField.Add(dialog.AddDate("Дата", true));
        else if (customType == Constants.Module.CustomType.Bool)
          personalValuesField.Add(dialog.AddBoolean("Логическое значение", true));
        else if (customType == Constants.Module.CustomType.Numeric)
          personalValuesField.Add(dialog.AddInteger("Число", true));
        else if (customType == Constants.Module.CustomType.String)
          personalValuesField.Add(dialog.AddString("Строка", true));
        else if (customType == Constants.Module.CustomType.Enumeration)
          personalValuesField.Add(dialog.AddSelect("Перечисление", true)
                                  .From(selectedPropInfo.EnumCollection.ToArray()));
        else
          personalValuesField.Add(dialog.AddSelect("Значение", true)
                                  .From(Functions.Module.GetEntitiyNamesByType(selectedPropInfo.PropertyGuid,
                                                                               _obj.DocumentType?.DocumentTypeGuid).ToArray()));
      }
      else if (selectedValue == Constants.Module.Date.Period)
      {
        personalValuesField.AddRange(new List<IDateDialogValue>() {
                                       dialog.AddDate("Дата с", true),
                                       dialog.AddDate("Дата по", true)
                                     });
      }
      else if (selectedValue == Constants.Module.Numeric.NumberWithLength)
      {
        personalValuesField.AddRange(new List<IIntegerDialogValue>() {
                                       dialog.AddInteger("Длина числа", true)
                                     });
      }
      else if (selectedValue == Constants.Module.Numeric.NumberRange)
      {
        personalValuesField.AddRange(new List<IIntegerDialogValue>() {
                                       dialog.AddInteger("С", true),
                                       dialog.AddInteger("По", true)
                                     });
      }
      else if (selectedValue == Constants.Module.String.FirstName || selectedValue == Constants.Module.String.LastName || selectedValue == Constants.Module.String.FullName)
      {
        personalValuesField.AddRange(new List<IDropDownDialogValue>() {
                                       dialog.AddSelect("Пол", false).From(Enum.GetNames(typeof(Bogus.DataSets.Name.Gender)))
                                     });
      }
    }
    
    /// <summary>
    /// Заполнить генерируемые контролы диалога значениями из таблицы
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <param name="controls">Контролы</param>
    private void FillDialogControlFromTable(Faker.IParametersMatchingParameters parameterRow, ref List<object> controls)
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
    private object GetValueInSelectedType(string customType, string typeGuid, string convertedValue)
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
        var strWithId = string.Format(", Id: ({0})", convertedValue);
        result = Functions.Module.GetEntitiyNamesByType(typeGuid, _obj.DocumentType?.DocumentTypeGuid).FirstOrDefault(_ => _.Contains(strWithId));
      }
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
    private string GetValueFromDialogControl(object control, string customType, string typeGuid)
    {
      var result = string.Empty;
      if (control == null)
        return result;
      
      if (customType == Constants.Module.CustomType.Enumeration)
        result = (control as IDialogControl<string>).Value;
      else if (customType == Constants.Module.CustomType.Navigation)
      {
        var dialogValue = (control as IDialogControl<string>).Value;
        var regex = new Regex(@"\(\d*\)$");
        var matches = regex.Matches(dialogValue);
        if (matches.Count > 0)
        {
          foreach (Match match in matches)
            result = match.Value.Substring(1, match.Value.Length-2);
        }
      }
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