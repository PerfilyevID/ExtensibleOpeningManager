using KPLN_Loader.Common;
using System.Collections.Generic;
using System.Windows;

namespace ExtensibleOpeningManager
{
    public static class ModuleData
    {
        public static string Build = "Revit 2018 MEP";
        public static string Version = "1.0.0.0b";
        public static string Date = "2020/08/14";
        public static string RevitVersion = "2018";
        public static string ManualPage = "https://kpln.kdb24.ru/article/87288/";
        public static bool SystemClosed = false;
        public static string ModuleName = "Мониторинг отверстий";
        public static Window RevitWindow { get; set; }
        public static readonly Queue<IExecutableCommand> CommandQueue = new Queue<IExecutableCommand>();
    }
}
