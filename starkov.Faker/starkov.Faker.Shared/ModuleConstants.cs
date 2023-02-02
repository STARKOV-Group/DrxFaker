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
      
      /// <summary>
      /// Значение.
      /// </summary>
      public const string Value = "Value";
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
    }
    
    /// <summary>
    /// Форматы документов.
    /// </summary>
    public static class DocumentFormats
    {
      /// <summary>
      /// Pdf.
      /// </summary>
      public const string Pdf = "pdf";
    }
    
    /// <summary>
    /// Язык формирования данных в библиотеке Bogus.
    /// </summary>
    public static class BogusLanguages
    {
      /// <summary>
      /// Русский.
      /// </summary>
      public const string Russian = "ru";
    }
  }
}