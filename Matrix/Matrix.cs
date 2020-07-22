using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Forms;
using ExtensibleOpeningManager.Tools.Instances;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Matrix
{
    public class Matrix<T>
    {
        public Matrix(List<T> elements)
        {
            if (elements.Count == 0) { throw new Exception("Нельзя создать матрицу на основе пустого списка!"); }
            double maxX = -999999;
            double maxY = -999999;
            double maxZ = -999999;
            double minX = 999999;
            double minY = 999999;
            double minZ = 999999;
            foreach (var i in elements)
            {
                bool detected = false;
                MatrixElement el = null;
                if (!detected && i.GetType() == typeof(ExtensibleElement))
                {
                    el = new MatrixElement(i as ExtensibleElement);
                    detected = true;
                }
                if (!detected && i.GetType() == typeof(ExtensibleSubElement))
                {
                    el = new MatrixElement(i as ExtensibleSubElement);
                    detected = true;
                }
                if (!detected && i.GetType() == typeof(SE_LinkedWall))
                {
                    el = new MatrixElement(i as SE_LinkedWall);
                    detected = true;
                }
                if(!detected)
                {
                    el = new MatrixElement(i as Element);
                    detected = true;
                }
                if (el == null) { continue; }
                Elements.Add(el);
                if (maxX < el.BoundingBox.Max.X) { maxX = el.BoundingBox.Max.X; }
                if (maxY < el.BoundingBox.Max.Y) { maxY = el.BoundingBox.Max.Y; }
                if (maxZ < el.BoundingBox.Max.Z) { maxZ = el.BoundingBox.Max.Z; }
                if (minX > el.BoundingBox.Min.X) { minX = el.BoundingBox.Min.X; }
                if (minY > el.BoundingBox.Min.Y) { minY = el.BoundingBox.Min.Y; }
                if (minZ > el.BoundingBox.Min.Z) { minZ = el.BoundingBox.Min.Z; }
            }
            if (Elements.Count == 0) { throw new Exception("Null collection!"); }
            BoundingBoxXYZ boundingBox = new BoundingBoxXYZ() { Min = new XYZ(minX, minY, minZ), Max = new XYZ(maxX, maxY, maxZ) };
            double x_length = Math.Abs(boundingBox.Max.X - boundingBox.Min.X);
            double y_length = Math.Abs(boundingBox.Max.Y - boundingBox.Min.Y);
            double z_length = Math.Abs(boundingBox.Max.Z - boundingBox.Min.Z);
            double length = Math.Ceiling(Math.Max(x_length, y_length) / 12) * 12;
            length = Math.Ceiling(Math.Max(length, z_length) / 12) * 12 * 2;
            XYZ c = new XYZ(Math.Round((boundingBox.Max.X + boundingBox.Min.X) / 2 / 12) * 12, Math.Round((boundingBox.Max.Y + boundingBox.Min.Y) / 2 / 12) * 12, Math.Round((boundingBox.Max.Z + boundingBox.Min.Z) / 2 / 12) * 12);
            BoundingBox = new BoundingBoxXYZ() { Min = new XYZ(c.X - length, c.Y - length, c.Z - length), Max = new XYZ(c.X + length, c.Y + length, c.Z + length) };
            Container = new MatrixContainer(BoundingBox.Min, BoundingBox.Max);
            foreach (MatrixElement el in Elements)
            {
                Container.Add(el);
            }
            Container.Optimize();
        }
        public List<Intersection> GetContext(ExtensibleSubElement element)
        {
            return GetBySolidIntersection(new MatrixElement(element));
        }
        public List<Intersection> GetContext(SE_LinkedWall wall)
        {
            return GetBySolidIntersection(new MatrixElement(wall));
        }
        public List<Intersection> GetContext(ExtensibleElement element)
        {
            return GetBySolidIntersection(new MatrixElement(element));
        }
        private MatrixContainer Container { get; }
        private List<MatrixElement> Elements = new List<MatrixElement>();
        private BoundingBoxXYZ BoundingBox { get; }
        public List<MatrixElement> GetByBoundingBoxIntersection(MatrixElement element)
        {
            return Container.GetByBoundingBoxIntersection(element);
        }
        public List<Intersection> GetBySolidIntersection(MatrixElement element)
        {
            return Container.GetBySolidIntersection(element);
        }

    }
}
