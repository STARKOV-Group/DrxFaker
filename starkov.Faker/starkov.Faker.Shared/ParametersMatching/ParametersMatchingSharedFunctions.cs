using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using starkov.Faker.ParametersMatching;

namespace starkov.Faker.Shared
{
  partial class ParametersMatchingFunctions
  {

    /// <summary>
    /// Получить варианты заполнения по указанному типу
    /// </summary>
    /// <param name="type">Обобщенное наименование типа</param>
    /// <returns>Варианты выбора для заполнения</returns>
    public static List<string> GetMatchingTypeToParameters(string type)
    {
      type = GetMatchingTypeToCustomType(type);
      #region Словарь
      var dict = new Dictionary<string, List<string>>()
      {
        {
          Constants.Module.CustomType.Date, new List<string>()
          {
            Constants.Module.Common.NullValue,
            Constants.Module.Common.FixedValue,
            Constants.Module.Date.Period
          }
        },
        {
          Constants.Module.CustomType.Bool, new List<string>()
          {
            Constants.Module.Common.NullValue,
            Constants.Module.Common.FixedValue,
            Constants.Module.Common.RandomValue
          }
        },
        {
          Constants.Module.CustomType.Numeric, new List<string>()
          {
            Constants.Module.Common.NullValue,
            Constants.Module.Common.FixedValue,
            Constants.Module.Numeric.NumberRange,
            Constants.Module.Numeric.NumberWithLength
          }
        },
        {
          Constants.Module.CustomType.String, new List<string>()
          {
            Constants.Module.Common.NullValue,
            Constants.Module.Common.FixedValue,
            Constants.Module.String.RandomString,
            Constants.Module.String.Paragraph,
            Constants.Module.String.RandomPhone,
            Constants.Module.String.NumberInStr,
            Constants.Module.String.FirstName,
            Constants.Module.String.LastName,
            Constants.Module.String.FullName,
            Constants.Module.String.JobTitle,
            Constants.Module.String.Email,
            Constants.Module.String.Login,
            Constants.Module.String.State,
            Constants.Module.String.City,
            Constants.Module.String.Street,
            Constants.Module.String.Department,
            Constants.Module.String.CompanyName
          }
        },
        {
          Constants.Module.CustomType.Enumeration, new List<string>()
          {
            Constants.Module.Common.NullValue,
            Constants.Module.Common.FixedValue,
            Constants.Module.Common.RandomValue
          }
        },
        {
          Constants.Module.CustomType.Navigation, new List<string>()
          {
            Constants.Module.Common.NullValue,
            Constants.Module.Common.FixedValue,
            Constants.Module.Common.RandomValue
          }
        }
      };
      #endregion
      
      var list = new List<string>();
      dict.TryGetValue(type, out list);
      return list;
    }

    /// <summary>
    /// Получить обобщенный тип по типу свойства
    /// </summary>
    /// <param name="type">Наименование типа свойства</param>
    /// <returns>Обобщенное наименование типа</returns>
    public static string GetMatchingTypeToCustomType(string type)
    {
      var dict = new Dictionary<string, string>()
      {
        { Sungero.Metadata.PropertyType.Data.ToString(), Constants.Module.CustomType.Date },
        { Sungero.Metadata.PropertyType.DateTime.ToString(), Constants.Module.CustomType.Date },
        { Sungero.Metadata.PropertyType.Boolean.ToString(), Constants.Module.CustomType.Bool },
        { Sungero.Metadata.PropertyType.Double.ToString(), Constants.Module.CustomType.Numeric },
        { Sungero.Metadata.PropertyType.Integer.ToString(), Constants.Module.CustomType.Numeric },
        { Sungero.Metadata.PropertyType.String.ToString(), Constants.Module.CustomType.String },
        { Sungero.Metadata.PropertyType.Text.ToString(), Constants.Module.CustomType.String },
        { Sungero.Metadata.PropertyType.Enumeration.ToString(), Constants.Module.CustomType.Enumeration },
        { Sungero.Metadata.PropertyType.Navigation.ToString(), Constants.Module.CustomType.Navigation }
      };
      
      var customType = string.Empty;
      dict.TryGetValue(type, out customType);
      return customType;
    }
    
    /// <summary>
    /// Получить обобщенный тип по типу контрола
    /// </summary>
    /// <param name="customType">Обобщенное наименование типа свойства</param>
    /// <param name="control">Контрол</param>
    /// <returns>Обобщенное наименование типа</returns>
    public static string GetMatchingControlTypeToCustomType(string customType, object control)
    {
      if (customType == Constants.Module.CustomType.Enumeration || customType == Constants.Module.CustomType.Navigation)
        return customType;
      
      if (control is CommonLibrary.IDateDialogValue)
        customType = Constants.Module.CustomType.Date;
      else if (control is CommonLibrary.IBooleanDialogValue)
        customType = Constants.Module.CustomType.Bool;
      else if (control is CommonLibrary.IIntegerDialogValue)
        customType = Constants.Module.CustomType.Numeric;
      else if (control is CommonLibrary.IStringDialogValue)
        customType = Constants.Module.CustomType.String;
      
      return customType;
    }
  }
}