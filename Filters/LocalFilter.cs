using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Filters
{
    public static class LocalFilter
    {
        public static bool Passes(Wall wall)
        {
            if ((wall.Location as LocationCurve).Curve.GetType() != typeof(Line) ||
                wall.Width < UserPreferences.MinWallWidth / 304.8 ||
               (wall.Name.StartsWith("00") && !UserPreferences.PlaceOnStructuralWalls) ||
               (!wall.Name.StartsWith("00") && !UserPreferences.PlaceOnArchitecturalWalls))
            {
                return false;
            }
            return true;
        }
    }
}
