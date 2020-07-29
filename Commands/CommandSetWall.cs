using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandSetWall : IExecutableCommand
    {
        public CommandSetWall(ExtensibleElement instance)
        {
            Element = instance;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                if (UserPreferences.Department == Collections.Department.MEP)
                {
                    Element.SetWall(UiTools.PickWall(app, Collections.PickOptions.References));
                    UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OnManualElementChanged(Element.Id);
                    return Result.Succeeded;
                }
                else
                {
                    Element.SetWall(UiTools.PickWall(app, Collections.PickOptions.Local));
                    UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OnManualElementChanged(Element.Id);
                    return Result.Succeeded;
                }
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
