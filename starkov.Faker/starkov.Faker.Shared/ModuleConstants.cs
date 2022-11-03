using System;
using Sungero.Core;

namespace starkov.Faker.Constants
{
  public static class Module
  {
    /// <summary>
    /// Наименования свойств.
    /// </summary>
    public static class PropertyNames
    {
      /// <summary>
      /// Пароль.
      /// </summary>
      public const string Password = "Password";
      
      /// <summary>
      /// Логин.
      /// </summary>
      public const string LoginName = "LoginName";
    }
    
    /// <summary>
    /// Обобщенное наименование типов.
    /// </summary>
    public static class CustomType
    {
      /// <summary>
      /// Дата.
      /// </summary>
      public const string Date = "Date";
      
      /// <summary>
      /// Логический тип данных.
      /// </summary>
      public const string Bool = "Bool";
      
      /// <summary>
      /// Числовой тип данных.
      /// </summary>
      public const string Numeric = "Numeric";
      
      /// <summary>
      /// Строковый тип данных.
      /// </summary>
      public const string String = "String";
      
      /// <summary>
      /// Перечисление.
      /// </summary>
      public const string Enumeration = "Enumeration";
      
      /// <summary>
      /// Ссылочный тип данных.
      /// </summary>
      public const string Navigation = "Navigation";
    }
    
    /// <summary>
    /// Варианты заполнения по обобщенным типам.
    /// </summary>
    public static class FillOptions
    {
      /// <summary>
      /// Варианты заполнения использующиеся во всех типах.
      /// </summary>
      public static class Common
      {
        public const string FixedValue = "Фиксированное значение";
        public const string RandomValue = "Случайное распределение";
        public const string NullValue = "Пустое значение (NULL)";
      }
      
      /// <summary>
      /// Варианты заполнения данных с типом дата.
      /// </summary>
      public static class Date
      {
        public const string Period = "Временной промежуток";
      }
      
      /// <summary>
      /// Варианты заполнения данных с логичиским типом.
      /// </summary>
      public static class Bool
      {
        
      }
      
      /// <summary>
      /// Варианты заполнения данных с числовым типом.
      /// </summary>
      public static class Numeric
      {
        public const string NumberRange = "Диапазон чисел";
        public const string NumberWithLength = "Число определенной длины";
      }
      
      /// <summary>
      /// Варианты заполнения данных со строковым типом.
      /// </summary>
      public static class String
      {
        public const string RandomString = "Случайная строка";
        public const string Paragraph = "Случайный параграф";
        public const string RandomPhone = "Телефонный номер";
        public const string NumberInStr = "Число в виде строки";
        public const string Email = "Email";
        public const string Login = "Логин";
        public const string LastName = "Фамилия";
        public const string FirstName = "Имя";
        public const string FullName = "ФИО";
        public const string JobTitle = "Название должности";
        public const string State = "Субъект федерации";
        public const string City = "Город";
        public const string Street = "Улица";
        public const string Department = "Название подразделения";
        public const string CompanyName = "Название организации";
      }
      
      /// <summary>
      /// Варианты заполнения данных с типом перечисление.
      /// </summary>
      public static class Enumeration
      {
        
      }
      
      /// <summary>
      /// Варианты заполнения данных с типом ссылка.
      /// </summary>
      public static class Navigation
      {
        
      }
    }
    
    /// <summary>
    /// Guid'ы сущностей.
    /// </summary>
    public static class Guids
    {
      /// <summary>
      /// GUID справочника "Учетная запись".
      /// </summary>
      public const string Login = "55f542e9-4645-4f8d-999e-73cc71df62fd";
      
      /// <summary>
      /// GUID справочника "Наша организация".
      /// </summary>
      public const string BusinessUnit = "eff95720-181f-4f7d-892d-dec034c7b2ab";
      
      /// <summary>
      /// GUID справочника "Подразделение".
      /// </summary>
      public const string Department = "61b1c19f-26e2-49a5-b3d3-0d3618151e12";
      
      /// <summary>
      /// GUID справочника "Персона".
      /// </summary>
      public const string Person = "f5509cdc-ac0c-4507-a4d3-61d7a0a9b6f6";
      
      /// <summary>
      /// GUID справочника "Сотрудник".
      /// </summary>
      public const string Employee = "b7905516-2be5-4931-961c-cb38d5677565";
      
      /// <summary>
      /// GUID справочника "Должность".
      /// </summary>
      public const string JobTitle = "4a37aec4-764c-4c14-8887-e1ecafa5b4c5";
    }
  }
}