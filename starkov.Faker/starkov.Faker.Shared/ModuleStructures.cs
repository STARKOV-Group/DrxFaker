using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace starkov.Faker.Structures.Module
{
  
  /// <summary>
  /// Информация о параметрах заполнения свойств.
  /// </summary>
  partial class ParameterInfo
  {
    /// <summary>
    /// Справочник соответствие заполняемых параметров сущности.
    /// </summary>
    public starkov.Faker.IParametersMatching ParametersMatching { get; set; }
    
    /// <summary>
    /// Наименование свойства.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Guid типа свойства.
    /// </summary>
    public string PropertyTypeGuid { get; set; }
    
    /// <summary>
    /// Тип свойства.
    /// </summary>
    public string PropertyType { get; set; }
    
    /// <summary>
    /// Вариант заполнения.
    /// </summary>
    public string FillOption { get; set; }
    
    /// <summary>
    /// Выбранное значение.
    /// </summary>
    public string ChosenValue { get; set; }
    
    /// <summary>
    /// Начальное значение промежутка.
    /// </summary>
    public string ValueFrom { get; set; }
    
    /// <summary>
    /// Конечное значение промежутка.
    /// </summary>
    public string ValueTo { get; set; }
  }
  
  /// <summary>
  /// Информация о коллекциях.
  /// </summary>
  partial class CollectionInfo
  {
    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Локализированное наименование.
    /// </summary>
    public string LocalizedName { get; set; }
    
    /// <summary>
    /// Информация о свойствах.
    /// </summary>
    public List<starkov.Faker.Structures.Module.PropertyInfo> Properties { get; set; }
  }

  /// <summary>
  /// Информация о свойствах.
  /// </summary>
  partial class PropertyInfo
  {
    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Локализированное наименование.
    /// </summary>
    public string LocalizedName { get; set; }
    
    /// <summary>
    /// Тип свойства.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Guid ссылочного свойства.
    /// </summary>
    public string PropertyGuid { get; set; }
    
    /// <summary>
    /// Признак обязательности свойства.
    /// </summary>
    public bool IsRequired { get; set; }
    
    /// <summary>
    /// Значения свойства перечислении.
    /// </summary>
    public List<starkov.Faker.Structures.Module.EnumerationInfo> EnumCollection { get; set; }
    
    /// <summary>
    /// Максимальная длина текстового поля.
    /// </summary>
    public int? MaxStringLength { get; set; }
  }
  
  /// <summary>
  /// Информация о перечислениях.
  /// </summary>
  partial class EnumerationInfo
  {
    /// <summary>
    /// Наименование.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Локализированное наименование.
    /// </summary>
    public string LocalizedName { get; set; }
  }
}