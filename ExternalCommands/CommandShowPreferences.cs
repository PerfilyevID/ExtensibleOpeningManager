using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Forms;

namespace ExtensibleOpeningManager.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CommandShowPreferences : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Preferences PreferencesForm = new Preferences();
            PreferencesForm.ShowDialog();
            return Result.Succeeded;
        }
    }
}
