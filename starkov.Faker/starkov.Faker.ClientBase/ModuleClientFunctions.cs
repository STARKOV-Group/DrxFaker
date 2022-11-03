using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Shared;
using System.Threading;
using System.Threading.Tasks;
using Sungero.RecordManagement;
using Sungero.Docflow;

namespace starkov.Faker.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Вывод диалога для запуска генерации сущностей
    /// </summary>
    public virtual void ShowDIalogForGenerateEntity()
    {
      var dialog = Dialogs.CreateInputDialog("Ввод данных");
      
      #region Поля диалога
      var entityField = dialog.AddSelect("Сущность для заполнения", true, ParametersMatchings.Null);
      var countField = dialog.AddInteger("Количество создаваемых записей", true);
      #endregion
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        Functions.Module.Remote.CreateAsyncForGenerateEntities(countField.Value.GetValueOrDefault(), entityField.Value.Id);
        Dialogs.ShowMessage("Процесс по генерации сущностей успешно запущен");
      }
    }

  }
}