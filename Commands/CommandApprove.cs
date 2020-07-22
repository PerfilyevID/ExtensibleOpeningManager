using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandApprove : IExecutableCommand
    {
        public CommandApprove(ExtensibleElement element)
        {
            Element = element;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            Element.Apply();
            return Result.Succeeded;
        }
    }
}
