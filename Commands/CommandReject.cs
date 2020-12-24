using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandReject : IExecutableCommand
    {
        public CommandReject(ExtensibleElement element)
        {
            Element = element;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            Element.Reject();
            //Element.AddComment(Variables.msg_rejected);
            return Result.Succeeded;
        }
    }
}
