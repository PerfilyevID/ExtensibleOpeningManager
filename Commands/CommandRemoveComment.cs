using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandRemoveComment : IExecutableCommand
    {
        public CommandRemoveComment(ExtensibleElement element, ExtensibleComment comment)
        {
            Element = element;
            Comment = comment;
        }
        ExtensibleElement Element { get; }
        ExtensibleComment Comment { get; }
        public Result Execute(UIApplication app)
        {
            Element.RemoveComment(Comment);
            return Result.Succeeded;
        }
    }
}
