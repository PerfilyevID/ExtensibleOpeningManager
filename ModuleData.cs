using KPLN_Loader.Common;
using System.Collections.Generic;
using System.Windows;

namespace ExtensibleOpeningManager
{
    public static class ModuleData
    {
        public static string ModuleName = "Мониторинг отверстий";
        public static Window RevitWindow { get; set; }
        public static readonly Queue<IExecutableCommand> CommandQueue = new Queue<IExecutableCommand>();
    }
}
