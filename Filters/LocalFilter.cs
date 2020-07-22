using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Filters
{
    public static class LocalFilter
    {
        public static bool Passes(Wall wall)
        {
            if (wall.Width < UserPreferences.MinWallWidth ||
                (wall.Name.StartsWith("00") && !UserPreferences.PlaceOnStructuralWalls) ||
                (!wall.Name.StartsWith("00") && !UserPreferences.PlaceOnArchitecturalWalls))
            {
                return false;
            }
            return true;
        }
    }
}
