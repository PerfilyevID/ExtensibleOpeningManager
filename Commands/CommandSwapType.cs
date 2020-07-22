using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandSwapType : IExecutableCommand
    {
        public CommandSwapType(ExtensibleElement instance)
        {
            Element = instance;
        }
        private ExtensibleElement Element {get;}
        public Result Execute(UIApplication app)
        {
            Element.SwapType();
            return Result.Succeeded;
        }
    }
}
