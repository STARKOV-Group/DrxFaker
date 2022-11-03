using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace starkov.Faker.Structures.Module
{

  /// <summary>
  /// Информация о свойствах
  /// </summary>
  partial class PropertyInfo
  {
    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Локализированное наименование
    /// </summary>
    public string LocalizedName { get; set; }
    
    /// <summary>
    /// Тип свойства
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Guid ссылочного свойства
    /// </summary>
    public string PropertyGuid { get; set; }
    
    /// <summary>
    /// Признак обязательности свойства
    /// </summary>
    public bool IsRequired { get; set; }
    
    /// <summary>
    /// Значения свойства перечислении
    /// </summary>
    public List<string> EnumCollection { get; set; }
    
    /// <summary>
    /// Максимальная длина текстового поля
    /// </summary>
    public int? MaxStringLength { get; set; }
  }
}