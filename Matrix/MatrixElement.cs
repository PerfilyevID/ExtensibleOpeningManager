using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using System;
using System.IO;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Matrix
{
    public class MatrixElement
    {
        public Element Element { get; }
        public Solid Solid { get; }
        public XYZ Centroid { get; }
        public BoundingBoxXYZ BoundingBox { get; }
        public Solid IntersectionSolid { get; set; }
        public object Object { get; set; }
        public MatrixElement(ExtensibleElement element)
        {
            Object = element;
            Solid = element.Solid;
            Element = element.Instance;
            Centroid = Solid.ComputeCentroid();
            BoundingBox = new BoundingBoxXYZ();
            BoundingBoxXYZ elementSolid = Solid.GetBoundingBox();
            XYZ min = elementSolid.Min + Centroid;
            XYZ max = elementSolid.Max + Centroid;
            BoundingBox.Min = min;
            BoundingBox.Max = max;
        }
        public MatrixElement(ExtensibleSubElement element)
        {
            Object = element;
            Solid = element.Solid;
            Element = element.Element;
            Centroid = Solid.ComputeCentroid();
            BoundingBox = new BoundingBoxXYZ();
            BoundingBoxXYZ elementSolid = Solid.GetBoundingBox();
            XYZ min = elementSolid.Min + Centroid;
            XYZ max = elementSolid.Max + Centroid;
            BoundingBox.Min = min;
            BoundingBox.Max = max;
        }
        public MatrixElement(SE_LinkedWall element)
        {
            Object = element;
            Solid = element.Solid;
            Element = element.Wall;
            Centroid = Solid.ComputeCentroid();
            BoundingBox = new BoundingBoxXYZ();
            BoundingBoxXYZ elementSolid = Solid.GetBoundingBox();
            XYZ min = elementSolid.Min + Centroid;
            XYZ max = elementSolid.Max + Centroid;
            BoundingBox.Min = min;
            BoundingBox.Max = max;
        }
        public MatrixElement(Element element)
        {
            Solid = GeometryTools.GetSolidOfElement(element, ViewDetailLevel.Medium);
            Element = element;
            Centroid = Solid.ComputeCentroid();
            BoundingBox = new BoundingBoxXYZ();
            BoundingBoxXYZ elementSolid = Solid.GetBoundingBox();
            XYZ min = elementSolid.Min + Centroid;
            XYZ max = elementSolid.Max + Centroid;
            BoundingBox.Min = min;
            BoundingBox.Max = max;
        }
    }
}
