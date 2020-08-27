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
        public Matrix(List<T> elements, bool showProgress = false)
        {
            if (elements.Count == 0) { throw new Exception("Нельзя создать матрицу на основе пустого списка!"); }
            double maxX = -999999;
            double maxY = -999999;
            double maxZ = -999999;
            double minX = 999999;
            double minY = 999999;
            double minZ = 999999;
            string format = "{0} из " + elements.Count.ToString() + " элементов обработано";
            if (showProgress)
            {
                using (Progress_Single progress = new Progress_Single("Подготовка поисковой матрицы", format, elements.Count))
                {
                    foreach (var i in elements)
                    {
                        progress.Increment();
                        try
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
                            if (!detected && i.GetType() == typeof(SE_LinkedInstance))
                            {
                                el = new MatrixElement(i as SE_LinkedInstance);
                                detected = true;
                            }
                            if (!detected && i.GetType() == typeof(Wall))
                            {
                                el = new MatrixElement(i as Wall);
                                detected = true;
                            }
                            if (!detected)
                            {
                                el = new MatrixElement(i as Element);
                                detected = true;
                            }
                            if (el == null)
                            {
                                continue;
                            }
                            Elements.Add(el);
                            if (maxX < el.BoundingBox.Max.X) { maxX = el.BoundingBox.Max.X; }
                            if (maxY < el.BoundingBox.Max.Y) { maxY = el.BoundingBox.Max.Y; }
                            if (maxZ < el.BoundingBox.Max.Z) { maxZ = el.BoundingBox.Max.Z; }
                            if (minX > el.BoundingBox.Min.X) { minX = el.BoundingBox.Min.X; }
                            if (minY > el.BoundingBox.Min.Y) { minY = el.BoundingBox.Min.Y; }
                            if (minZ > el.BoundingBox.Min.Z) { minZ = el.BoundingBox.Min.Z; }
                        }
                        catch (Exception)
                        { }
                    }
                }
            }
            else
            {
                foreach (var i in elements)
                {
                    try
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
                        if (!detected && i.GetType() == typeof(SE_LinkedInstance))
                        {
                            el = new MatrixElement(i as SE_LinkedInstance);
                            detected = true;
                        }
                        if (!detected && i.GetType() == typeof(Wall))
                        {
                            el = new MatrixElement(i as Wall);
                            detected = true;
                        }
                        if (!detected)
                        {
                            el = new MatrixElement(i as Element);
                            detected = true;
                        }
                        if (el == null)
                        {
                            continue;
                        }
                        Elements.Add(el);
                        if (maxX < el.BoundingBox.Max.X) { maxX = el.BoundingBox.Max.X; }
                        if (maxY < el.BoundingBox.Max.Y) { maxY = el.BoundingBox.Max.Y; }
                        if (maxZ < el.BoundingBox.Max.Z) { maxZ = el.BoundingBox.Max.Z; }
                        if (minX > el.BoundingBox.Min.X) { minX = el.BoundingBox.Min.X; }
                        if (minY > el.BoundingBox.Min.Y) { minY = el.BoundingBox.Min.Y; }
                        if (minZ > el.BoundingBox.Min.Z) { minZ = el.BoundingBox.Min.Z; }
                    }
                    catch (Exception)
                    { }
                }
            }
            if (Elements.Count == 0) { throw new Exception("Null collection!"); }
            BoundingBoxXYZ boundingBox = new BoundingBoxXYZ() { Min = new XYZ(minX, minY, minZ), Max = new XYZ(maxX, maxY, maxZ) };
            double x_length = Math.Abs(boundingBox.Max.X - boundingBox.Min.X);
            double y_length = Math.Abs(boundingBox.Max.Y - boundingBox.Min.Y);
            double z_length = Math.Abs(boundingBox.Max.Z - boundingBox.Min.Z);
            double length = Math.Ceiling(Math.Max(x_length, y_length) / MatrixContainer.Size) * MatrixContainer.Size;
            length = Math.Ceiling(Math.Max(length, z_length) / MatrixContainer.Size) * MatrixContainer.Size * 2;
            XYZ c = new XYZ(Math.Round((boundingBox.Max.X + boundingBox.Min.X) / 2 / MatrixContainer.Size) * MatrixContainer.Size, Math.Round((boundingBox.Max.Y + boundingBox.Min.Y) / 2 / MatrixContainer.Size) * MatrixContainer.Size, Math.Round((boundingBox.Max.Z + boundingBox.Min.Z) / 2 / MatrixContainer.Size) * MatrixContainer.Size);
            BoundingBox = new BoundingBoxXYZ() { Min = new XYZ(c.X - length, c.Y - length, c.Z - length), Max = new XYZ(c.X + length, c.Y + length, c.Z + length) };

            if (showProgress)
            {
                int l = (int)(Math.Ceiling(length / MatrixContainer.Size / 2)) * (int)(Math.Ceiling(length / MatrixContainer.Size / 2)) * (int)(Math.Ceiling(length / MatrixContainer.Size / 2));
                format = "{0} из " + l.ToString() + " поисковых частей создано";
                using (Progress_Single progress = new Progress_Single("Подготовка поисковой матрицы", format, l))
                {
                    Container = new MatrixContainer(BoundingBox.Min, BoundingBox.Max, true, progress);
                }  
            }
            else
            {
                Container = new MatrixContainer(BoundingBox.Min, BoundingBox.Max, true, null);
            }
            if (showProgress)
            {
                format = "{0} из " + Elements.Count.ToString() + " элементов добавлено в матрицу поиска";
                using (Progress_Single progress = new Progress_Single("Подготовка поисковой матрицы", format, Elements.Count))
                {
                    foreach (MatrixElement el in Elements)
                    {
                        progress.Increment();
                        Container.Add(el);
                    }
                }
            }
            else
            {
                foreach (MatrixElement el in Elements)
                {
                    Container.Add(el);
                }
            }
            Container.Optimize();
        }
        public List<ExtensibleSubElement> GetSubElements(SE_LinkedWall element)
        {
            return Container.GetSubElementsBySolidIntersection(new MatrixElement(element));
        }
        public List<SE_LinkedWall> GetContext(ExtensibleSubElement element)
        {
            return Container.GetWallsBySolidIntersection(new MatrixElement(element));
        }
        public List<Intersection> GetContext(SE_LinkedWall element)
        {
            try
            {
                return Container.GetBySolidIntersection(new MatrixElement(element));

            }
            catch (Exception)
            {
                return new List<Intersection>();
            }
        }
        public List<Intersection> GetContext(ExtensibleElement element)
        {
            return Container.GetBySolidIntersection(new MatrixElement(element));
        }
        private MatrixContainer Container { get; }
        private List<MatrixElement> Elements = new List<MatrixElement>();
        private BoundingBoxXYZ BoundingBox { get; }

    }
}
