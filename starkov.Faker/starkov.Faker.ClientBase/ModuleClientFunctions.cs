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
    /// Открыть настройки модуля.
    /// </summary>
    public virtual void OpenModuleSetup()
    {
      var databook = Functions.ModuleSetup.Remote.GetModuleSetup();
      if (databook == null)
      {
        Dialogs.ShowMessage(starkov.Faker.Resources.ErrorNotCreatedSetupDatabook, MessageType.Error);
        return;
      }
      
      databook.Show();
    }

    /// <summary>
    /// Вывод диалога для запуска генерации сущностей.
    /// </summary>
    public virtual void ShowDIalogForGenerateEntity()
    {
      var dialog = Dialogs.CreateInputDialog(starkov.Faker.Resources.DialogDataInput);
      
      #region Поля диалога
      var entityField = dialog.AddSelect(starkov.Faker.Resources.DialogFieldEntityToFill, true, ParametersMatchings.Null);
      var countField = dialog.AddInteger(starkov.Faker.Resources.DialogFieldEntityCreateCount, true);
      #endregion
      
      dialog.SetOnRefresh((arg) =>
                          {
                            if (countField.Value.HasValue && countField.Value <= 0)
                              arg.AddError(starkov.Faker.Resources.DialogError_RecordCountLessThanOne);
                          });
      
      if (dialog.Show() == DialogButtons.Ok)
      {
        Functions.Module.Remote.CreateAsyncForGenerateEntities(countField.Value.GetValueOrDefault(), Convert.ToInt32(entityField.Value.Id));
        Dialogs.ShowMessage(starkov.Faker.Resources.MessageEntityGenerationProcessStarted);
      }
    }

  }
}