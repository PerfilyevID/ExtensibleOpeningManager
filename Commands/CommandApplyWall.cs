using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandApplyWall : IExecutableCommand
    {
        public CommandApplyWall(ExtensibleElement instance)
        {
            Element = instance;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            Element.ApplyWall();
            return Result.Succeeded;
        }
    }
}
