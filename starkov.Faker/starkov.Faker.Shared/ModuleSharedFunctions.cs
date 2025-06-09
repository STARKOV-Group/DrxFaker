using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using CommonLibrary;
using Sungero.Domain.Shared;
using Sungero.Metadata;

namespace starkov.Faker.Shared
{
  public class ModuleFunctions
  {
    
    #region Проверка и приведение типов
    
    /// <summary>
    /// Сравнить объект с типом.
    /// </summary>
    /// <param name="comparedObject">Объект для сравнения.</param>
    /// <param name="comparedType">Тип для сравнения.</param>
    /// <returns>True - если тип объекта совпадает с входным типом, иначе false.</returns>
    public static bool CompareObjectWithType(object comparedObject, System.Type comparedType)
    {
      if (comparedObject == null)
        return false;
      
      var objectType = comparedObject.GetType();
      return Equals(objectType, comparedType);
    }
    
    #region Приведение типов метаданных
    
    /// <summary>
    /// Приведение объекта к типу WorkflowEntityMetadata.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом WorkflowEntityMetadata, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Metadata.WorkflowEntityMetadata CastToWorkflowEntityMetadata(object castEntity)
    {
      Sungero.Metadata.WorkflowEntityMetadata val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (Sungero.Metadata.WorkflowEntityMetadata)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("WorkflowEntityMetadata", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу IEntity.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом IEntity, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Domain.Shared.IEntity CastToEntity(object castEntity)
    {
      IEntity val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IEntity)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IEntity", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу IChildEntityCollection<IChildEntity>.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом IChildEntityCollection<IChildEntity>, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Domain.Shared.IChildEntityCollection<IChildEntity> CastToChildEntityCollection(object castEntity)
    {
      IChildEntityCollection<IChildEntity> val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IChildEntityCollection<IChildEntity>)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IChildEntityCollection<IChildEntity>", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу EnumPropertyInfo.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом EnumPropertyInfo, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Domain.Shared.EnumPropertyInfo CastToEnumPropertyInfo(object castEntity)
    {
      EnumPropertyInfo val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (EnumPropertyInfo)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("EnumPropertyInfo", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу EnumPropertyMetadata.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом EnumPropertyMetadata, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Metadata.EnumPropertyMetadata CastToEnumPropertyMetadata(object castEntity)
    {
      EnumPropertyMetadata val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (EnumPropertyMetadata)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("EnumPropertyMetadata", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу NavigationPropertyMetadata.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом NavigationPropertyMetadata, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Metadata.NavigationPropertyMetadata CastToNavigationPropertyMetadata(object castEntity)
    {
      NavigationPropertyMetadata val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (NavigationPropertyMetadata)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("NavigationPropertyMetadata", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу StringPropertyMetadata.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом StringPropertyMetadata, либо null при ошибке во время приведения к типу.</returns>
    public static Sungero.Metadata.StringPropertyMetadata CastToStringPropertyMetadata(object castEntity)
    {
      StringPropertyMetadata val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (StringPropertyMetadata)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("StringPropertyMetadata", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    #endregion
    
    #region Приведение типов диалога
    
    /// <summary>
    /// Приведение объекта к типу DateDialogValue.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом DateDialogValue, либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IDateDialogValue CastToDateDialogValue(object castEntity)
    {
      IDateDialogValue val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IDateDialogValue)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IDateDialogValue", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу BooleanDialogValue.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом BooleanDialogValue, либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IBooleanDialogValue CastToBooleanDialogValue(object castEntity)
    {
      IBooleanDialogValue val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IBooleanDialogValue)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IBooleanDialogValue", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу IntegerDialogValue.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом IntegerDialogValue, либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IIntegerDialogValue CastToIntegerDialogValue(object castEntity)
    {
      IIntegerDialogValue val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IIntegerDialogValue)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IIntegerDialogValue", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу StringDialogValue.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом StringDialogValue, либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IStringDialogValue CastToStringDialogValue(object castEntity)
    {
      IStringDialogValue val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IStringDialogValue)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IStringDialogValue", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу DropDownDialogValue.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом DropDownDialogValue, либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IDropDownDialogValue CastToDropDownDialogValue(object castEntity)
    {
      IDropDownDialogValue val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IDropDownDialogValue)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IDropDownDialogValue", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу DialogControl(string).
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом DialogControl(string), либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IDialogControl<string> CastToDialogControlString(object castEntity)
    {
      IDialogControl<string> val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IDialogControl<string>)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IDialogControl<string>", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    /// <summary>
    /// Приведение объекта к типу DialogControl.
    /// </summary>
    /// <param name="castEntity">Объект для приведения.</param>
    /// <returns>Объект с типом DialogControl, либо null при ошибке во время приведения к типу.</returns>
    public static CommonLibrary.IDialogControl CastToDialogControl(object castEntity)
    {
      IDialogControl val = null;
      
      if (castEntity == null)
        return val;
      
      try
      {
        val = (IDialogControl)castEntity;
      }
      catch (Exception ex)
      {
        Logger.Error(starkov.Faker.Resources.ErrorDuringCastFormat("IDialogControl", ex.Message, ex.StackTrace));
      }
      
      return val;
    }
    
    #endregion
    
    #endregion

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
    
    #region Общие функции
    
    /// <summary>
    /// Получить ИД из строки.
    /// </summary>
    /// <param name="entitiyName">Строка с наименованием.</param>
    /// <returns>Список наименований сущностей.</returns>
    public virtual long GetIdFromEntitiyName(string entitiyName)
    {
      long id = 0;
      var idInString = entitiyName;
      var regex = new System.Text.RegularExpressions.Regex(@"\(\d*\)$");
      var matches = regex.Matches(entitiyName);
      if (matches.Count > 0)
      {
        foreach (System.Text.RegularExpressions.Match match in matches)
          idInString = match.Value.Substring(1, match.Value.Length-2);
      }
      
      long.TryParse(idInString, out id);
      return id;
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
        "Sid",
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
        "Storage",
        "Versions",
        "Parameters"
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
    
    #endregion
  }
}