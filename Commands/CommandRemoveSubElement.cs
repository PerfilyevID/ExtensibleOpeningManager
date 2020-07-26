using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;
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
