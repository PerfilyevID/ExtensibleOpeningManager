using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Extensible;
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
                Matrix.Matrix<SE_LinkedWall> matrix;
                List<SE_LinkedWall> context;
                try
                {
                    matrix = new Matrix.Matrix<SE_LinkedWall>(walls);
                    context = matrix.GetContext(task);
                }
                catch (Exception)
                {
                    return Result.Failed;
                }
                SE_LinkedWall targetWall = null;
                try
                {
                    targetWall = SE_LinkedWall.GetLinkedWallById(context, int.Parse(ExtensibleController.Read(task.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] {Variables.separator_element }, StringSplitOptions.None)[2], System.Globalization.NumberStyles.Integer));
                }
                catch (Exception) { targetWall = null; }
                bool condition_AR = (UserPreferences.PlaceOnArchitecturalWalls && !targetWall.Wall.Name.StartsWith("00"));
                bool condition_KR = (UserPreferences.PlaceOnStructuralWalls && targetWall.Wall.Name.StartsWith("00"));
                if (targetWall != null && (condition_AR || condition_KR))
                {
                    ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(targetWall, task, app.ActiveUIDocument.Document));
                    element.SetWall(targetWall);
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
