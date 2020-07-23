using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandRemove : IExecutableCommand
    {
        public CommandRemove(ExtensibleElement element)
        {
            Element = element;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            Element.Remove();
            return Result.Succeeded;
        }
    }
}