using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Tools
{
    public static class GeometryTools
    {
        public static Solid GetCorrectSolid(Wall wall, Transform transform = null)
        {
            List<Solid> solids = new List<Solid>();
            IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
            Element e2 = wall.Document.GetElement(sideFaces[0]);
            Face face = e2.GetGeometryObjectFromReference(sideFaces[0]) as Face;
            XYZ normal = face.ComputeNormal(new UV(0.5, 0.5));
            IList<CurveLoop> loops = face.GetEdgesAsCurveLoops();
            foreach (CurveLoop loop in loops)
            {
                List<CurveLoop> gloops = new List<CurveLoop>();
                gloops.Add(loop);
                IList<CurveLoop> iloops = gloops;
                if (transform != null)
                {
                    solids.Add(SolidUtils.CreateTransformed(GeometryCreationUtilities.CreateExtrusionGeometry(iloops, normal.Negate(), wall.Width, new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId)), transform));
                }
                else
                {
                    solids.Add(GeometryCreationUtilities.CreateExtrusionGeometry(iloops, normal.Negate(), wall.Width, new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId)));
                }
            }
            Solid solid = solids[0];
            foreach (Solid s in solids)
            {
                solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, s, BooleanOperationsType.Union);
            }
            return solid;
        }
        public static bool SolidIsValid(Solid s)
        {
            try
            {
                XYZ pt = s.ComputeCentroid();
                if (pt == null) { return false; }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static Solid GetByGeometryElement(GeometryElement geometryElement)
        {
            Solid theSolid = null;
            foreach (GeometryObject obj in geometryElement)
            {
                try
                {
                    if (obj.GetType() == typeof(Solid))
                    {
                        Solid solid = obj as Solid;
                        if (!SolidIsValid(theSolid))
                        {
                            theSolid = solid;
                            break;
                        }
                        else
                        {
                            if (solid.Volume > theSolid.Volume || !SolidIsValid(theSolid))
                            {
                                theSolid = solid;
                                break;
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
            if (!SolidIsValid(theSolid))
            {
                foreach (GeometryObject obj in geometryElement)
                {
                    try
                    {
                        GeometryInstance geoInst = obj as GeometryInstance;
                        if (geoInst != null)
                        {
                            GeometryElement geoElemTmp = geoInst.GetInstanceGeometry();
                            foreach (GeometryObject geomObjTmp in geoElemTmp)
                            {
                                try
                                {
                                    if (geomObjTmp.GetType() == typeof(Solid))
                                    {
                                        Solid solidObj2 = geomObjTmp as Solid;
                                        if (!SolidIsValid(theSolid) && solidObj2.Faces.Size > 0)
                                        {
                                            theSolid = solidObj2;
                                            break;
                                        }
                                        else
                                        {
                                            if (!SolidIsValid(theSolid))
                                            {
                                                if ((solidObj2.Volume > theSolid.Volume || !SolidIsValid(theSolid)) && solidObj2.Faces.Size > 0)
                                                {
                                                    theSolid = solidObj2;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            return theSolid;
        }
        public static Solid GetSolidOfElement(Element element)
        {
            Solid theSolid = null;
            GeometryElement geometryElement;
            try
            {
                FamilyInstance instance = element as FamilyInstance;
                geometryElement = instance.get_Geometry(new Options() { IncludeNonVisibleObjects = true, DetailLevel = ViewDetailLevel.Coarse, ComputeReferences = true });
                theSolid = GetByGeometryElement(geometryElement);
                if (theSolid != null) { return theSolid; }
            }
            catch (Exception) { }
            try
            {
                geometryElement = element.get_Geometry(new Options() { IncludeNonVisibleObjects = true, DetailLevel = ViewDetailLevel.Coarse, ComputeReferences = true });
                theSolid = GetByGeometryElement(geometryElement);
                if (theSolid != null) { return theSolid; }
            }
            catch (Exception) { }

            return theSolid;
        }
    }
}
