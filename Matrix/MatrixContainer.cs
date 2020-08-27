using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Forms;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Matrix
{
    public class MatrixContainer
    {
        public static readonly double Size = 48;
        private static int Count = 0;
        private double Length { get; }
        private double Width { get; }
        private double Height { get; }
        private BoundingBoxXYZ BoundingBox { get; }
        private List<MatrixContainer> Containers = new List<MatrixContainer>();
        private List<MatrixElement> Elements = new List<MatrixElement>();
        private MatrixContainer Parent { get; set; }
        public MatrixContainer(XYZ min, XYZ max, bool parent = true, Progress_Single progress = null)
        {
            if (parent) { Count = 8; }
            else { Count += 8; }
            BoundingBox = new BoundingBoxXYZ();
            BoundingBox.Min = min;
            BoundingBox.Max = max;
            Width = Math.Round(Math.Abs(BoundingBox.Max.X - BoundingBox.Min.X));
            Length = Math.Round(Math.Abs(BoundingBox.Max.Y - BoundingBox.Min.Y));
            Height = Math.Round(Math.Abs(BoundingBox.Max.Z - BoundingBox.Min.Z));
            if (Length >= Size && Height >= Size && Width >= Size)
            {
                //##
                //$#
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2), false, progress));
                //$#
                //##
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height / 2), false, progress));
                //##
                //#$
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2), false, progress));
                //#$
                //##
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height / 2), false, progress));
                //##
                //$#
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height), false, progress));
                //$#
                //##
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height), false, progress));
                //##
                //#$
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height), false, progress));
                //#$
                //##
                if (progress != null) { progress.Increment(); }
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height), false, progress));
            }
        }
        public void Optimize()
        {
            List<MatrixContainer> optimizedContainers = new List<MatrixContainer>();
            foreach (MatrixContainer container in Containers.ToArray())
            {
                if (!container.IsEmpty())
                {
                    optimizedContainers.Add(container);
                }

            }
            Containers = optimizedContainers;
            if (Containers.Count != 0)
            {
                foreach (MatrixContainer subContainer in Containers)
                {
                    subContainer.Optimize();
                }
            }
        }
        public List<MatrixElement> GetByBoundingBoxIntersection(MatrixElement element)
        {
            List<int> ids = new List<int>();
            List<MatrixElement> elements = new List<MatrixElement>();
            if (IntersectionTools.IntersectsBoundingBox(element.BoundingBox, BoundingBox))
            {
                if (Containers.Count == 0)
                {
                    if (Elements.Count != 0)
                    {
                        foreach (MatrixElement e in Elements)
                        {
                            int elementId = e.Element.Id.IntegerValue;
                            if (!ids.Contains(elementId))
                            {
                                elements.Add(e);
                                ids.Add(e.Element.Id.IntegerValue);
                            }
                        }
                    }
                }
                else
                {
                    foreach (MatrixContainer container in Containers)
                    {
                        List<MatrixElement> subElements = container.GetByBoundingBoxIntersection(element);
                        if (subElements.Count != 0)
                        {
                            foreach (MatrixElement e in subElements)
                            {
                                int elementId = e.Element.Id.IntegerValue;
                                if (!ids.Contains(elementId))
                                {
                                    elements.Add(e);
                                    ids.Add(e.Element.Id.IntegerValue);
                                }
                            }
                        }
                    }
                }
            }
            return elements;
        }
        public List<Intersection> GetBySolidIntersection(MatrixElement element)
        {
            List<Intersection> elements = new List<Intersection>();
            try
            {
                foreach (MatrixElement e in GetByBoundingBoxIntersection(element))
                {
                    try
                    {
                        if (IntersectionTools.IntersectsSolid(e.Solid, element.Solid))
                        {
                            try
                            {
                                Solid s = IntersectionTools.GetIntersectionSolid(e.Solid, element.Solid);
                                elements.Add(new Intersection(e.Element, s));
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception)
            { }
            return elements;
        }
        public List<ExtensibleSubElement> GetSubElementsBySolidIntersection(MatrixElement element)
        {
            List<ExtensibleSubElement> subElements = new List<ExtensibleSubElement>();
            foreach (MatrixElement e in GetByBoundingBoxIntersection(element))
            {
                try
                {
                    if (IntersectionTools.IntersectsSolid(e.Solid, element.Solid))
                    {
                        subElements.Add(e.Object as ExtensibleSubElement);
                    }
                }
                catch (Exception) { }
            }
            return subElements;
        }
        public List<SE_LinkedWall> GetWallsBySolidIntersection(MatrixElement element)
        {
            List<SE_LinkedWall> walls = new List<SE_LinkedWall>();
            foreach (MatrixElement e in GetByBoundingBoxIntersection(element))
            {
                try
                {
                    if (IntersectionTools.IntersectsSolid(e.Solid, element.Solid))
                    {
                        if (e.Element.GetType() == typeof(Wall))
                        {
                            walls.Add(new SE_LinkedWall(e.Element as Wall));
                        }
                    }
                }
                catch (Exception) { }
            }
            return walls;
        }
        public void Add(MatrixElement element)
        {
            if (Containers.Count != 0)
            {
                if (IntersectionTools.IntersectsBoundingBox(element.BoundingBox, BoundingBox))
                {
                    foreach (MatrixContainer container in Containers)
                    {
                        container.Add(element);
                    }
                }
            }
            else
            {
                if (IntersectionTools.IntersectsBoundingBox(element.BoundingBox, BoundingBox))
                {
                    Elements.Add(element);
                }
            }
        }
        private bool IsEmpty()
        {
            if (Containers.Count == 0)
            {
                if (Elements.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                foreach (MatrixContainer container in Containers)
                {
                    if (!container.IsEmpty())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
