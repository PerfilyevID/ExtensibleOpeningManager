using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Tools.Instances
{
    public class UVLine
    {
        public UV StartPoint { get; set; }
        public XYZ Direction
        {
            get
            {
                return new XYZ(EndPoint.U - StartPoint.U, EndPoint.V - StartPoint.V, 0);
            }
        }
        public UV EndPoint { get; set; }
        public Surface Surface { get; }
        public UVLine(Curve curve, Surface surface)
        {
            Surface = surface;
            UV sp;
            double sd;
            Surface.Project(curve.GetEndPoint(0), out sp, out sd);
            StartPoint = sp;
            UV ep;
            double ed;
            Surface.Project(curve.GetEndPoint(1), out ep, out ed);
            EndPoint = ep;
        }
        public double AngleTo(UVLine line)
        {
            double angle1 = Math.Round(Math.Atan2(-line.Direction.Normalize().Y, -line.Direction.Normalize().X) * 180 / Math.PI, 5);
            double angle2 = Math.Round(Math.Atan2(Direction.Normalize().Y, Direction.Normalize().X) * 180 / Math.PI, 5);
            if (angle2 < angle1) { angle2 += 360; }
            double trueAngle = angle2 - angle1;
            if (trueAngle < -360) { trueAngle += 360; }
            if (trueAngle > 360) { trueAngle -= 360; }
            return trueAngle;
        }
    }
}
