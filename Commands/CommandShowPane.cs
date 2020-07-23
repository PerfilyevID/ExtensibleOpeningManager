using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Forms;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandShowPane : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            try
            {
                if (!app.GetDockablePane(new DockablePaneId(DockablePreferences.PageGuid)).IsShown())
                {
                    app.GetDockablePane(new DockablePaneId(DockablePreferences.PageGuid)).Show();
                    ModuleData.SystemClosed = false;
                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }

        }
    }
}
