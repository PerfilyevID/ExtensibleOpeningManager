using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandZoomElement : IExecutableCommand
    {
        public CommandZoomElement(SE_LinkedWall wall)
        {
            Solid = wall.Solid;
            Box = wall.BoundingBox;
        }
        public CommandZoomElement(ExtensibleElement element)
        {
            Solid = element.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid();
            box.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid();
            Box = box;
        }
        public CommandZoomElement(ExtensibleSubElement subElement)
        {
            Solid = subElement.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid();
            box.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid();
            Box = box;
        }
        private Solid Solid { get; }
        private BoundingBoxXYZ Box { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                if (UiTools.Get3DView(app.ActiveUIDocument) == null)
                { UiTools.Create3DView(app.ActiveUIDocument.Document); }
                if (UiTools.Get3DView(app.ActiveUIDocument) != null)
                {
                    UiTools.ActivateView(UiTools.Get3DView(app.ActiveUIDocument), app.ActiveUIDocument);
                }
                UiTools.ZoomElement(element, uiapp.ActiveUIDocument);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
