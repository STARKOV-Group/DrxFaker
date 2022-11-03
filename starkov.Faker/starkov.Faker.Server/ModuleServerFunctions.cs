using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Domain.SessionExtensions;
using Bogus;

namespace starkov.Faker.Server
{
  public class ModuleFunctions
  {
    
    #region Генерация данных
    
    #region Дата
    
    /// <summary>
    /// Генерация даты по временному промежутку
    /// </summary>
    /// <param name="from">Текстовая дата начала промежутка</param>
    /// <param name="to">Текстовая дата окончания промежутка</param>
    /// <returns>Дата</returns>
    public static DateTime GenerateDateByPeriod(string from, string to)
    {
      DateTime startDate;
      DateTime endDate;
      if (!Calendar.TryParseDate(from, out startDate) || !Calendar.TryParseDate(to, out endDate))
        return Calendar.Today;

      var rnd = new Random();
      int range = (endDate - startDate).Days;
      return startDate.AddDays(rnd.Next(range));
    }
    
    #endregion
    
    #region Логичесое значение
    
    /// <summary>
    /// Генерация логического значения
    /// </summary>
    /// <returns>True или false</returns>
    public static bool GenerateRandomBool()
    {
      var faker = new Bogus.Faker();
      return faker.Random.Bool();
    }
    
    #endregion
    
    #region Числа
    
    /// <summary>
    /// Генерация числа определенной длины
    /// </summary>
    /// <param name="strLength">Длина числа, в виде текста</param>
    /// <returns>Число</returns>
    public static int GenerateNumberWithLength(string strLength)
    {
      int length;
      if (!int.TryParse(strLength, out length))
        return 0;

      var rnd = new Random();
      var from = (int)Math.Pow(10, length - 1);
      var to = (int)Math.Pow(10, length) - 1;
      return rnd.Next(from, to);
    }

    /// <summary>
    /// Генерация числа из заданного диапазона
    /// </summary>
    /// <param name="from">Начало диапазона, в виде текста</param>
    /// <param name="to">Конец диапазона, в виде текста</param>
    /// <returns>Число</returns>
    public static int GenerateNumberByRange(string from, string to)
    {
      int startInt;
      int endInt;
      if (!int.TryParse(from, out startInt) || !int.TryParse(to, out endInt))
        return 0;

      var rnd = new Random();
      return rnd.Next(startInt, endInt);
    }
    
    #endregion
    
    #region Строка

    /// <summary>
    /// Генерация случайной строки
    /// </summary>
    /// <returns>Строка</returns>
    public static string GenerateStringSentence()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Lorem.Sentence(faker.Random.Int(1, 5));
    }
    
    /// <summary>
    /// Генерация параграфа
    /// </summary>
    /// <returns>Параграф</returns>
    public static string GenerateStringParagraph()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Lorem.Paragraph();
    }
    
    /// <summary>
    /// Генерация номера мобильного телефона
    /// </summary>
    /// <returns>Номер телефона</returns>
    public static string GenerateStringPhone()
    {
      var faker = new Bogus.Faker("ru");
      return "+7" + faker.Phone.PhoneNumber();
    }
    
    /// <summary>
    /// Генерация числа в виде строки
    /// </summary>
    /// <returns>Число в виде строки</returns>
    public static string GenerateStringNumber()
    {
      var faker = new Bogus.Faker();
      return faker.Random.Int(0, 1000000).ToString();
    }
    
    /// <summary>
    /// Генерация случайного имени
    /// </summary>
    /// <param name="gender">Пол</param>
    /// <returns>Имя</returns>
    public static string GenerateStringFirstName(string gender)
    {
      var faker = new Bogus.Faker("ru");
      Bogus.DataSets.Name.Gender enumGender;
      if (Enum.TryParse(gender, out enumGender))
        return faker.Name.FirstName(enumGender as Bogus.DataSets.Name.Gender?);
      
      return faker.Name.FirstName();
    }
    
    /// <summary>
    /// Генерация случайной фамилии
    /// </summary>
    /// <param name="gender">Пол</param>
    /// <returns>Фамилия</returns>
    public static string GenerateStringLastName(string gender)
    {
      var faker = new Bogus.Faker("ru");
      Bogus.DataSets.Name.Gender enumGender;
      if (Enum.TryParse(gender, out enumGender))
        return faker.Name.LastName(enumGender as Bogus.DataSets.Name.Gender?);
      
      return faker.Name.LastName();
    }
    
    /// <summary>
    /// Генерация случайного ФИО
    /// </summary>
    /// <param name="gender">Пол</param>
    /// <returns>ФИО</returns>
    public static string GenerateStringFullName(string gender)
    {
      var faker = new Bogus.Faker("ru");
      Bogus.DataSets.Name.Gender enumGender;
      if (Enum.TryParse(gender, out enumGender))
        return faker.Name.FullName(enumGender as Bogus.DataSets.Name.Gender?);
      
      return faker.Name.FullName();
    }
    
    /// <summary>
    /// Генерация случайного названия должности
    /// </summary>
    /// <returns>Название должности</returns>
    public static string GenerateStringJobTitle()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Name.JobTitle();
    }
    
    /// <summary>
    /// Генерация случайного email
    /// </summary>
    /// <returns>Email</returns>
    public static string GenerateStringEmail()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Person.Email;
    }
    
    /// <summary>
    /// Генерация случайного логина
    /// </summary>
    /// <returns>Логин</returns>
    public static string GenerateStringLogin()
    {
      var faker = new Bogus.Faker();
      return faker.Internet.UserName();
    }
    
    /// <summary>
    /// Генерация случайного субъекта федерации
    /// </summary>
    /// <returns>Субъект федерации</returns>
    public static string GenerateStringState()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Person.Address.State;
    }
    
    /// <summary>
    /// Генерация случайного города
    /// </summary>
    /// <returns>Город</returns>
    public static string GenerateStringCity()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Person.Address.City;
    }
    
    /// <summary>
    /// Генерация случайной улицы
    /// </summary>
    /// <returns>Улица</returns>
    public static string GenerateStringStreet()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Person.Address.Street;
    }
    
    /// <summary>
    /// Генерация случайного подразделения
    /// </summary>
    /// <returns>Название подразделения</returns>
    public static string GenerateStringDepartment()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Commerce.Department();
    }
    
    /// <summary>
    /// Генерация случайной организации
    /// </summary>
    /// <returns>Название организации</returns>
    public static string GenerateStringCompanyName()
    {
      var faker = new Bogus.Faker("ru");
      return faker.Company.CompanyName();
    }
    
    #endregion
    
    #region Перечисление
    
    /// <summary>
    /// Выбор случайного перечисления из коллекции строк
    /// </summary>
    /// <param name="enumvalues">Список значений перечислений</param>
    /// <returns>Перечисление</returns>
    public static Enumeration PickRandomEnumeration(List<string> enumvalues)
    {
      var faker = new Bogus.Faker();
      var selectedString = faker.PickRandom(enumvalues);
      return new Enumeration(selectedString);
    }
    
    #endregion
    
    #region Ссылка
    
    /// <summary>
    /// Выбор случайной сущности по guid типа
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <param name="documentTypeGuid">Guid типа документа</param>
    /// <returns>Случайно выбранная сущность</returns>
    public static IEntity PickRandomEntity(string typeGuid, string documentTypeGuid)
    {
      var entities = Functions.Module.GetEntitiesByTypeGuid(typeGuid, documentTypeGuid);
      if (!entities.Any())
        return null;
      
      var faker = new Bogus.Faker();
      return entities.Skip(faker.Random.Int(0, entities.Count() - 1)).FirstOrDefault();
    }
    
    /// <summary>
    /// Выбор случайного не использующегося логина
    /// </summary>
    /// <returns>Случайно выбранный логин</returns>
    public static ILogin PickRandomLogin()
    {
      var logins = Functions.Module.GetAllUnusedLogins();
      if (!logins.Any())
        return null;
      
      var faker = new Bogus.Faker();
      return logins.Skip(faker.Random.Int(0, logins.Count() - 1)).FirstOrDefault();
    }
    
    #endregion
    
    #endregion
    
    #region Получение значения свойства в зависимости от параметров
    
    /// <summary>
    /// Получить значение свойства по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Значение свойства</returns>
    public object GetPropertyValueByParameters(IParametersMatchingParameters parameterRow)
    {
      object result = null;
      if (parameterRow == null)
        return result;
      
      if (parameterRow.PropertyType == Constants.Module.CustomType.Date)
        result = GetDateByParameters(parameterRow);
      else if (parameterRow.PropertyType == Constants.Module.CustomType.Bool)
        result = GetBoolByParameters(parameterRow);
      else if (parameterRow.PropertyType == Constants.Module.CustomType.Numeric)
        result = GetIntByParameters(parameterRow);
      else if (parameterRow.PropertyType == Constants.Module.CustomType.String)
        result = GetStringByParameters(parameterRow);
      else if (parameterRow.PropertyType == Constants.Module.CustomType.Enumeration)
        result = GetEnumByParameters(parameterRow);
      else if (parameterRow.PropertyType == Constants.Module.CustomType.Navigation)
        result = GetEntityByParameters(parameterRow);
      
      return result;
    }
    
    /// <summary>
    /// Получить дату по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Дата</returns>
    public DateTime GetDateByParameters(IParametersMatchingParameters parameterRow)
    {
      DateTime date = Calendar.Today;
      
      if (parameterRow.FillOption == Constants.Module.Common.FixedValue && Calendar.TryParseDate(parameterRow.ChosenValue, out date))
        return date;
      
      if (parameterRow.FillOption == Constants.Module.Date.Period)
        date = Functions.Module.GenerateDateByPeriod(parameterRow.ValueFrom, parameterRow.ValueTo);
      
      return date;
    }
    
    /// <summary>
    /// Получить логическое значение по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Логическое значение</returns>
    public bool GetBoolByParameters(IParametersMatchingParameters parameterRow)
    {
      bool logic = false;
      
      if (parameterRow.FillOption == Constants.Module.Common.FixedValue && bool.TryParse(parameterRow.ChosenValue, out logic))
        return logic;
      
      if (parameterRow.FillOption == Constants.Module.Common.RandomValue)
        logic = Functions.Module.GenerateRandomBool();
      
      return logic;
    }
    
    /// <summary>
    /// Получить число по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Число</returns>
    public int GetIntByParameters(IParametersMatchingParameters parameterRow)
    {
      int num = 0;
      
      if (parameterRow.FillOption == Constants.Module.Common.FixedValue && int.TryParse(parameterRow.ChosenValue, out num))
        return num;
      
      if (parameterRow.FillOption == Constants.Module.Numeric.NumberWithLength)
        num = Functions.Module.GenerateNumberWithLength(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.Numeric.NumberRange)
        num = Functions.Module.GenerateNumberByRange(parameterRow.ValueFrom, parameterRow.ValueTo);
      
      return num;
    }
    
    /// <summary>
    /// Получить строку по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Строка</returns>
    public string GetStringByParameters(IParametersMatchingParameters parameterRow)
    {
      var str = string.Empty;
      
      if (parameterRow.FillOption == Constants.Module.Common.FixedValue)
        str = parameterRow.ChosenValue;
      else if (parameterRow.FillOption == Constants.Module.String.RandomString)
        str = Functions.Module.GenerateStringSentence();
      else if (parameterRow.FillOption == Constants.Module.String.Paragraph)
        str = Functions.Module.GenerateStringParagraph();
      else if (parameterRow.FillOption == Constants.Module.String.RandomPhone)
        str = Functions.Module.GenerateStringPhone();
      else if (parameterRow.FillOption == Constants.Module.String.NumberInStr)
        str = Functions.Module.GenerateStringNumber();
      else if (parameterRow.FillOption == Constants.Module.String.FirstName)
        str = Functions.Module.GenerateStringFirstName(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.String.LastName)
        str = Functions.Module.GenerateStringLastName(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.String.FullName)
        str = Functions.Module.GenerateStringFullName(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.String.JobTitle)
        str = Functions.Module.GenerateStringJobTitle();
      else if (parameterRow.FillOption == Constants.Module.String.Email)
        str = Functions.Module.GenerateStringEmail();
      else if (parameterRow.FillOption == Constants.Module.String.Login)
        str = Functions.Module.GenerateStringLogin();
      else if (parameterRow.FillOption == Constants.Module.String.State)
        str = Functions.Module.GenerateStringState();
      else if (parameterRow.FillOption == Constants.Module.String.City)
        str = Functions.Module.GenerateStringCity();
      else if (parameterRow.FillOption == Constants.Module.String.Street)
        str = Functions.Module.GenerateStringStreet();
      else if (parameterRow.FillOption == Constants.Module.String.Department)
        str = Functions.Module.GenerateStringDepartment();
      else if (parameterRow.FillOption == Constants.Module.String.CompanyName)
        str = Functions.Module.GenerateStringCompanyName();
      
      return str;
    }
    
    /// <summary>
    /// Получить перечисление по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Перечисление</returns>
    public Enumeration? GetEnumByParameters(IParametersMatchingParameters parameterRow)
    {
      Enumeration? newEnum = null;
      var databook = parameterRow.ParametersMatching;
      
      if (parameterRow.FillOption == Constants.Module.Common.FixedValue)
        newEnum = new Enumeration(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.Common.RandomValue)
      {
        var properties = Functions.Module.GetPropertiesType(databook.DatabookType?.DatabookTypeGuid ?? databook.DocumentType?.DocumentTypeGuid);
        var enumValues = properties.FirstOrDefault(_ => _.Name == parameterRow.PropertyName).EnumCollection;
        newEnum = Functions.Module.PickRandomEnumeration(enumValues);
      }
      
      return newEnum;
    }
    
    /// <summary>
    /// Получить сущность по параметрам
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами</param>
    /// <returns>Сущность</returns>
    public IEntity GetEntityByParameters(IParametersMatchingParameters parameterRow)
    {
      int num;
      IEntity entity = null;
      var databook = parameterRow.ParametersMatching;
      
      if (parameterRow.FillOption == Constants.Module.Common.FixedValue && int.TryParse(parameterRow.ChosenValue, out num))
        entity = GetEntityByTypeGuidAndId(parameterRow.PropertyTypeGuid, num);
      else if (parameterRow.FillOption == Constants.Module.Common.RandomValue)
      {
        if (parameterRow.PropertyName == "Login" &&
            !string.IsNullOrEmpty(databook.DatabookType?.DatabookTypeGuid) &&
            Equals(TypeExtension.GetTypeByGuid(Guid.Parse(databook.DatabookType.DatabookTypeGuid)), typeof(Sungero.Company.IEmployee)))
          entity = PickRandomLogin();
        else
          entity = PickRandomEntity(parameterRow.PropertyTypeGuid, databook.DocumentType?.DocumentTypeGuid);
      }
      
      return entity;
    }
    
    #endregion
    
    #region Общие функции
    
    /// <summary>
    /// Запустить АО для генерации сущностей
    /// </summary>
    /// <param name="count">Кол-во создаваемых записей</param>
    /// <param name="databookId">ИД справочника по которому будет происходить генерация</param>
    [Remote]
    public virtual void CreateAsyncForGenerateEntities(int count, int databookId)
    {
      var asyncHandler = Faker.AsyncHandlers.EntitiesGeneration.Create();
      asyncHandler.Count = count;
      asyncHandler.DatabookId = databookId;
      asyncHandler.ExecuteAsync();
    }
    
    /// <summary>
    /// Получить последний Guid типа
    /// </summary>
    /// <param name="guid">Guid типа сущности</param>
    /// <returns>Последний Guid типа</returns>
    [Remote]
    public virtual string GetFinalTypeGuidByAncestor(string guid)
    {
      var typeGuid = Guid.Parse(guid);
      var type = TypeExtension.GetTypeByGuid(typeGuid);
      if (type == null || !type.GetEntityMetadata().IsAncestorMetadata)
        return guid;
      
      var finalType = type.GetFinalType();
      if (finalType == null)
        return guid;
      
      return finalType.GUID.ToString();
    }
    
    /// <summary>
    /// Получить список с информацией о реквизитах типа сущности
    /// </summary>
    /// <param name="guid">Guid типа сущности</param>
    /// <returns>список с информацией о реквизитах типа сущности</returns>
    [Remote]
    public static List<Structures.Module.PropertyInfo> GetPropertiesType(string guid)
    {
      var propertiesList = new List<Structures.Module.PropertyInfo>();
      
      var typeGuid = Guid.Parse(guid);
      var type = TypeExtension.GetTypeByGuid(typeGuid);
      if (type == null)
        return propertiesList;
      
      var typeMetadata = type.GetFinalType().GetEntityMetadata();
      
      var excludeProperties = Functions.Module.GetExcludeProperties();
      var excludePropertyTypes = Functions.Module.GetExcludePropertyTypes();
      var properties = typeMetadata.Properties.Where(_ => !excludeProperties.Contains(_.Name))
        .Where(_ => !excludePropertyTypes.Contains(_.PropertyType));
      
      //Учетные записи
      if (guid == Constants.Module.Guids.Login)
      {
        var additionalExcludeProps = Functions.Module.GetAdditionalExcludePropForLogins();
        properties = properties.Where(_ => !additionalExcludeProps.Contains(_.Name));
        
        propertiesList.Add(Structures.Module.PropertyInfo.Create("Password",
                                                                 "Пароль",
                                                                 Constants.Module.CustomType.String,
                                                                 string.Empty,
                                                                 true,
                                                                 new List<string>(),
                                                                 null));
      }
      
      foreach (var propertyMetadata in properties)
      {
        propertiesList.Add(Structures.Module.PropertyInfo.Create(propertyMetadata.Name,
                                                                 propertyMetadata.GetLocalizedName().ToString(),
                                                                 propertyMetadata.PropertyType.ToString(),
                                                                 (propertyMetadata as Sungero.Metadata.NavigationPropertyMetadata)?.EntityGuid.ToString() ?? string.Empty,
                                                                 propertyMetadata.IsRequired,
                                                                 (propertyMetadata as Sungero.Metadata.EnumPropertyMetadata)?.Values.Select(_ => _.Name).ToList() ?? new List<string>(),
                                                                 (propertyMetadata as Sungero.Metadata.StringPropertyMetadata)?.Length));
      }
      
      return propertiesList;
    }
    
    #endregion
    
    #region Работа с сущностями
    
    /// <summary>
    /// Получить список всех сущностей по Guid типа
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <returns>Список сущностей</returns>
    [Remote]
    public virtual IQueryable<IEntity> GetEntitiesByTypeGuid(string typeGuid)
    {
      return GetEntitiesByTypeGuid(typeGuid, string.Empty);
    }
    
    /// <summary>
    /// Получить список сущностей по Guid типа
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <param name="documentTypeGuid">Guid типа документа</param>
    /// <returns>Список сущностей</returns>
    [Remote]
    public virtual IQueryable<IEntity> GetEntitiesByTypeGuid(string typeGuid, string documentTypeGuid)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(typeGuid));
      using (var session = new Sungero.Domain.Session())
      {
        if (entityType.Name == "IDocumentKind" && !string.IsNullOrEmpty(documentTypeGuid))
          return session.GetAll(entityType).Where(_ => Sungero.Docflow.DocumentKinds.As(_).DocumentType.DocumentTypeGuid == documentTypeGuid);
        else
          return session.GetAll(entityType);
      }
    }
    
    /// <summary>
    /// Получить сущность по Guid типа и id
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <param name="id">ИД сущности</param>
    /// <returns>Cущность</returns>
    [Remote]
    public virtual IEntity GetEntityByTypeGuidAndId(string typeGuid, int id)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(typeGuid));
      using (var session = new Sungero.Domain.Session())
      {
        return session.GetAll(entityType).FirstOrDefault(_ => _.Id == id);
      }
    }
    
    /// <summary>
    /// Создать сущность по Guid типа
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности</param>
    /// <returns>Сущность</returns>
    [Remote]
    public virtual IEntity CreateEntityByTypeGuid(string typeGuid)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(typeGuid));
      using (var session = new Sungero.Domain.Session())
      {
        return session.Create(entityType);
      }
    }
    
    /// <summary>
    /// Получить список всех не использующихся учетных записей
    /// </summary>
    /// <returns>Список учетных записей</returns>
    [Remote]
    public virtual IQueryable<ILogin> GetAllUnusedLogins()
    {
      var usedLoginsId = Sungero.Company.PublicFunctions.Employee.Remote.GetEmployees()
        .Where(_ => _.Login != null)
        .Select(_ => _.Login.Id);
      var systemLogins = new List<string>() { "Administrator", "Integration Service", "Service User", "Adviser" };
      
      return Sungero.CoreEntities.Logins.GetAll(_ => !usedLoginsId.Contains(_.Id))
        .Where(_ => !systemLogins.Contains(_.LoginName));
    }
    
    #endregion
  }
}