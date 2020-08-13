using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandGroupInstances : IExecutableCommand
    {
        public CommandGroupInstances(List<ExtensibleElement> instances)
        {
            ExtensibleElements = instances;
        }
        private List<ExtensibleElement> ExtensibleElements { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                SE_LinkedWall wall = ExtensibleElements[0].Wall;
                if (CreationTools.HasInvalidIntersections(ExtensibleElements, wall)) {return Result.Failed; }
                List<ExtensibleSubElement> subElements = new List<ExtensibleSubElement>();
                foreach (ExtensibleElement e in ExtensibleElements)
                {
                    foreach (ExtensibleSubElement s in e.SubElements)
                    {
                        subElements.Add(s);
                    }
                    e.Remove();
                }
                app.ActiveUIDocument.Document.Regenerate();
                ExtensibleElement groupedElement = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.GroupInstance(ExtensibleElements[0].Wall, ExtensibleElements, app.ActiveUIDocument.Document));
                groupedElement.SetWall(wall);
                groupedElement.AddComment(Variables.msg_created);
                groupedElement.Approve(true);
                groupedElement.Reject();
                foreach (ExtensibleSubElement s in subElements)
                {
                    groupedElement.AddSubElement(s);
                }
                groupedElement.Approve(false);
                groupedElement.Reject();
                groupedElement.Instance.LookupParameter(Variables.parameter_offset_bounds).Set(UserPreferences.DefaultOffset / 304.8);
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
