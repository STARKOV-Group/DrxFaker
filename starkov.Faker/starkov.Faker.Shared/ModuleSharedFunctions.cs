using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace starkov.Faker.Shared
{
  public class ModuleFunctions
  {

    #region Получение вариантов заполнения
    
    /// <summary>
    /// Получить варианты заполнения для свойств с типом Дата.
    /// </summary>
    /// <returns>Варианты заполнения.</returns>
    public virtual List<string> GetFillOptionForDate()
    {
      return new List<string>() {
        Constants.Module.FillOptions.Common.NullValue,
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.Date.Period
      };
    }
    
    /// <summary>
    /// Получить варианты заполнения для свойств с логичиским типом данных.
    /// </summary>
    /// <returns>Варианты заполнения.</returns>
    public virtual List<string> GetFillOptionForBool()
    {
      return new List<string>() {
        Constants.Module.FillOptions.Common.NullValue,
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.Common.RandomValue
      };
    }
    
    /// <summary>
    /// Получить варианты заполнения для свойств с числовым типом данных.
    /// </summary>
    /// <returns>Варианты заполнения.</returns>
    public virtual List<string> GetFillOptionForNumeric()
    {
      return new List<string>() {
        Constants.Module.FillOptions.Common.NullValue,
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.Numeric.NumberRange,
        Constants.Module.FillOptions.Numeric.NumberWithLength
      };
    }
    
    /// <summary>
    /// Получить варианты заполнения для свойств с строковым типом данных.
    /// </summary>
    /// <returns>Варианты заполнения.</returns>
    public virtual List<string> GetFillOptionForString()
    {
      return new List<string>() {
        Constants.Module.FillOptions.Common.NullValue,
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.String.RandomString,
        Constants.Module.FillOptions.String.Paragraph,
        Constants.Module.FillOptions.String.RandomPhone,
        Constants.Module.FillOptions.String.NumberInStr,
        Constants.Module.FillOptions.String.FirstName,
        Constants.Module.FillOptions.String.LastName,
        Constants.Module.FillOptions.String.FullName,
        Constants.Module.FillOptions.String.JobTitle,
        Constants.Module.FillOptions.String.Email,
        Constants.Module.FillOptions.String.Login,
        Constants.Module.FillOptions.String.State,
        Constants.Module.FillOptions.String.City,
        Constants.Module.FillOptions.String.Street,
        Constants.Module.FillOptions.String.Department,
        Constants.Module.FillOptions.String.CompanyName
      };
    }
    
    /// <summary>
    /// Получить варианты заполнения для свойств с типом Перечисление.
    /// </summary>
    /// <returns>Варианты заполнения.</returns>
    public virtual List<string> GetFillOptionForEnumeration()
    {
      return new List<string>() {
        Constants.Module.FillOptions.Common.NullValue,
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.Common.RandomValue
      };
    }
    
    /// <summary>
    /// Получить варианты заполнения для свойств с сылочным типом данных.
    /// </summary>
    /// <returns>Варианты заполнения.</returns>
    public virtual List<string> GetFillOptionForNavigation()
    {
      return new List<string>() {
        Constants.Module.FillOptions.Common.NullValue,
        Constants.Module.FillOptions.Common.FixedValue,
        Constants.Module.FillOptions.Common.RandomValue
      };
    }
    
    #endregion
    
    /// <summary>
    /// Получить список наименований всех сущностей по Guid типа.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <param name="documentTypeGuid">Guid типа документа.</param>
    /// <returns>Список наименований сущностей.</returns>
    public virtual System.Collections.Generic.IEnumerable<string> GetEntitiyNamesByType(string typeGuid, string documentTypeGuid)
    {
      return Functions.Module.Remote.GetEntitiesByTypeGuid(typeGuid, documentTypeGuid).AsEnumerable()
        .Select(_ => string.Format("{0}, Id: ({1})", _.DisplayValue, _.Id));
    }
    
    /// <summary>
    /// Получить список реквизитов, которые не доступны для выбора.
    /// </summary>
    /// <returns>Список реквизитов.</returns>
    public virtual List<string> GetExcludeProperties()
    {
      return new List<string> {
        "Id",
        "SID",
        "TypeDiscriminator",
        "Params",
        "NeedWriteHistory",
        "DisplayValue",
        "IsPropertyChangedHandlerEnabled",
        "SecureObject",
        "InternalAccessRights",
        "IsTransient",
        "RootEntity",
        "PersonalPhotoHash",
        "IsSystem",
        "PersonalPhotoHash",
        "IsCardReadOnly",
        "HasRelations",
        "AssociatedApplication",
        "LastVersionApproved",
        "HasVersions",
        "LastVersionSignatureType",
        "HasPublicBody",
        "Storage"
      };
    }
    
    /// <summary>
    /// Получить список дополнительных реквизитов сущности Учетная запись, которые не доступны для выбора.
    /// </summary>
    /// <returns>Список реквизитов.</returns>
    public virtual List<string> GetAdditionalExcludePropForLogins()
    {
      return new List<string> {
        "NeedChangePassword",
        "TypeAuthentication",
        "PasswordLastChangeDate",
        "LockoutEndDate"
      };
    }
    
    /// <summary>
    /// Получить список типов реквизитов, которые не доступны для выбора.
    /// </summary>
    /// <returns>Список типов.</returns>
    public virtual List<object> GetExcludePropertyTypes()
    {
      return new List<object> {
        Sungero.Metadata.PropertyType.Collection,
        Sungero.Metadata.PropertyType.BinaryData,
        Sungero.Metadata.PropertyType.Component,
        Sungero.Metadata.PropertyType.Image
      };
    }
  }
}