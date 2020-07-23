using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandLoopReject : IExecutableCommand
    {
        public CommandLoopReject(LoopController controller)
        {
            Controller = controller;
        }
        private LoopController Controller { get; set; }
        public Result Execute(UIApplication app)
        {
            try
            {
                foreach (ExtensibleElement element in Controller.CreatedElements)
                {
                    element.Reject() ;
                }
                Controller.CreatedElements.Clear();
                Controller.Next();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                PrintError(e);
                Controller.CreatedElements.Clear();
                Controller.Next();
                return Result.Failed;
            }
        }
    }
}
