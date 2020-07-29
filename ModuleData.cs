using KPLN_Loader.Common;
using System.Collections.Generic;
using System.Windows;

namespace ExtensibleOpeningManager
{
    public static class ModuleData
    {
        public static string Build = "Revit 2020 MEP";
        public static string Version = "1.0.0.0a";
        public static string Date = "2020/07/29";
        public static string ManualPage = "https://kpln.kdb24.ru/article/60264/";
        public static bool SystemClosed = false;
        public static string ModuleName = "Мониторинг отверстий";
        public static Window RevitWindow { get; set; }
        public static readonly Queue<IExecutableCommand> CommandQueue = new Queue<IExecutableCommand>();
    }
}
