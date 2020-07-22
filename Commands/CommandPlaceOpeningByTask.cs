using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandPlaceOpeningByTask : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            /*
            ExtensibleSubElement element = UiTools.PickInstance(app, Collections.PickTypeOptions.Instance, Collections.PickOptions.References);
            CreationTools.CreateFamilyInstance(element, app.ActiveUIDocument.Document);
            return Result.Succeeded;
            */
            return Result.Succeeded;
        }
    }
}
