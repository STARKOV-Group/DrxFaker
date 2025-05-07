﻿using System;
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
    /// Получить варианты заполнения по указанному типу.
    /// </summary>
    /// <param name="type">Обобщенное наименование типа.</param>
    /// <returns>Варианты выбора для заполнения.</returns>
    public static List<string> GetMatchingTypeToParameters(string type)
    {
      type = GetMatchingTypeToCustomType(type);
      var dict = new Dictionary<string, List<string>>()
      {
        { Constants.Module.CustomType.Date, Functions.Module.GetFillOptionForDate() },
        { Constants.Module.CustomType.Bool, Functions.Module.GetFillOptionForBool() },
        { Constants.Module.CustomType.Numeric, Functions.Module.GetFillOptionForNumeric() },
        { Constants.Module.CustomType.String, Functions.Module.GetFillOptionForString() },
        { Constants.Module.CustomType.Enumeration, Functions.Module.GetFillOptionForEnumeration() },
        { Constants.Module.CustomType.Navigation, Functions.Module.GetFillOptionForNavigation() }
      };
      
      var list = new List<string>();
      dict.TryGetValue(type, out list);
      return list;
    }

    /// <summary>
    /// Получить обобщенный тип по типу свойства.
    /// </summary>
    /// <param name="type">Наименование типа свойства.</param>
    /// <returns>Обобщенное наименование типа.</returns>
    public static string GetMatchingTypeToCustomType(string type)
    {
      var dict = new Dictionary<string, string>()
      {
        { Sungero.Metadata.PropertyType.Data.ToString(), Constants.Module.CustomType.Date },
        { Sungero.Metadata.PropertyType.DateTime.ToString(), Constants.Module.CustomType.Date },
        { Sungero.Metadata.PropertyType.Boolean.ToString(), Constants.Module.CustomType.Bool },
        { Sungero.Metadata.PropertyType.Double.ToString(), Constants.Module.CustomType.Numeric },
        { Sungero.Metadata.PropertyType.Integer.ToString(), Constants.Module.CustomType.Numeric },
        { Sungero.Metadata.PropertyType.LongInteger.ToString(), Constants.Module.CustomType.Numeric },
        { Sungero.Metadata.PropertyType.String.ToString(), Constants.Module.CustomType.String },
        { Sungero.Metadata.PropertyType.Text.ToString(), Constants.Module.CustomType.String },
        { Sungero.Metadata.PropertyType.Enumeration.ToString(), Constants.Module.CustomType.Enumeration },
        { Sungero.Metadata.PropertyType.Navigation.ToString(), Constants.Module.CustomType.Navigation }
      };
      
      var customType = string.Empty;
      dict.TryGetValue(type, out customType);
      return customType;
    }
  }
}