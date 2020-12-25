using KPLN_Loader.Common;
using System.Collections.Generic;
using System.Windows;

namespace ExtensibleOpeningManager
{
    public static class ModuleData
    {
#if Revit2020_AR || Revit2020_KR || Revit2020_MEP
        public static string RevitVersion = "2020";
        public static Window RevitWindow { get; set; }
#endif
#if Revit2018_AR || Revit2018_KR || Revit2018_MEP
        public static string RevitVersion = "2018";
#endif
        public static System.IntPtr MainWindowHandle { get; set; }
        public static string Build = string.Format("built for Revit {0} {1}", RevitVersion, UserPreferences.Department.ToString("G"));
        public static string Version = "1.0.2.0b";
        public static string Date = "2020/12/25";
        
        public static string ManualPage = "https://kpln.kdb24.ru/article/87288/";
        public static bool SystemClosed = false;
        public static string ModuleName = "Мониторинг отверстий";
        public static readonly Queue<IExecutableCommand> CommandQueue = new Queue<IExecutableCommand>();
    }
}
