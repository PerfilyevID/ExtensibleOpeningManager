using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Tools.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Tools
{
    public static class GeometryTools
    {
        public static Solid GetCorrectSolid(Wall wall, Transform transform = null)
        {
            Solid solid;
            try
            {
                solid = GetOptimizedWallSolid(wall);
                if (solid != null)
                {
                    if (transform == null)
                    {
                        return solid;
                    }
                    else
                    {
                        return SolidUtils.CreateTransformed(solid, transform);
                    }
                }
            }
            catch (Exception) { }
            try
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
                solid = solids[0];
                solids.RemoveAt(0);
                foreach (Solid s in solids)
                {
                    try
                    {
                        solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, s, BooleanOperationsType.Union);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception)
            {
                List<Solid> solids = new List<Solid>();
                if (transform != null)
                {
                    foreach (GeometryObject obj in wall.get_Geometry(new Options()).GetTransformed(transform))
                    {
                        if (obj.GetType() == typeof(Solid))
                        {
                            solids.Add(obj as Solid);
                        }
                    }
                }
                else
                {
                    foreach (GeometryObject obj in wall.get_Geometry(new Options()))
                    {
                        if (obj.GetType() == typeof(Solid))
                        {
                            solids.Add(obj as Solid);
                        }
                    }
                }
                Solid so = solids[0];
                solids.RemoveAt(0);
                foreach (Solid s in solids)
                {
                    try
                    {
                        so = BooleanOperationsUtils.ExecuteBooleanOperation(so, s, BooleanOperationsType.Union);
                    }
                    catch (Exception) { }
                }
                solid = so;
            }
            if (solid == null)
            {
                List<Solid> solids = new List<Solid>();
                if (transform != null)
                {
                    foreach (GeometryObject obj in wall.get_Geometry(new Options()).GetTransformed(transform))
                    {
                        try
                        {
                            if (obj.GetType() == typeof(Solid))
                            {
                                if ((obj as Solid).Edges.Size != 0)
                                {
                                    solids.Add(obj as Solid);
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    foreach (GeometryObject obj in wall.get_Geometry(new Options()))
                    {
                        if (obj.GetType() == typeof(Solid))
                        {
                            solids.Add(obj as Solid);
                        }
                    }
                }
                Solid maxSolid = solids[0];
                Solid so = solids[0];
                solids.RemoveAt(0);
                foreach (Solid s in solids)
                {
                    try
                    {
                        so = BooleanOperationsUtils.ExecuteBooleanOperation(so, s, BooleanOperationsType.Union);
                        if (maxSolid.Volume < s.Volume)
                        {
                            maxSolid = s;
                        }
                    }
                    catch (Exception) { }
                }
                solid = so;
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
        public static void CreateShape(Document doc, Solid s)
        {
            List<GeometryObject> g = new List<GeometryObject>() { s };
            DirectShape shape = DirectShape.CreateElement(doc, new ElementId(-2000151));
            IList <GeometryObject> ig = g;
            shape.AppendShape(ig);
        }
        private static Solid GetByGeometryElement(GeometryElement geometryElement)
        {
            List<Solid> solids = new List<Solid>();
            foreach (GeometryObject obj in geometryElement)
            {
                try
                {
                    if (obj.GetType() == typeof(Solid))
                    {
                        Solid solid = obj as Solid;
                        if (SolidIsValid(solid))
                        {
                            solids.Add(solid);
                        }
                    }
                }
                catch (Exception) { }
                try
                {
                    GeometryInstance gInstance = obj as GeometryInstance;
                    if (gInstance != null)
                    {
                        GeometryElement gElement = gInstance.GetInstanceGeometry();
                        foreach (GeometryObject gObject in gElement)
                        {
                            try
                            {
                                if (gObject.GetType() == typeof(Solid))
                                {
                                    Solid solid = gObject as Solid;
                                    if (SolidIsValid(solid))
                                    {
                                        solids.Add(solid);
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
                catch (Exception) { }
            }
            solids = solids.OrderBy(x => x.Volume).ToList();
            solids.Reverse();
            Solid combinedSolid = solids[0];
            try
            {
                foreach (Solid s in solids)
                {
                    try
                    {
                        combinedSolid = BooleanOperationsUtils.ExecuteBooleanOperation(combinedSolid, s, BooleanOperationsType.Union);

                    }
                    catch (Exception) { }
                }
            }
            catch (Exception)
            {
                return solids[0];
            }
            return combinedSolid;
        }
        public static Solid GetSolidOfElement(Element element, ViewDetailLevel level)
        {
            Solid theSolid = null;
            GeometryElement geometryElement;
            try
            {
                FamilyInstance instance = element as FamilyInstance;
                geometryElement = instance.get_Geometry(new Options() { IncludeNonVisibleObjects = true, DetailLevel = level, ComputeReferences = true });
                theSolid = GetByGeometryElement(geometryElement);
                if (theSolid != null) { return theSolid; }
            }
            catch (Exception) { }
            try
            {
                geometryElement = element.get_Geometry(new Options() { IncludeNonVisibleObjects = true, DetailLevel = level, ComputeReferences = true });
                theSolid = GetByGeometryElement(geometryElement);
                if (theSolid != null) { return theSolid; }
            }
            catch (Exception) { }
            return theSolid;
        }
        public static Solid GetOptimizedWallSolid(Wall wall)
        {
            try
            {
                List<Solid> solids = new List<Solid>();
                IList<Reference> sideFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
                Element e2 = wall.Document.GetElement(sideFaces[0]);
                Face face = e2.GetGeometryObjectFromReference(sideFaces[0]) as Face;
                Surface surface = face.GetSurface();
                XYZ normal = wall.Orientation;
                XYZ NORMAL = normal;
                IList<CurveLoop> loops = face.GetEdgesAsCurveLoops();
                CurveLoop edgeLoop = null;
                IList<CurveLoop> iLoops = new List<CurveLoop>();
                foreach (CurveLoop loop in loops)
                {
                    if (loop.IsCounterclockwise(normal))
                    {
                        edgeLoop = loop;
                        List<Line> optimizedCurves = RemoveCounterClockwiseLines(edgeLoop, surface);
                        List<Line> saved = new List<Line>();
                        int count = 0;
                        while (optimizedCurves.Count != count)
                        {
                            saved = optimizedCurves;
                            try
                            {
                                optimizedCurves = RemoveCoherentLines(optimizedCurves);
                                optimizedCurves = JoinCurves(optimizedCurves);
                                optimizedCurves = RemoveCoherentLines(optimizedCurves);
                                optimizedCurves = RemoveCounterClockwiseLines(optimizedCurves, surface);
                                optimizedCurves = JoinCurves(optimizedCurves);
                                count = optimizedCurves.Count();
                            }
                            catch (Exception)
                            {
                                optimizedCurves = saved;
                                break;
                            }
                        }
                        CurveLoop resultLoop = new CurveLoop();
                        foreach (Curve c in optimizedCurves)
                        {
                            resultLoop.Append(c);
                        }
                        iLoops.Add(resultLoop);
                    }
                }
                return GeometryCreationUtilities.CreateExtrusionGeometry(iLoops, normal.Negate(), wall.Width, new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId));
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static List<Line> RemoveCounterClockwiseLines(List<Line> curveloop, Surface surface)
        {
            List<Line> list = new List<Line>();
            Curve previous;
            Curve current;
            Curve next;
            for (int i = 0; i < curveloop.Count(); i++)
            {
                if (i == 0)
                {
                    previous = curveloop.Last();
                    current = curveloop.ElementAt(0);
                    next = curveloop.ElementAt(1);
                    if (!CounterClockVise(previous, current, next, surface))
                    { list.Add(GetLine(current)); }
                }
                if (i == curveloop.Count() - 1)
                {
                    previous = curveloop.ElementAt(i - 1);
                    current = curveloop.ElementAt(i);
                    next = curveloop.ElementAt(0);
                    if (!CounterClockVise(previous, current, next, surface))
                    { list.Add(GetLine(current)); }
                }
                if (i != curveloop.Count() - 1 && i != 0)
                {
                    previous = curveloop.ElementAt(i - 1);
                    current = curveloop.ElementAt(i);
                    next = curveloop.ElementAt(i + 1);
                    if (!CounterClockVise(previous, current, next, surface))
                    { list.Add(GetLine(current)); }
                }
            }
            return list;
        }
        private static List<Line> RemoveCounterClockwiseLines(CurveLoop curveloop, Surface surface)
        {
            List<Line> list = new List<Line>();
            Curve previous;
            Curve current;
            Curve next;
            for (int i = 0; i < curveloop.Count(); i++)
            {
                if (i == 0)
                {
                    previous = curveloop.Last();
                    current = curveloop.ElementAt(0);
                    next = curveloop.ElementAt(1);
                    if (!CounterClockVise(previous, current, next, surface))
                    { list.Add(GetLine(current)); }
                }
                if (i == curveloop.Count() - 1)
                {
                    previous = curveloop.ElementAt(i - 1);
                    current = curveloop.ElementAt(i);
                    next = curveloop.ElementAt(0);
                    if (!CounterClockVise(previous, current, next, surface))
                    { list.Add(GetLine(current)); }
                }
                if (i != curveloop.Count() - 1 && i != 0)
                {
                    previous = curveloop.ElementAt(i - 1);
                    current = curveloop.ElementAt(i);
                    next = curveloop.ElementAt(i + 1);
                    if (!CounterClockVise(previous, current, next, surface))
                    { list.Add(GetLine(current)); }
                }
            }
            return list;
        }
        private static Line GetLine(Curve curve)
        {
            return Line.CreateBound(curve.GetEndPoint(0), curve.GetEndPoint(1));
        }
        private static List<Line> JoinCurves(List<Line> curves)
        {
            List<XYZ> points = new List<XYZ>();
            Line current;
            Line next;
            List<Line> curveLoop = new List<Line>();
            for (int i = 0; i < curves.Count; i++)
            {
                if (i == curves.Count - 1)
                {
                    current = curves.ElementAt(i);
                    next = curves.ElementAt(0);
                    try
                    {
                        points.Add(FindIntersection(current, next));
                    }
                    catch (Exception) { }
                }
                else
                {
                    current = curves.ElementAt(i);
                    next = curves.ElementAt(i + 1);
                    try
                    {
                        points.Add(FindIntersection(current, next));
                    }
                    catch (Exception) { }
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                if (i == curves.Count - 1)
                {
                    try
                    {
                        curveLoop.Add(Line.CreateBound(points[i], points[0]));
                    }
                    catch (Exception)
                    { }
                }
                else
                {
                    try
                    {
                        curveLoop.Add(Line.CreateBound(points[i], points[i + 1]));
                    }
                    catch (Exception) { }
                }
            }
            return curveLoop;
        }
        private static XYZ FindIntersection(Line curve_a, Line curve_b)
        {

            Line curve_A = Line.CreateBound(curve_a.GetEndPoint(0) + (curve_a as Line).Direction.Negate() * 200, curve_a.GetEndPoint(1) + (curve_a as Line).Direction * 200);
            Line curve_B = Line.CreateBound(curve_b.GetEndPoint(0) + (curve_b as Line).Direction.Negate() * 200, curve_b.GetEndPoint(1) + (curve_b as Line).Direction * 200);
            IntersectionResultArray result;
            curve_A.Intersect(curve_B, out result);
            try
            {
                foreach (IntersectionResult i in result)
                {
                    try
                    {
                        return i.XYZPoint;
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
            IList<ClosestPointsPairBetweenTwoCurves> res = new List<ClosestPointsPairBetweenTwoCurves>();
            curve_A.ComputeClosestPoints(curve_B, false, false, false, out res);
            foreach (var i in res)
            {
                return i.XYZPointOnFirstCurve;
            }
            return null;
        }
        private static List<Line> RemoveCoherentLines(List<Line> curves)
        {
            int r = 3;
            List<Line> optimizedCurves = new List<Line>();
            for (int i = 0; i < curves.Count(); i++)
            {
                if (i == 0)
                {
                    if (curves[i].Direction.ToString() != curves[i + 1].Direction.ToString() && curves[i].Direction.ToString() != curves[i + 1].Direction.Negate().ToString())
                    {
                        optimizedCurves.Add(curves[i]);
                    }
                }
                if (i == curves.Count() - 1)
                {
                    if (curves[i].Direction.ToString() != curves[0].Direction.ToString() && curves[i].Direction.ToString() != curves[0].Direction.Negate().ToString())
                    {
                        optimizedCurves.Add(curves[i]);
                    }
                }
                if (i != curves.Count() - 1 && i != 0)
                {
                    if (curves[i].Direction.ToString() != curves[i + 1].Direction.ToString() && curves[i].Direction.ToString() != curves[i + 1].Direction.Negate().ToString())
                    {
                        optimizedCurves.Add(curves[i]);
                    }
                }
            }
            return optimizedCurves;
        }
        private static bool CounterClockVise(Curve previous, Curve current, Curve next, Surface surface)
        {
            UVLine pr = new UVLine(previous, surface);
            UVLine cr = new UVLine(current, surface);
            UVLine nt = new UVLine(next, surface);
            if (cr.AngleTo(nt) > 180 || pr.AngleTo(cr) > 180)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
