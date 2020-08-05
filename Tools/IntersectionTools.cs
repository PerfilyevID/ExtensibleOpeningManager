using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Tools.Instances;
using System;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Tools
{
    public static class IntersectionTools
    {
        public static Intersection GetIntersection(Wall wall, Element element)
        {
            return null;
        }
        public static Solid SolidIntersection(Solid solid_1, Solid solid_2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid_1, solid_2, BooleanOperationsType.Intersect);
        }
        public static bool IntersectsBoundingBox(BoundingBoxXYZ boxA, BoundingBoxXYZ boxB)
        {
            if (boxA.Min.X > boxB.Max.X || boxB.Min.X > boxA.Max.X)
            {
                return false;
            }
            if (boxA.Min.Y > boxB.Max.Y || boxB.Min.Y > boxA.Max.Y)
            {
                return false;
            }
            if (boxA.Min.Z > boxB.Max.Z || boxB.Min.Z > boxA.Max.Z)
            {
                return false;
            }
            return true;
        }
        public static bool IntersectsSolid(Solid solidA, Solid solidB)
        {
            try
            {

                Solid intersection_result = BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect);
                if (Math.Abs(intersection_result.Volume) != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static Solid GetIntersectionSolid(Solid solidA, Solid solidB)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, BooleanOperationsType.Intersect);
        }
    }
}
