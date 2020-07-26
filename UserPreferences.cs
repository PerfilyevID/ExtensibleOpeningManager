using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager
{
    public static class UserPreferences
    {
        public static readonly Department Department = Department.AR;
        public static string SubDepartment = "ОВ";
        public static bool PlaceOnArchitecturalWalls = true;
        public static bool PlaceOnStructuralWalls = true;
        public static double MinInstanceWidth = 10;
        public static double MaxInstanceWidth = 10;
        public static double MinWallWidth = 80;
        public static double DefaultOffset = 50;
        public static void TryLoadParameters()
        {
            return;
        }
    }
}
