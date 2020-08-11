using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common.ExtensibleSubElements
{
    public class SE_LinkedWall
    {
        private SE_LinkedWall()
        {

        }
        public SE_LinkedWall(Wall wall)
        {
            Document = wall.Document;
            LinkId = new ElementId(-1);
            Wall = wall;
            Transform = null;
            WallLine = (wall.Location as LocationCurve).Curve as Line;
            Solid = GeometryTools.GetCorrectSolid(Wall, Transform);
            BoundingBox = new BoundingBoxXYZ();
            BoundingBox.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid();
            BoundingBox.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid();
        }
        public SE_LinkedWall(RevitLinkInstance revitLinkInstance, Wall wall)
        {
            Document = revitLinkInstance.GetLinkDocument();
            LinkId = revitLinkInstance.Id;
            Wall = wall;
            Transform = revitLinkInstance.GetTransform();
            Line wallLine = (wall.Location as LocationCurve).Curve as Line;
            WallLine = wallLine.CreateTransformed(Transform) as Line;
            Solid = GeometryTools.GetCorrectSolid(Wall, Transform);
            BoundingBox = new BoundingBoxXYZ();
            BoundingBox.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid();
            BoundingBox.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid();
        }
        public void CreateSolid(Document doc)
        {
            List<GeometryObject> geom = new List<GeometryObject>();
            geom.Add(Solid);
            IList<GeometryObject> igeom = geom;
            DirectShape shape = DirectShape.CreateElement(doc, new ElementId(-2001140));
            shape.SetShape(igeom);
        }
        public Document Document { get; set; }
        public ElementId LinkId { get; set; }
        public Solid Solid { get; set; }
        public Wall Wall { get; set; }
        public bool SavedAsConcrete { get; set; }
        public bool IsConcreteTask
        {
            get 
            {
                if (Wall == null)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        if (Wall.Name.StartsWith("00"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }
        public Transform Transform { get; set; }
        public BoundingBoxXYZ BoundingBox { get; set; }
        public Line WallLine { get; set; }
        public override string ToString()
        {
            double area = Solid.SurfaceArea;
            double volume = Solid.Volume;
            return string.Join(Variables.separator_element, new string[]
                                { ExtensibleConverter.ConvertLocation(Wall.Location),
                                LinkId.ToString(),
                                Wall.Id.ToString(),
                                Wall.GetTypeId().ToString(),
                                ExtensibleConverter.ConvertDouble(area),
                                ExtensibleConverter.ConvertDouble(volume),
                                Wall.LevelId.ToString(),
                                ExtensibleConverter.ConvertPoint(Solid.ComputeCentroid()),
                                IsConcreteTask.ToString()});
        }
        public static SE_LinkedWall GetLinkedWallById(List<SE_LinkedWall> context, int targetWall)
        {
            foreach (SE_LinkedWall wall in context)
            {
                if (wall.Wall.Id.IntegerValue == targetWall)
                {
                    return wall;
                }
            }
            return null;
        }
        public static SE_LinkedWall TryParse(Document doc, string value)
        {
            string[] values = value.Split(new string[] { Variables.separator_element }, StringSplitOptions.None);
            bool savedAsConcrete = false;
            try
            {
                savedAsConcrete = values[8] == true.ToString();
            }
            catch (Exception) { }
            ElementId linkId = new ElementId(int.Parse(values[1], System.Globalization.NumberStyles.Integer));
            RevitLinkInstance link = null;
            if (linkId.IntegerValue != -1)
            {
                link = CollectorTools.GetRevitLinkById(linkId, doc);
            }
            Wall wall = null;
            Solid solid = null;
            if (linkId.IntegerValue == -1)
            {
                ElementId wallId = new ElementId(int.Parse(values[2]));
                Element wallElement = doc.GetElement(wallId);
                if (wallElement != null && wallElement.GetType() == typeof(Wall))
                {
                    wall = wallElement as Wall;
                    solid = GeometryTools.GetCorrectSolid(wall);
                }
            }
            else
            {
                if (link != null)
                {
                    try
                    {
                        ElementId wallId = new ElementId(int.Parse(values[2]));
                        Element wallElement = link.GetLinkDocument().GetElement(wallId);
                        if (wallElement != null && wallElement.GetType() == typeof(Wall))
                        {
                            wall = wallElement as Wall;
                            solid = GeometryTools.GetCorrectSolid(wall, link.GetTransform());
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
            if (wall != null)
            {
                Line wallLine = (wall.Location as LocationCurve).Curve as Line;

                BoundingBoxXYZ BoundingBox = new BoundingBoxXYZ();
                BoundingBox.Max = solid.GetBoundingBox().Max + solid.ComputeCentroid();
                BoundingBox.Min = solid.GetBoundingBox().Min + solid.ComputeCentroid();
                Transform transform = null;
                if (link != null)
                {
                    transform = link.GetTransform();
                    wallLine = wallLine.CreateTransformed(transform) as Line;
                }
                return new SE_LinkedWall() { Wall = wall, WallLine = wallLine, SavedAsConcrete = savedAsConcrete, Solid = solid, Document = wall.Document, LinkId = linkId, Transform = transform, BoundingBox = BoundingBox };
            }
            else
            { 
                return null;
            }
        }
    }
}
