using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandPlaceOpeningByTaskOnPickedWall : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            try
            {
                SE_LinkedWall wall = UiTools.PickWall(app, Collections.PickOptions.Local);
                Matrix.Matrix<ExtensibleSubElement> matrix;
                List<ExtensibleSubElement> context;
                try
                {
                    matrix = new Matrix.Matrix<ExtensibleSubElement>(CollectorTools.GetMepSubInstances(app.ActiveUIDocument.Document));
                    context = matrix.GetSubElements(wall);
                }
                catch (System.Exception)
                {
                    return Result.Failed;
                }
                foreach (ExtensibleSubElement subElement in context)
                {
                    if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OpeningExist(subElement, wall))
                    {
                        continue;
                    }
                    string[] valueParts = ExtensibleController.Read(subElement.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] { Variables.separator_element}, StringSplitOptions.None);
                    int wallId = int.Parse(valueParts[2], System.Globalization.NumberStyles.Integer);
                    if (wallId != wall.Wall.Id.IntegerValue) { continue; }
                    ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, subElement, app.ActiveUIDocument.Document));
                    element.SetWall(wall);
                    element.AddSubElement(new SE_LocalElement(subElement.Element));
                    element.Reject();
                    element.AddComment(Variables.msg_created);
                    element.Approve();
                }
                return Result.Succeeded;
            }
            catch (System.Exception)
            {
                return Result.Failed;
            }

        }
    }
}