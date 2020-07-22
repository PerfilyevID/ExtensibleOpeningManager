using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandUpdate : IExecutableCommand
    {
        public CommandUpdate(ExtensibleElement instance)
        {
            Instance = instance;
        }
        private ExtensibleElement Instance { get; }
        public Result Execute(UIApplication app)
        {
            Instance.Update();
            return Result.Succeeded;
        }
    }
}
