using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using System;
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
        public MatrixElement(ExtensibleElement element)
        {
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
        public static Solid GetSolidOfElement(Element element)
        {
            Solid theSolid = null;
            GeometryElement geometryElement;
            if (element.GetType() == typeof(FamilyInstance))
            {
                FamilyInstance instance = element as FamilyInstance;
                geometryElement = instance.get_Geometry(new Options() { IncludeNonVisibleObjects = false, DetailLevel = ViewDetailLevel.Undefined, ComputeReferences = false });
            }
            else
            {
                geometryElement = element.get_Geometry(new Options() { IncludeNonVisibleObjects = false, DetailLevel = ViewDetailLevel.Undefined, ComputeReferences = true });
            }
            foreach (GeometryObject obj in geometryElement)
            {
                try
                {
                    if (obj.GetType() == typeof(Solid))
                    {
                        Solid solid = obj as Solid;
                        if (theSolid == null)
                        {
                            theSolid = solid;
                            break;
                        }
                        else
                        {
                            if (solid.Volume > theSolid.Volume || theSolid == null)
                            {
                                theSolid = solid;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e) { PrintError(e); }
            }
            if (theSolid == null)
            {
                foreach (GeometryObject obj in geometryElement)
                {
                    GeometryInstance geoInst = obj as GeometryInstance;
                    if (geoInst != null)
                    {
                        GeometryElement geoElemTmp = geoInst.GetInstanceGeometry();
                        foreach (GeometryObject geomObjTmp in geoElemTmp)
                        {
                            if (geomObjTmp.GetType() == typeof(Solid))
                            {
                                Solid solidObj2 = geomObjTmp as Solid;
                                if (theSolid == null && solidObj2.Faces.Size > 0)
                                {
                                    theSolid = solidObj2;
                                    break;
                                }
                                else
                                {
                                    if (theSolid != null)
                                    {
                                        if ((solidObj2.Volume > theSolid.Volume || theSolid == null) && solidObj2.Faces.Size > 0)
                                        {
                                            theSolid = solidObj2;
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return theSolid;
        }
        public MatrixElement(Element element)
        {
            Solid = GetSolidOfElement(element);
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
