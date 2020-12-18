using Autodesk.Revit.DB;

namespace ExtensibleOpeningManager.Tools.Instances
{
    public class Intersection
    {
        public bool IsValid
        {
            get
            {
                if (Solid == null || BoundingBox == null)
                {
                    return false;
                }
                return true;
            }
        }
        public Element Element { get; }
        public Solid Solid { get; }
        public BoundingBoxXYZ BoundingBox { get; set; }
        public bool Intersects(Intersection intersection)
        {
            return IntersectionTools.IntersectsSolid(Solid, intersection.Solid);
        }
        public Intersection(Element element, Solid solid)
        {
            Element = element;
            Solid = solid;
            try
            {
                BoundingBox = new BoundingBoxXYZ();
                BoundingBox.Max = Solid.GetBoundingBox().Max + Solid.ComputeCentroid();
                BoundingBox.Min = Solid.GetBoundingBox().Min + Solid.ComputeCentroid();
            }
            catch (System.Exception)
            {
                BoundingBox = null;
                Solid = null;
            }
        }
    }
}
