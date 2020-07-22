using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandReset : IExecutableCommand
    {
        public CommandReset(ExtensibleElement element)
        {
            Instance = element;
        }
        private ExtensibleElement Instance { get; }
        public Result Execute(UIApplication app)
        {
            Instance.Reset();
            return Result.Succeeded;
        }
    }
}
