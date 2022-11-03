using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace starkov.Faker.Shared
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Получить список наименований всех сущностей по Guid типа
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <param name="documentTypeGuid">Guid типа документа</param>
    /// <returns>Список наименований сущностей</returns>
    public virtual System.Collections.Generic.IEnumerable<string> GetEntitiyNamesByType(string typeGuid, string documentTypeGuid)
    {
      return Functions.Module.Remote.GetEntitiesByTypeGuid(typeGuid, documentTypeGuid).AsEnumerable()
        .Select(_ => string.Format("{0}, Id: ({1})", _.DisplayValue, _.Id));
    }
    
    /// <summary>
    /// Получить список реквизитов, которые не доступны для выбора
    /// </summary>
    /// <returns>Список реквизитов</returns>
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
    /// Получить список дополнительных реквизитов сущности Учетная запись, которые не доступны для выбора
    /// </summary>
    /// <returns>Список реквизитов</returns>
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
    /// Получить список типов реквизитов, которые не доступны для выбора
    /// </summary>
    /// <returns>Список реквизитов</returns>
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