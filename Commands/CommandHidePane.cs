using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Forms;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandHidePane : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            try
            {
                if (app.GetDockablePane(new DockablePaneId(DockablePreferences.PageGuid)).IsShown())
                {
                    app.GetDockablePane(new DockablePaneId(DockablePreferences.PageGuid)).Hide();
                    ModuleData.SystemClosed = true;
                }
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

        }
    }
}
