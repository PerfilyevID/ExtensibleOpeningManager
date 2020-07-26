using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandPlaceOpeningByTask : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            try
            {
                ExtensibleSubElement task = UiTools.PickInstance(app, Collections.PickTypeOptions.Instance, Collections.PickOptions.References);
                List<SE_LinkedWall> walls = new List<SE_LinkedWall>();
                foreach (Wall wall in CollectorTools.GetWalls(app.ActiveUIDocument.Document))
                {
                    walls.Add(new SE_LinkedWall(wall));
                }
                Matrix.Matrix<SE_LinkedWall> matrix = new Matrix.Matrix<SE_LinkedWall>(walls);
                List<SE_LinkedWall> context = matrix.GetContext(task);
                if (context.Count == 1)
                {
                    SE_LinkedWall wall = context[0];
                    ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, task, app.ActiveUIDocument.Document));
                    element.SetWall(wall);
                    element.AddSubElement(task);
                    element.Reject();
                    element.AddComment(Variables.msg_created);
                    element.Approve();
                }
                if (context.Count > 1)
                {
                    SE_LinkedWall wall = context[0];
                    ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, task, app.ActiveUIDocument.Document));
                    element.SetWall(wall);
                    element.AddSubElement(task);
                    element.Reject();
                    element.AddComment(Variables.msg_created);
                    element.Approve();
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
