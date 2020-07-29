using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Matrix
{
    public class MatrixContainer
    {
        private double Length { get; }
        private double Width { get; }
        private double Height { get; }
        private BoundingBoxXYZ BoundingBox { get; }
        private List<MatrixContainer> Containers = new List<MatrixContainer>();
        private List<MatrixElement> Elements = new List<MatrixElement>();
        public MatrixContainer(XYZ min, XYZ max)
        {
            BoundingBox = new BoundingBoxXYZ();
            BoundingBox.Min = min;
            BoundingBox.Max = max;
            Width = Math.Round(Math.Abs(BoundingBox.Max.X - BoundingBox.Min.X));
            Length = Math.Round(Math.Abs(BoundingBox.Max.Y - BoundingBox.Min.Y));
            Height = Math.Round(Math.Abs(BoundingBox.Max.Z - BoundingBox.Min.Z));
            if (Length >= 12.0 && Height >= 12.0 && Width >= 12.0)
            {
                //##
                //$#
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2)));
                //$#
                //##
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height / 2)));
                //##
                //#$
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2)));
                //#$
                //##
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height / 2)));
                //##
                //$#
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height)));
                //$#
                //##
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height)));
                //##
                //#$
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height)));
                //#$
                //##
                Containers.Add(new MatrixContainer(new XYZ(BoundingBox.Min.X + Width / 2, BoundingBox.Min.Y + Length / 2, BoundingBox.Min.Z + Height / 2),
                    new XYZ(BoundingBox.Min.X + Width, BoundingBox.Min.Y + Length, BoundingBox.Min.Z + Height)));
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
            foreach (MatrixElement e in GetByBoundingBoxIntersection(element))
            {
                if (IntersectionTools.IntersectsSolid(e.Solid, element.Solid))
                {
                    Solid s = IntersectionTools.GetIntersectionSolid(e.Solid, element.Solid);
                    elements.Add(new Intersection(e.Element, s));
                }
            }
            return elements;
        }
        public List<ExtensibleSubElement> GetSubElementsBySolidIntersection(MatrixElement element)
        {
            List<ExtensibleSubElement> subElements = new List<ExtensibleSubElement>();
            foreach (MatrixElement e in GetByBoundingBoxIntersection(element))
            {
                if (IntersectionTools.IntersectsSolid(e.Solid, element.Solid))
                {
                    if (e.Element.GetType() == typeof(Wall))
                    {
                        subElements.Add(e.Object as ExtensibleSubElement);
                    }
                }
            }
            return subElements;
        }
        public List<SE_LinkedWall> GetWallsBySolidIntersection(MatrixElement element)
        {
            List<SE_LinkedWall> walls = new List<SE_LinkedWall>();
            foreach (MatrixElement e in GetByBoundingBoxIntersection(element))
            {
                if (IntersectionTools.IntersectsSolid(e.Solid, element.Solid))
                {
                    if(e.Element.GetType() == typeof(Wall))
                    {
                        walls.Add(new SE_LinkedWall(e.Element as Wall));
                    }
                }
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
