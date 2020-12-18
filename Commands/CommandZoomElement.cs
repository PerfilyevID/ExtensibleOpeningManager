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
            try { Id = wall.Wall.Id; }
            catch (Exception) { Id = null; }
            Solid = wall.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = wall.BoundingBox.Min + new XYZ(-5, -5, -2);
            box.Max = wall.BoundingBox.Max + new XYZ(5, 5, 1);
            Box = box;
        }
        public CommandZoomElement(ExtensibleElement element)
        {
            try { Id = element.Instance.Id; }
            catch (Exception) { Id = null; }
            Solid = element.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid() + new XYZ(-5, -5, -2);
            box.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid() + new XYZ(5, 5, 1);
            Box = box;
        }
        public CommandZoomElement(ExtensibleSubElement subElement)
        {
            try { Id = subElement.Element.Id; }
            catch (Exception) { Id = null; }
            Solid = subElement.Solid;
            BoundingBoxXYZ box = new BoundingBoxXYZ();
            box.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid() + new XYZ(-5, -5, -2);
            box.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid() + new XYZ(5, 5, 1);
            Box = box;
        }
        private ElementId Id { get; }
        private Solid Solid { get; }
        private BoundingBoxXYZ Box { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                UiTools.ZoomElement(Box, Solid.ComputeCentroid(), app.ActiveUIDocument);
                try
                {
                    if (Id != null) { app.ActiveUIDocument.Selection.SetElementIds(new List<ElementId>() { Id }); }
                }
                catch (Exception) { }
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
