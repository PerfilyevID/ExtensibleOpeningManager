using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using System;
using System.Collections.Generic;
using System.Linq;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Tools.Instances
{
    public class PlaceParameters
    {
        public XYZ Position { get; }
        public double Width { get; }
        public double Height { get; }
        public double Thickness { get; }
        public double Offset { get; }
        public double OffsetDown { get; }
        public double OffsetUp { get; }
        public double Elevation { get; }
        public double GlobalOffset { get; set; }
        public double GetAngle(FamilyInstance instance)
        {
            XYZ wallLineDirection = LongestLine.Direction;
            XYZ instanceFacingOrientation = instance.FacingOrientation;
            double wallAngle = ConvertRadiansToDegrees(Math.Atan2(wallLineDirection.X, wallLineDirection.Y));
            if (wallAngle < 0) { wallAngle += 360; }
            double instanceAngle = ConvertRadiansToDegrees(Math.Atan2(instanceFacingOrientation.X, instanceFacingOrientation.Y));
            if (instanceAngle < 0) { instanceAngle += 360; }
            double trueAngle = instanceAngle - wallAngle + 90;
            if (trueAngle < 0) { trueAngle += 360; }
            if (trueAngle > 360) { trueAngle -= 360; }
            double Angle = ConvertToRadians(trueAngle);
            return Angle;
        }
        public Level Level { get; }
        private Line LongestLine { get; set; }
        private SE_LinkedWall Wall { get; set; }
        public PlaceParameters(SE_LinkedWall wall, Intersection intersection, Document doc)
        {
            Wall = wall;
            double MaxZ = -999;
            double MinZ = 999;
            List<XYZ> points = new List<XYZ>();
            List<XYZ> projectedPoints = new List<XYZ>();
            List<Solid> geometry = new List<Solid>() { intersection.Solid };
            foreach (Edge edge in intersection.Solid.Edges)
            {
                Curve curve = edge.AsCurve();
                points.Add(curve.GetEndPoint(0));
            }
            if (wall.LinkId != ElementId.InvalidElementId)
            {
                Curve wallCurve = (wall.Wall.Location as LocationCurve).Curve.CreateTransformed(wall.Transform);
                foreach (XYZ point in points)
                {
                    try
                    {
                        if (point.Z > MaxZ) { MaxZ = point.Z; }
                        if (point.Z < MinZ) { MinZ = point.Z; }
                        projectedPoints.Add(wallCurve.Project(point).XYZPoint);
                    }
                    catch (Exception e) { PrintError(e); }
                }
            }
            else
            {
                Curve wallCurve = (wall.Wall.Location as LocationCurve).Curve;
                foreach (XYZ point in points)
                {
                    try
                    {
                        if (point.Z > MaxZ) { MaxZ = point.Z; }
                        if (point.Z < MinZ) { MinZ = point.Z; }
                        projectedPoints.Add(wallCurve.Project(point).XYZPoint);
                    }
                    catch (Exception e) { PrintError(e); }
                }
            }
            LongestLine = null;
            foreach (XYZ pt_1 in projectedPoints)
            {
                foreach (XYZ pt_2 in projectedPoints)
                {
                    try
                    {
                        Line projectedLine = Line.CreateBound(pt_1, pt_2);
                        if (LongestLine == null)
                        {
                            LongestLine = projectedLine;
                        }
                        else
                        {
                            if (LongestLine.Length < projectedLine.Length)
                            {
                                LongestLine = projectedLine;
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            XYZ position = GetCentroid(new List<BoundingBoxXYZ>() { intersection.BoundingBox });
            XYZ projectedPosition = wall.WallLine.Project(position).XYZPoint;
            Position = new XYZ(projectedPosition.X, projectedPosition.Y, position.Z);
            Width = Math.Round(LongestLine.Length, Variables.round_system_value);
            Height = GetCompoundHeight(geometry);
            Thickness = Math.Round(wall.Wall.Width, Variables.round_system_value);
            Offset = Math.Round(UserPreferences.DefaultOffset / 304.8, Variables.round_system_value);
            Level = GetNearestLevel(doc, wall.Wall.Document.GetElement(wall.Wall.LevelId) as Level);
            if (Position.Z - wall.BoundingBox.Min.Z - Height / 2 > 0)
            {
                OffsetDown = Math.Round(Position.Z - wall.BoundingBox.Min.Z - Height / 2, Variables.round_system_value);
            }
            else
            { 
                OffsetDown = 0;
            }
            if (wall.BoundingBox.Max.Z - Position.Z - Height / 2 > 0)
            {
                OffsetUp = Math.Round(wall.BoundingBox.Max.Z - Position.Z - Height / 2, Variables.round_system_value);
            }
            else
            {
                OffsetUp = 0;
            }
            Elevation = Math.Round(Position.Z - Level.Elevation - Height / 2, Variables.round_system_value);
    }
        public PlaceParameters(SE_LinkedWall wall, List<Intersection> intersection, Document doc)
        {
            Wall = wall;
            double MaxZ = -999;
            double MinZ = 999;
            List<XYZ> points = new List<XYZ>();
            List<XYZ> projectedPoints = new List<XYZ>();
            List<BoundingBoxXYZ> boxes = new List<BoundingBoxXYZ>();
            List<Solid> geometry = new List<Solid>();
            foreach (Intersection i in intersection)
            {
                geometry.Add(i.Solid);
                boxes.Add(i.BoundingBox);
                foreach (Edge edge in i.Solid.Edges)
                {
                    Curve curve = edge.AsCurve();
                    points.Add(curve.GetEndPoint(0));
                    //points.Add(curve.GetEndPoint(1));
                }
            }
            if (wall.LinkId != ElementId.InvalidElementId)
            {
                Curve wallCurve = (wall.Wall.Location as LocationCurve).Curve.CreateTransformed(wall.Transform);
                foreach (XYZ point in points)
                {
                    try
                    {
                        if (point.Z > MaxZ)
                        { 
                            MaxZ = point.Z;
                        }
                        if (point.Z < MinZ)
                        {
                            MinZ = point.Z;
                        }
                        projectedPoints.Add(wallCurve.Project(point).XYZPoint);
                    }
                    catch (Exception e) { PrintError(e); }
                }
            }
            else
            {
                Curve wallCurve = (wall.Wall.Location as LocationCurve).Curve;
                foreach (XYZ point in points)
                {
                    try
                    {
                        if (point.Z > MaxZ)
                        { 
                            MaxZ = point.Z;
                        }
                        if (point.Z < MinZ)
                        {
                            MinZ = point.Z;
                        }
                        projectedPoints.Add(wallCurve.Project(point).XYZPoint);
                    }
                    catch (Exception e) { PrintError(e); }
                }
            }
            LongestLine = null;
            foreach (XYZ pt_1 in projectedPoints)
            {
                foreach (XYZ pt_2 in projectedPoints)
                {
                    try
                    {
                        Line projectedLine = Line.CreateBound(pt_1, pt_2);
                        if (LongestLine == null)
                        {
                            LongestLine = projectedLine;
                        }
                        else
                        {
                            if (LongestLine.Length < projectedLine.Length)
                            {
                                LongestLine = projectedLine;
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            XYZ position = GetCentroid(boxes);
            XYZ projectedPosition = wall.WallLine.Project(position).XYZPoint;
            Position = new XYZ(projectedPosition.X, projectedPosition.Y, position.Z);
            Width = Math.Round(LongestLine.Length, Variables.round_system_value);
            Height = Math.Round(GetCompoundHeight(geometry), Variables.round_system_value);
            Thickness = Math.Round(wall.Wall.Width, Variables.round_system_value);
            Offset = Math.Round(UserPreferences.DefaultOffset / 304.8, Variables.round_system_value);
            Level = GetNearestLevel(doc, wall.Wall.Document.GetElement(wall.Wall.LevelId) as Level);
            if (Position.Z - wall.BoundingBox.Min.Z - Height / 2 > 0)
            {

                OffsetDown = Math.Round(Position.Z - wall.BoundingBox.Min.Z - Height / 2, Variables.round_system_value);
            }
            else
            {
                OffsetDown = 0;
            }
            if (wall.BoundingBox.Max.Z - Position.Z - Height / 2 > 0)
            {
                OffsetUp = Math.Round(wall.BoundingBox.Max.Z - Position.Z - Height / 2, Variables.round_system_value);
            }
            else
            {
                OffsetUp = 0;
            }
            Elevation = Math.Round(Position.Z - Level.Elevation - Height / 2, Variables.round_system_value);
        }
        private XYZ GetCentroid(List<BoundingBoxXYZ> boxes)
        {
            double max_X = -999999;
            double max_Y = -999999;
            double max_Z = -999999;
            double min_X = 999999;
            double min_Y = 999999;
            double min_Z = 999999;
            foreach (BoundingBoxXYZ box in boxes)
            {
                if (max_X < box.Max.X) { max_X = box.Max.X; }
                if (max_Y < box.Max.Y) { max_Y = box.Max.Y; }
                if (max_Z < box.Max.Z) { max_Z = box.Max.Z; }
                if (min_X > box.Min.X) { min_X = box.Min.X; }
                if (min_Y > box.Min.Y) { min_Y = box.Min.Y; }
                if (min_Z > box.Min.Z) { min_Z = box.Min.Z; }
            }
            return new XYZ((max_X + min_X) / 2, (max_Y + min_Y) / 2, (max_Z + min_Z) / 2);
        }
        private Level GetNearestLevel(Document doc, Level level)
        {
            Level l = null;
            double difference = 999999;
            foreach (Element lvl in CollectorTools.GetLevels(doc))
            {
                if (l == null)
                {
                    l = lvl as Level;
                    difference = Math.Abs(l.Elevation - level.Elevation);
                    continue;
                }
                if (Math.Abs((lvl as Level).Elevation - level.Elevation) < difference)
                {
                    l = lvl as Level;
                    difference = Math.Abs(l.Elevation - level.Elevation);
                }
            }
            return l;
        }
        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }
        public static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
        public static double GetCompoundHeight(List<Solid> geometry)
        {
            double min = 999;
            double max = -999;
            foreach (Solid solid in geometry)
            {
                double mi = solid.GetBoundingBox().Min.Z + solid.ComputeCentroid().Z;
                if (mi < min || min == 999)
                {
                    min = mi;
                }
                double ma = solid.GetBoundingBox().Max.Z + solid.ComputeCentroid().Z;
                if (ma > max || max == -999)
                {
                    max = ma;
                }
            }
            double result = max - min;
            return result;
        }
    }
}
