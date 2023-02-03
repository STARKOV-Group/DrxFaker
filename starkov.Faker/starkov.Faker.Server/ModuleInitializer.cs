using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;
using Sungero.Domain.Shared;

namespace starkov.Faker.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      FillDatabookTypes();
      CreateModuleSetup();
    }
    
    /// <summary>
    /// Заполнить типы справочников.
    /// </summary>
    public static void FillDatabookTypes()
    {
      InitializationLogger.Debug("Init: Fill databook types");
      
      //Метаданные базового типа
      var databookEntryType = typeof(Sungero.CoreEntities.IDatabookEntry);
      var baseMetadata = databookEntryType.GetEntityMetadata();
      
      var databookTypes = DatabookTypes.GetAll();
      var typesGuidList = GetTypesGuid();
      foreach (var typeGuid in typesGuidList)
      {
        try
        {
          var entityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(typeGuid);
          //Если baseMetadata не является предком для entityMetadata или entityMetadata является абстрактным типом
          if (entityMetadata == null || !baseMetadata.IsAncestorFor(entityMetadata) || entityMetadata.IsAbstract)
            continue;
          
          var type = TypeExtension.GetTypeByGuid(typeGuid);
          var finalEntityMetadata = Sungero.Metadata.Services.MetadataSearcher.FindEntityMetadata(type.GetFinalType());
          
          var databookWithOldType = databookTypes.FirstOrDefault(t => t.DatabookTypeGuid == typeGuid.ToString());
          var databookWithFinalType = databookTypes.FirstOrDefault(t => t.DatabookTypeGuid == finalEntityMetadata.NameGuid.ToString());
          var databookWithAncestorType = databookTypes.FirstOrDefault(t => t.AncestorGuids.Any(anc => anc.Guid == typeGuid.ToString()));
          
          if (finalEntityMetadata == null || Equals(finalEntityMetadata.NameGuid, typeGuid))
          {
            if (databookWithAncestorType != null)
            {
              databookWithAncestorType.DatabookTypeGuid = typeGuid.ToString();
              databookWithAncestorType.AncestorGuids.Remove(databookWithAncestorType.AncestorGuids.FirstOrDefault(anc => anc.Guid == typeGuid.ToString()));
              databookWithAncestorType.Save();
            }
            else if (databookWithOldType == null)
            {
              var newType = DatabookTypes.Create();
              newType.Name = entityMetadata.GetDisplayName();
              newType.DatabookTypeGuid = typeGuid.ToString();
              newType.Save();
            }
            continue;
          }
          
          if (databookWithAncestorType != null && databookWithAncestorType.DatabookTypeGuid != finalEntityMetadata.NameGuid.ToString())
          {
            databookWithAncestorType.DatabookTypeGuid = finalEntityMetadata.NameGuid.ToString();
            databookWithAncestorType.Save();
          }
          else if (databookWithFinalType != null && !databookWithFinalType.AncestorGuids.Any(anc => Equals(anc.Guid, typeGuid.ToString())))
          {
            databookWithFinalType.AncestorGuids.AddNew().Guid = typeGuid.ToString();
            databookWithFinalType.Save();
          }
          else if (databookWithOldType != null)
          {
            databookWithOldType.DatabookTypeGuid = finalEntityMetadata.NameGuid.ToString();
            databookWithOldType.AncestorGuids.AddNew().Guid = typeGuid.ToString();
            databookWithOldType.Save();
          }
          else if (databookWithAncestorType == null && databookWithFinalType == null && databookWithOldType == null)
          {
            var newType = DatabookTypes.Create();
            newType.Name = finalEntityMetadata.GetDisplayName();
            newType.DatabookTypeGuid = finalEntityMetadata.NameGuid.ToString();
            newType.AncestorGuids.AddNew().Guid = typeGuid.ToString();
            newType.Save();
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("Faker init fill databook types error: {0}", ex.Message);
        }
      }
    }
    
    /// <summary>
    /// Получить список Guid-ов из таблицы sungero_system_entitytype.
    /// </summary>
    /// <returns>Guid всех объектов системы, за исключением переданных в параметре.</returns>
    public static List<Guid> GetTypesGuid()
    {
      var notSelectedGuids = GetNotSelectGuids();
      
      var typesList = new List<Guid>();
      using (var command = SQL.GetCurrentConnection().CreateCommand())
      {
        command.CommandText = string.Format(Queries.Module.SelectTypeGuids, string.Join("','", notSelectedGuids));
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
            typesList.Add(reader.GetGuid(0));
        }
      }
      
      return typesList;
    }
    
    /// <summary>
    /// Получить список Guid-ов объектов исключаемых из заполнения.
    /// </summary>
    /// <returns>Список Guid.</returns>
    public static List<string> GetNotSelectGuids()
    {
      return new List<string>
      {
        "a6de7bb6-8c2d-425a-b35a-1d60119ecfa8", //Активный пользователь
        "668418c4-bd08-4aeb-94d7-d0c30869c1a0", //Бинарный образ документа
        "f5061291-4ac6-428f-b091-d53acdbe9ae5", //Группа документов
        "218ac7df-9b29-400a-b3fc-c2e22927f638", //Действие по отправке документа
        "1e9415ec-6ba8-46b5-b864-94b4385ffb52", //Пакет бинарных образов документов
        "91fcb864-f2ee-43d7-854b-23a3dcce65cb", //Приложение-обработчик
        "32ea0857-adf7-41c2-bc0c-188320e40786", //Результат распознавания сущности
        "2b84ed67-1ef8-4132-9ca1-a1d5d2f9f406", //Тип объекта
        "0896aa80-e1da-4a1e-9485-d172f8e242bc", //Тип документа
        "34d8054f-43c0-4c6a-a1a6-7ee427d65dc8", //Тип прав
        "effcb90e-8151-43f3-8139-661aa5bc6885", //Тип файлов
        "56c16259-fa13-4474-98fc-3f517cc4ed02", //Фоновый процесс
        "1cafe357-1b39-4fcc-a703-9607b958f5ef", //Хранилище
        "96d026d3-0224-4cc8-b633-67a30679b1e5", //Часовой пояс
        "de2707a2-2a1f-41cb-98ef-e6e17449bea8", //Частный календарь рабочего времени
        "de73a02c-c1bf-4edf-bee4-bf2705d282b8", //Элемент очереди выдачи прав на документы
        "5ec13d1f-de94-43a3-a51a-1bef325d9dad", //Элемент очереди выдачи прав на проект и папки проекта
        "aa042ddf-a9fb-4dea-883c-d0024b9574da", //Элемент очереди выдачи прав на проектные документы
        "b7edf323-816d-4213-abca-6ee7da1c03bd", //Элемент очереди выдачи прав участникам проекта
        "2d30e2aa-1d0b-45f0-8e4d-00318b3a5cfd", //Элемент очереди конвертации тел документов
        "50a0d7aa-1f04-4e4a-8f0c-044e0ba99949", //Элемент очереди синхронизации контрагентов
        "f9a3ec37-0fd4-4343-a295-9394ba830a0e"  //Элемент очереди синхронизации сообщений
      };
    }
    
    /// <summary>
    /// Создать запись справочника Настройка модуля.
    /// </summary>
    public static void CreateModuleSetup()
    {
      if (Faker.ModuleSetups.GetAll().Any())
        return;
      
      var databook = Faker.ModuleSetups.Create();
      databook.Name = starkov.Faker.Resources.ModuleSetupsDatabookName;
      databook.LoginNamesNumber = 30;
      databook.AttachmentsNumber = 30;
      databook.Save();
    }
  }

}
