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
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandZoomElement : IExecutableCommand
    {
        public CommandZoomElement(SE_LinkedWall wall)
        {
            Solid = wall.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = wall.BoundingBox.Min + new XYZ(-5, -5, -2);
            box.Max = wall.BoundingBox.Max + new XYZ(5, 5, 1);
            Box = box;
        }
        public CommandZoomElement(ExtensibleElement element)
        {
            Solid = element.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid() + new XYZ(-5, -5, -2);
            box.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid() + new XYZ(5, 5, 1);
            Box = box;
        }
        public CommandZoomElement(ExtensibleSubElement subElement)
        {
            Solid = subElement.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid() + new XYZ(-5, -5, -2);
            box.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid() + new XYZ(5, 5, 1);
            Box = box;
        }
        private Solid Solid { get; }
        private BoundingBoxXYZ Box { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                UiTools.ZoomElement(Box, Solid.ComputeCentroid(), app.ActiveUIDocument);
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }
        }
    }
}
