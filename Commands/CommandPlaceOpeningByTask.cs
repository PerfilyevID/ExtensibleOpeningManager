using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Forms;
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
                int wallId = int.Parse(ExtensibleController.Read(task.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] { Variables.separator_element }, StringSplitOptions.None)[2], System.Globalization.NumberStyles.Integer);
                bool isConcreteOpening = bool.Parse(ExtensibleController.Read(task.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] { Variables.separator_element }, StringSplitOptions.None)[8]);
                if (isConcreteOpening)
                {
                    if (UserPreferences.Department == Collections.Department.AR && ((task.Element as FamilyInstance).Symbol.FamilyName == Variables.family_kr_round || (task.Element as FamilyInstance).Symbol.FamilyName == Variables.family_kr_square))
                    {
                        Matrix.Matrix<SE_LinkedWall> matrix;
                        List<SE_LinkedWall> context;
                        try
                        {
                            matrix = new Matrix.Matrix<SE_LinkedWall>(CollectorTools.GetWalls(app.ActiveUIDocument.Document));
                            context = matrix.GetContext(task);
                            SE_LinkedWall targetWall = context[0];
                            if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OpeningExist(task, targetWall))
                            {
                                return Result.Cancelled;
                            }
                            try
                            {
                                ExtensibleElement instance = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(targetWall, task, app.ActiveUIDocument.Document));
                                instance.SetWall(targetWall);
                                instance.AddSubElement(task);
                                instance.Reject();
                                instance.AddComment(Variables.msg_created);
                                instance.Approve(true);
                                return Result.Succeeded;
                            }
                            catch (Exception) { }
                        }
                        catch (System.Exception)
                        {
                            return Result.Failed;
                        }
                    }
                    if (UserPreferences.Department == Collections.Department.AR)
                    {
                        Dialogs.ShowDialog("Задание предназначено для отделка КР", "Предупреждение");
                        return Result.Cancelled;
                    }
                }
                else
                {
                    if (UserPreferences.Department == Collections.Department.KR && ((task.Element as FamilyInstance).Symbol.FamilyName == Variables.family_ar_round || (task.Element as FamilyInstance).Symbol.FamilyName == Variables.family_ar_square))
                    {
                        Matrix.Matrix<SE_LinkedWall> matrix;
                        List<SE_LinkedWall> context;
                        try
                        {
                            matrix = new Matrix.Matrix<SE_LinkedWall>(CollectorTools.GetWalls(app.ActiveUIDocument.Document));
                            context = matrix.GetContext(task);
                            SE_LinkedWall targetWall = context[0];
                            if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OpeningExist(task, targetWall))
                            {
                                return Result.Cancelled;
                            }
                            try
                            {
                                ExtensibleElement instance = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(targetWall, task, app.ActiveUIDocument.Document));
                                instance.SetWall(targetWall);
                                instance.AddSubElement(task);
                                instance.Reject();
                                instance.AddComment(Variables.msg_created);
                                instance.Approve(true);
                                return Result.Succeeded;
                            }
                            catch (Exception) { }
                        }
                        catch (System.Exception)
                        {
                            return Result.Failed;
                        }
                    }
                    if (UserPreferences.Department == Collections.Department.KR)
                    {
                        Dialogs.ShowDialog("Задание предназначено для отделка АР", "Предупреждение");
                        return Result.Cancelled;
                    }
                }
                Element element = app.ActiveUIDocument.Document.GetElement(new ElementId(wallId));
                if (element == null)
                {
                    Dialogs.ShowDialog("Стена-основа для выбранного задания ненайдена", "Предупреждение");
                    return Result.Failed;
                }
                if (element.GetType() == typeof(Wall))
                {
                    SE_LinkedWall wall = new SE_LinkedWall(element as Wall);
                    if (IntersectionTools.IntersectsSolid(wall.Solid, task.Solid))
                    {
                        if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OpeningExist(task, wall))
                        {
                            return Result.Cancelled;
                        }
                        try
                        {
                            ExtensibleElement instance = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, task, app.ActiveUIDocument.Document));
                            instance.SetWall(wall);
                            instance.AddSubElement(task);
                            instance.Reject();
                            instance.AddComment(Variables.msg_created);
                            instance.Approve(true);
                        }
                        catch (Exception) { }
                    }
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
