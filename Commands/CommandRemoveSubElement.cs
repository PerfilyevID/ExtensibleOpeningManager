using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

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
            Element.RemoveSubElement(SubElement);
            return Result.Succeeded;
        }
    }
}
