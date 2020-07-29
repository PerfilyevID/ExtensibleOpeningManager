using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using KPLN_Loader.Common;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandRemoveSubElement : IExecutableCommand
    {
        public CommandRemoveSubElement(FamilyInstance instance, ExtensibleSubElement element)
        {
            Element = ExtensibleElement.GetExtensibleElementByInstance(instance);
            SubElement = element;
        }
        private ExtensibleElement Element { get; }
        private ExtensibleSubElement SubElement { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                Element.RemoveSubElement(SubElement);
                UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OnManualElementChanged(Element.Id);
                ModuleData.CommandQueue.Enqueue(new CommandSetSelection());
                return Result.Succeeded;
            }
            catch (System.Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }

        }
    }
}
