using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using Sungero.Domain.SessionExtensions;
using Bogus;
using System.Text.RegularExpressions;

namespace starkov.Faker.Server
{
  public class ModuleFunctions
  {
    
    #region Генерация данных
    
    #region Дата
    
    /// <summary>
    /// Генерация даты по временному промежутку.
    /// </summary>
    /// <param name="from">Текстовая дата начала промежутка.</param>
    /// <param name="to">Текстовая дата окончания промежутка.</param>
    /// <returns>Дата.</returns>
    public virtual DateTime GenerateDateByPeriod(string from, string to)
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
    /// Генерация логического значения.
    /// </summary>
    /// <returns>True или false.</returns>
    public virtual bool GenerateRandomBool()
    {
      var faker = new Bogus.Faker();
      return faker.Random.Bool();
    }
    
    #endregion
    
    #region Числа
    
    /// <summary>
    /// Генерация числа определенной длины.
    /// </summary>
    /// <param name="strLength">Длина числа, в виде текста.</param>
    /// <returns>Число.</returns>
    public virtual int GenerateNumberWithLength(string strLength)
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
    /// Генерация числа из заданного диапазона.
    /// </summary>
    /// <param name="from">Начало диапазона, в виде текста.</param>
    /// <param name="to">Конец диапазона, в виде текста.</param>
    /// <returns>Число.</returns>
    public virtual int GenerateNumberByRange(string from, string to)
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
    /// Генерация случайной строки.
    /// </summary>
    /// <returns>Строка.</returns>
    public virtual string GenerateStringSentence()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Lorem.Sentence(faker.Random.Int(1, 5));
    }
    
    /// <summary>
    /// Генерация параграфа.
    /// </summary>
    /// <returns>Параграф.</returns>
    public virtual string GenerateStringParagraph()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Lorem.Paragraph();
    }
    
    /// <summary>
    /// Генерация номера мобильного телефона.
    /// </summary>
    /// <returns>Номер телефона.</returns>
    public virtual string GenerateStringPhone()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return "+7" + faker.Phone.PhoneNumber();
    }
    
    /// <summary>
    /// Генерация числа в виде строки.
    /// </summary>
    /// <returns>Число в виде строки.</returns>
    public virtual string GenerateStringNumber()
    {
      var faker = new Bogus.Faker();
      return faker.Random.Int(0, 1000000).ToString();
    }
    
    /// <summary>
    /// Генерация случайного имени.
    /// </summary>
    /// <param name="gender">Пол.</param>
    /// <returns>Имя.</returns>
    public virtual string GenerateStringFirstName(string gender)
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      Bogus.DataSets.Name.Gender enumGender;
      if (Enum.TryParse(gender, out enumGender))
        return faker.Name.FirstName(enumGender as Bogus.DataSets.Name.Gender?);
      
      return faker.Name.FirstName();
    }
    
    /// <summary>
    /// Генерация случайной фамилии.
    /// </summary>
    /// <param name="gender">Пол.</param>
    /// <returns>Фамилия.</returns>
    public virtual string GenerateStringLastName(string gender)
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      Bogus.DataSets.Name.Gender enumGender;
      if (Enum.TryParse(gender, out enumGender))
        return faker.Name.LastName(enumGender as Bogus.DataSets.Name.Gender?);
      
      return faker.Name.LastName();
    }
    
    /// <summary>
    /// Генерация случайного ФИО.
    /// </summary>
    /// <param name="gender">Пол.</param>
    /// <returns>ФИО.</returns>
    public virtual string GenerateStringFullName(string gender)
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      Bogus.DataSets.Name.Gender enumGender;
      if (Enum.TryParse(gender, out enumGender))
        return faker.Name.FullName(enumGender as Bogus.DataSets.Name.Gender?);
      
      return faker.Name.FullName();
    }
    
    /// <summary>
    /// Генерация случайного названия должности.
    /// </summary>
    /// <returns>Название должности.</returns>
    public virtual string GenerateStringJobTitle()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Name.JobTitle();
    }
    
    /// <summary>
    /// Генерация случайного email.
    /// </summary>
    /// <returns>Email.</returns>
    public virtual string GenerateStringEmail()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Person.Email;
    }
    
    /// <summary>
    /// Генерация случайного логина.
    /// </summary>
    /// <returns>Логин.</returns>
    public virtual string GenerateStringLogin()
    {
      var faker = new Bogus.Faker();
      return faker.Internet.UserName();
    }
    
    /// <summary>
    /// Генерация случайного субъекта федерации.
    /// </summary>
    /// <returns>Субъект федерации.</returns>
    public virtual string GenerateStringState()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Person.Address.State;
    }
    
    /// <summary>
    /// Генерация случайного города.
    /// </summary>
    /// <returns>Город.</returns>
    public virtual string GenerateStringCity()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Person.Address.City;
    }
    
    /// <summary>
    /// Генерация случайной улицы.
    /// </summary>
    /// <returns>Улица.</returns>
    public virtual string GenerateStringStreet()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Person.Address.Street;
    }
    
    /// <summary>
    /// Генерация случайного подразделения.
    /// </summary>
    /// <returns>Название подразделения.</returns>
    public virtual string GenerateStringDepartment()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Commerce.Department();
    }
    
    /// <summary>
    /// Генерация случайной организации.
    /// </summary>
    /// <returns>Название организации.</returns>
    public virtual string GenerateStringCompanyName()
    {
      var faker = new Bogus.Faker(Constants.Module.BogusLanguages.Russian);
      return faker.Company.CompanyName();
    }
    
    #endregion
    
    #region Перечисление
    
    /// <summary>
    /// Выбор случайного перечисления из коллекции строк.
    /// </summary>
    /// <param name="enumvalues">Список значений перечислений.</param>
    /// <returns>Перечисление.</returns>
    public virtual Enumeration PickRandomEnumeration(List<string> enumvalues)
    {
      var faker = new Bogus.Faker();
      var selectedString = faker.PickRandom(enumvalues);
      return new Enumeration(selectedString);
    }
    
    #endregion
    
    #region Ссылка
    
    /// <summary>
    /// Выбор случайной сущности по guid типа.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <param name="documentTypeGuid">Guid типа документа.</param>
    /// <returns>Случайно выбранная сущность.</returns>
    public virtual IEntity PickRandomEntity(string typeGuid, string documentTypeGuid)
    {
      var entities = Functions.Module.GetEntitiesByTypeGuid(typeGuid, documentTypeGuid);
      if (!entities.Any())
        return null;
      
      var faker = new Bogus.Faker();
      return entities.Skip(faker.Random.Int(0, entities.Count() - 1)).FirstOrDefault();
    }
    
    /// <summary>
    /// Выбор случайного не использующегося логина.
    /// </summary>
    /// <returns>Случайно выбранный логин.</returns>
    public virtual ILogin PickRandomLogin()
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
    /// Получить значение свойства по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Значение свойства.</returns>
    public virtual object GetPropertyValueByParameters(IParametersMatchingParameters parameterRow, List<starkov.Faker.Structures.Module.PropertyInfo> propertiesInfo)
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
        result = GetEnumByParameters(parameterRow, propertiesInfo);
      else if (parameterRow.PropertyType == Constants.Module.CustomType.Navigation)
        result = GetEntityByParameters(parameterRow);
      
      return result;
    }
    
    /// <summary>
    /// Получить дату по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Дата.</returns>
    public virtual DateTime GetDateByParameters(IParametersMatchingParameters parameterRow)
    {
      DateTime date = Calendar.Today;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.FixedValue && Calendar.TryParseDate(parameterRow.ChosenValue, out date))
        return date;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Date.Period)
        date = Functions.Module.GenerateDateByPeriod(parameterRow.ValueFrom, parameterRow.ValueTo);
      
      return date;
    }
    
    /// <summary>
    /// Получить логическое значение по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Логическое значение.</returns>
    public virtual bool GetBoolByParameters(IParametersMatchingParameters parameterRow)
    {
      bool logic = false;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.FixedValue && bool.TryParse(parameterRow.ChosenValue, out logic))
        return logic;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.RandomValue)
        logic = Functions.Module.GenerateRandomBool();
      
      return logic;
    }
    
    /// <summary>
    /// Получить число по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Число.</returns>
    public virtual int GetIntByParameters(IParametersMatchingParameters parameterRow)
    {
      int num = 0;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.FixedValue && int.TryParse(parameterRow.ChosenValue, out num))
        return num;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Numeric.NumberWithLength)
        num = Functions.Module.GenerateNumberWithLength(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.FillOptions.Numeric.NumberRange)
        num = Functions.Module.GenerateNumberByRange(parameterRow.ValueFrom, parameterRow.ValueTo);
      
      return num;
    }
    
    /// <summary>
    /// Получить строку по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Строка.</returns>
    public virtual string GetStringByParameters(IParametersMatchingParameters parameterRow)
    {
      var str = string.Empty;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.FixedValue)
        str = parameterRow.ChosenValue;
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.RandomString)
        str = Functions.Module.GenerateStringSentence();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.Paragraph)
        str = Functions.Module.GenerateStringParagraph();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.RandomPhone)
        str = Functions.Module.GenerateStringPhone();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.NumberInStr)
        str = Functions.Module.GenerateStringNumber();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.FirstName)
        str = Functions.Module.GenerateStringFirstName(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.LastName)
        str = Functions.Module.GenerateStringLastName(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.FullName)
        str = Functions.Module.GenerateStringFullName(parameterRow.ChosenValue);
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.JobTitle)
        str = Functions.Module.GenerateStringJobTitle();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.Email)
        str = Functions.Module.GenerateStringEmail();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.Login)
        str = Functions.Module.GenerateStringLogin();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.State)
        str = Functions.Module.GenerateStringState();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.City)
        str = Functions.Module.GenerateStringCity();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.Street)
        str = Functions.Module.GenerateStringStreet();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.Department)
        str = Functions.Module.GenerateStringDepartment();
      else if (parameterRow.FillOption == Constants.Module.FillOptions.String.CompanyName)
        str = Functions.Module.GenerateStringCompanyName();
      
      return str;
    }
    
    /// <summary>
    /// Получить перечисление по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Перечисление.</returns>
    public virtual Enumeration? GetEnumByParameters(IParametersMatchingParameters parameterRow, List<starkov.Faker.Structures.Module.PropertyInfo> propertiesInfo)
    {
      Enumeration? newEnum = null;
      var enumValues = propertiesInfo.FirstOrDefault(i => i.Name == parameterRow.PropertyName).EnumCollection;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.FixedValue)
        newEnum = new Enumeration(enumValues.Where(i => i.LocalizedName == parameterRow.ChosenValue).Select(i => i.Name).FirstOrDefault());
      else if (parameterRow.FillOption == Constants.Module.FillOptions.Common.RandomValue)
        newEnum = Functions.Module.PickRandomEnumeration(enumValues.Select(i => i.Name).ToList());
      
      return newEnum;
    }
    
    /// <summary>
    /// Получить сущность по параметрам.
    /// </summary>
    /// <param name="parameterRow">Строка с параметрами.</param>
    /// <returns>Сущность.</returns>
    public virtual IEntity GetEntityByParameters(IParametersMatchingParameters parameterRow)
    {
      int num;
      IEntity entity = null;
      var databook = parameterRow.ParametersMatching;
      
      if (parameterRow.FillOption == Constants.Module.FillOptions.Common.FixedValue)
      {
        var idInString = parameterRow.ChosenValue;
        var regex = new Regex(@"\(\d*\)$");
        var matches = regex.Matches(parameterRow.ChosenValue);
        if (matches.Count > 0)
        {
          foreach (Match match in matches)
            idInString = match.Value.Substring(1, match.Value.Length-2);
        }
        
        if (int.TryParse(idInString, out num))
          entity = GetEntityByTypeGuidAndId(parameterRow.PropertyTypeGuid, num);
      }
      else if (parameterRow.FillOption == Constants.Module.FillOptions.Common.RandomValue)
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
    /// Запустить АО для генерации сущностей.
    /// </summary>
    /// <param name="count">Кол-во создаваемых записей.</param>
    /// <param name="databookId">ИД справочника по которому будет происходить генерация.</param>
    [Remote]
    public virtual void CreateAsyncForGenerateEntities(int count, int databookId)
    {
      var asyncHandler = Faker.AsyncHandlers.EntitiesGeneration.Create();
      asyncHandler.Count = count;
      asyncHandler.DatabookId = databookId;
      asyncHandler.ExecuteAsync(starkov.Faker.Resources.AsyncEndWorkMessage);
    }
    
    /// <summary>
    /// Получить список с информацией о реквизитах типа сущности.
    /// </summary>
    /// <param name="guid">Guid типа сущности.</param>
    /// <returns>Список с информацией о реквизитах типа сущности.</returns>
    [Remote]
    public virtual List<Structures.Module.PropertyInfo> GetPropertiesType(string guid)
    {
      var propertiesList = new List<Structures.Module.PropertyInfo>();
      
      var typeGuid = Guid.Parse(guid);
      var type = TypeExtension.GetTypeByGuid(typeGuid);
      if (type == null)
        return propertiesList;
      
      var typeMetadata = type.GetFinalType().GetEntityMetadata();
      IEntity entity = null;
      AccessRights.AllowRead(() =>
                             {
                               entity = CreateEntityByTypeGuid(typeMetadata.NameGuid.ToString());
                             });
      
      var excludeProperties = Functions.Module.GetExcludeProperties();
      var excludePropertyTypes = Functions.Module.GetExcludePropertyTypes();
      var properties = typeMetadata.Properties.Where(m => !excludeProperties.Contains(m.Name))
        .Where(m => !excludePropertyTypes.Contains(m.PropertyType));
      
      //Учетные записи
      if (guid == Constants.Module.Guids.Login)
      {
        var additionalExcludeProps = Functions.Module.GetAdditionalExcludePropForLogins();
        properties = properties.Where(m => !additionalExcludeProps.Contains(m.Name));
        
        propertiesList.Add(Structures.Module.PropertyInfo.Create(Constants.Module.PropertyNames.Password,
                                                                 starkov.Faker.Resources.LocalizedPassword,
                                                                 Constants.Module.CustomType.String,
                                                                 string.Empty,
                                                                 true,
                                                                 new List<Structures.Module.EnumerationInfo>(),
                                                                 null));
      }
      
      foreach (var propertyMetadata in properties)
      {
        #region Получение локализованных значений перечислений
        
        var enumInfo = new List<Structures.Module.EnumerationInfo>();
        if (propertyMetadata is Sungero.Metadata.EnumPropertyMetadata)
        {
          var infoProperties = entity.Info.Properties;
          var propertyEnumeration = infoProperties.GetType().GetProperty(propertyMetadata.Name);
          if (propertyEnumeration != null)
          {
            var enumPropertyInfo = propertyEnumeration.GetValue(infoProperties) as Sungero.Domain.Shared.EnumPropertyInfo;
            foreach (string val in (propertyMetadata as Sungero.Metadata.EnumPropertyMetadata).Values.Select(m => m.Name))
              enumInfo.Add(Structures.Module.EnumerationInfo.Create(val, enumPropertyInfo.GetLocalizedValue(new Enumeration(val))));
          }
        }
        
        #endregion
        
        propertiesList.Add(Structures.Module.PropertyInfo.Create(propertyMetadata.Name,
                                                                 propertyMetadata.GetLocalizedName().ToString(),
                                                                 propertyMetadata.PropertyType.ToString(),
                                                                 (propertyMetadata as Sungero.Metadata.NavigationPropertyMetadata)?.EntityGuid.ToString() ?? string.Empty,
                                                                 propertyMetadata.IsRequired,
                                                                 enumInfo,
                                                                 (propertyMetadata as Sungero.Metadata.StringPropertyMetadata)?.Length));
      }
      
      using (var session = new Sungero.Domain.Session())
      {
        session.Delete(entity);
      }
      
      return propertiesList;
    }
    
    #endregion
    
    #region Работа с сущностями
    
    /// <summary>
    /// Получить список всех сущностей по Guid типа.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <returns>Список сущностей.</returns>
    [Remote(IsPure = true)]
    public virtual IQueryable<IEntity> GetEntitiesByTypeGuid(string typeGuid)
    {
      return GetEntitiesByTypeGuid(typeGuid, string.Empty);
    }
    
    /// <summary>
    /// Получить список сущностей по Guid типа.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <param name="documentTypeGuid">Guid типа документа.</param>
    /// <returns>Список сущностей.</returns>
    [Remote(IsPure = true)]
    public virtual IQueryable<IEntity> GetEntitiesByTypeGuid(string typeGuid, string documentTypeGuid)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(typeGuid));
      using (var session = new Sungero.Domain.Session())
      {
        if (entityType.Name == "IDocumentKind" && !string.IsNullOrEmpty(documentTypeGuid))
          return session.GetAll(entityType).Where(ent => Sungero.Docflow.DocumentKinds.As(ent).DocumentType.DocumentTypeGuid == documentTypeGuid);
        else
          return session.GetAll(entityType);
      }
    }
    
    /// <summary>
    /// Получить сущность по Guid типа и id.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <param name="id">ИД сущности.</param>
    /// <returns>Cущность.</returns>
    [Remote(IsPure = true, PackResultEntityEagerly = true)]
    public virtual IEntity GetEntityByTypeGuidAndId(string typeGuid, int id)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(typeGuid));
      using (var session = new Sungero.Domain.Session())
      {
        return session.GetAll(entityType).FirstOrDefault(ent => ent.Id == id);
      }
    }
    
    /// <summary>
    /// Создать сущность по Guid типа.
    /// </summary>
    /// <param name="typeGuid">Guid типа сущности.</param>
    /// <returns>Сущность.</returns>
    [Remote(PackResultEntityEagerly = true)]
    public virtual IEntity CreateEntityByTypeGuid(string typeGuid)
    {
      var entityType = Sungero.Domain.Shared.TypeExtension.GetTypeByGuid(Guid.Parse(typeGuid));
      using (var session = new Sungero.Domain.Session())
      {
        return session.Create(entityType);
      }
    }
    
    /// <summary>
    /// Получить список всех не использующихся учетных записей.
    /// </summary>
    /// <returns>Список учетных записей.</returns>
    [Remote(IsPure = true)]
    public virtual IQueryable<ILogin> GetAllUnusedLogins()
    {
      var usedLoginsId = Sungero.Company.PublicFunctions.Employee.Remote.GetEmployees()
        .Where(emp => emp.Login != null)
        .Select(emp => emp.Login.Id);
      var systemLogins = new List<string>() { "Administrator", "Integration Service", "Service User", "Adviser" };
      
      return Sungero.CoreEntities.Logins.GetAll(l => !usedLoginsId.Contains(l.Id))
        .Where(l => !systemLogins.Contains(l.LoginName));
    }
    
    #endregion
  }
}