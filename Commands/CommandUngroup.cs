using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System.Collections.Generic;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandUngroup : IExecutableCommand
    {
        public CommandUngroup(ExtensibleElement instance)
        {
            Element = instance;
            Wall = instance.Wall;
        }
        private ExtensibleElement Element { get; }
        private SE_LinkedWall Wall { get; }
        public Result Execute(UIApplication app)
        {
            List<ExtensibleSubElement> SubElements = Element.SubElements;
            Element.Remove();
            app.ActiveUIDocument.Document.Regenerate();
            foreach (ExtensibleSubElement subElement in SubElements)
            {
                ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(Element.Wall, subElement, app.ActiveUIDocument.Document));
                element.Instance.LookupParameter(Variables.parameter_offset_bounds).Set(UserPreferences.DefaultOffset / 304.8);
                element.SetWall(Wall);
                element.AddSubElement(subElement);
                element.AddComment(Variables.msg_created);
                element.Approve();
                element.Reject();
            }
            return Result.Succeeded;
        }
    }
}
