﻿using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using System;
using System.Collections.Generic;
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
            foreach (Edge edge in intersection.Solid.Edges)
            {
                Curve curve = edge.AsCurve();
                points.Add(curve.GetEndPoint(1));
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
                    catch (Exception) { }
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
                    catch (Exception) { }
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
            Position = GetCentroid(new List<BoundingBoxXYZ>() { intersection.BoundingBox });
            Width = LongestLine.Length;
            Height = Math.Abs(MaxZ - MinZ);
            Thickness = wall.Wall.Width;
            Offset = UserPreferences.DefaultOffset / 304.8;
            if (Position.Z - wall.BoundingBox.Min.Z > 0)
            {
                OffsetDown = Position.Z - wall.BoundingBox.Min.Z - Height/2;
            }
            else
            { 
                OffsetDown = 0;
            }
            if (wall.BoundingBox.Max.Z - Position.Z > 0)
            {
                OffsetUp = wall.BoundingBox.Max.Z - Position.Z - Height / 2;
            }
            else
            {
                OffsetUp = 0;
            }
            Level = GetNearestLevel(doc, wall.Wall.Document.GetElement(wall.Wall.LevelId) as Level);
            Elevation = Position.Z - Level.Elevation - Height/2;
    }
        public PlaceParameters(SE_LinkedWall wall, List<Intersection> intersection, Document doc)
        {
            Wall = wall;
            double MaxZ = -999;
            double MinZ = 999;
            List<XYZ> points = new List<XYZ>();
            List<XYZ> projectedPoints = new List<XYZ>();
            List<BoundingBoxXYZ> boxes = new List<BoundingBoxXYZ>();
            foreach (Intersection i in intersection)
            {
                boxes.Add(i.BoundingBox);
                foreach (Edge edge in i.Solid.Edges)
                {
                    Curve curve = edge.AsCurve();
                    points.Add(curve.GetEndPoint(1));
                    points.Add(curve.GetEndPoint(0));
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
                    catch (Exception) { }
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
                    catch (Exception) { }
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
            Position = GetCentroid(boxes);
            Width = LongestLine.Length;
            Height = Math.Abs(MaxZ - MinZ);
            Thickness = wall.Wall.Width;
            Offset = UserPreferences.DefaultOffset / 304.8;
            if (Position.Z - wall.BoundingBox.Min.Z > 0)
            {
                OffsetDown = Position.Z - wall.BoundingBox.Min.Z - Height / 2;
            }
            else
            {
                OffsetDown = 0;
            }
            if (wall.BoundingBox.Max.Z - Position.Z > 0)
            {
                OffsetUp = wall.BoundingBox.Max.Z - Position.Z - Height / 2;
            }
            else
            {
                OffsetUp = 0;
            }
            Level = GetNearestLevel(doc, wall.Wall.Document.GetElement(wall.Wall.LevelId) as Level);
            Elevation = Position.Z - Level.Elevation - Height / 2;
        }
        public PlaceParameters(SE_LinkedInstance instance, Document doc)
        {
            RevitLinkInstance link = CollectorTools.GetRevitLinkById(instance.LinkId, doc);
            FamilyInstance familyInstance = instance.Element as FamilyInstance;
            Position = instance.Solid.ComputeCentroid();
            Width = familyInstance.LookupParameter(Variables.parameter_width).AsDouble();
            Height = familyInstance.LookupParameter(Variables.parameter_height).AsDouble();
            Thickness = familyInstance.LookupParameter(Variables.parameter_thickness).AsDouble();
            Offset = familyInstance.LookupParameter(Variables.parameter_offset_bounds).AsDouble();
            Level = GetNearestLevel(doc, link.Document.GetElement(familyInstance.LevelId) as Level);
            Elevation = Position.Z - Level.Elevation - link.GetTransform().BasisZ.Z;
            OffsetDown = familyInstance.LookupParameter(Variables.parameter_offset_down).AsDouble();
            OffsetUp = familyInstance.LookupParameter(Variables.parameter_offset_up).AsDouble();
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
    }
}
