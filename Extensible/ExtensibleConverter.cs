using Autodesk.Revit.DB;
using static KPLN_Loader.Output.Output;
using System;

namespace ExtensibleOpeningManager.Extensible
{
    public static class ExtensibleConverter
    {
        public static string ConvertDouble(double d)
        {
            return Math.Round(d, 5).ToString();
        }
        public static string ConvertPoint(XYZ point)
        {
            string x = ConvertDouble(point.X);
            string y = ConvertDouble(point.Y);
            string z = ConvertDouble(point.Z);
            return string.Format("({0}+{1}+{2})", x, y, z);
        }
        public static XYZ ConvertToPoint(string point)
        {
            string value = point;
            value = value.Remove(0, 1);
            value = value.Remove(value.Length-1, 1);
            string[] values = value.Split('+');
            double x = double.Parse(values[0]);
            double y = double.Parse(values[1]);
            double z = double.Parse(values[2]);
            return new XYZ(x, y, z);
        }
        public static string ConvertLocation(Location loc)
        {
            try
            {
                if (loc.GetType() == typeof(LocationCurve))
                {
                    Curve curve = (loc as LocationCurve).Curve;
                    return string.Format("[{0}:{1}]", ConvertPoint(curve.GetEndPoint(0)), ConvertPoint(curve.GetEndPoint(1)));
                }
                if (loc.GetType() == typeof(LocationPoint))
                {
                    return ConvertPoint((loc as LocationPoint).Point);
                }
                return Variables.empty;
            }
            catch (Exception)
            {
                return Variables.empty;
            }
        }
    }
}
