using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandAddSubElement : IExecutableCommand
    {
        public CommandAddSubElement(ExtensibleElement instance)
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
                    Element.AddSubElement(UiTools.PickInstance(app, Collections.PickTypeOptions.Element, Collections.PickOptions.Local));
                    UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OnManualElementChanged(Element.Id);
                    ModuleData.CommandQueue.Enqueue(new CommandSetSelection(Element.Instance));
                    return Result.Succeeded;
                }
                else
                {
                    Element.AddSubElement(UiTools.PickInstance(app, Collections.PickTypeOptions.Instance, Collections.PickOptions.References));
                    UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OnManualElementChanged(Element.Id);
                    ModuleData.CommandQueue.Enqueue(new CommandSetSelection(Element.Instance));
                    return Result.Succeeded;
                }
            }
            catch (Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }
        }
    }
}
