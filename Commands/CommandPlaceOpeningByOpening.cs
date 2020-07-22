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
    public class CommandPlaceOpeningByOpening : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            /*
            FamilyInstance instance = UiTools.PickInstance();
            ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(instance);
            CreationTools.CreateFamilyInstance(element, app.ActiveUIDocument.Document);
            return Result.Succeeded;
            */
            return Result.Succeeded;
        }
    }
}
