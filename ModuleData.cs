using KPLN_Loader.Common;
using System.Collections.Generic;
using System.Windows;

namespace ExtensibleOpeningManager
{
    public static class ModuleData
    {
        #if Revit2020
        public static string RevitVersion = "2020";
        #endif
        #if Revit2018
        public static string RevitVersion = "2018";
        #endif
        public static string Build = string.Format("Revit {0} MEP", RevitVersion);
        public static string Version = "1.0.0.1b";
        public static string Date = "2020/08/20";
        
        public static string ManualPage = "https://kpln.kdb24.ru/article/87288/";
        public static bool SystemClosed = false;
        public static string ModuleName = "Мониторинг отверстий";
        #if Revit2020
        public static Window RevitWindow { get; set; }
        #endif
        public static readonly Queue<IExecutableCommand> CommandQueue = new Queue<IExecutableCommand>();
    }
}
