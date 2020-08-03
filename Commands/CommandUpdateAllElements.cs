using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Controll;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandUpdateAllElements : IExecutableCommand
    {
        public CommandUpdateAllElements(UiController controller)
        {
            Controller = controller;
        }
        private UiController Controller { get; }
        public Result Execute(UIApplication app)
        {
            Controller.UpdateAllElements();
            return Result.Succeeded;
        }
    }
}
