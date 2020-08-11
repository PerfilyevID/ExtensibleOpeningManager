using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using KPLN_Loader.Common;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandPlaceTaskOnPickedWall : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            try
            {
                SE_LinkedWall wall = UiTools.PickWall(app, Collections.PickOptions.References);
                Matrix.Matrix<Element> matrix;
                List<Intersection> context;
                try
                {
                    matrix = new Matrix.Matrix<Element>(CollectorTools.GetMepElements(app.ActiveUIDocument.Document));
                    context = matrix.GetContext(wall);
                }
                catch (System.Exception e)
                {
                    PrintError(e);
                    return Result.Failed;
                }
                foreach (Intersection intersection in context)
                {
                    try
                    {
                        if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).IntersectionExist(intersection, wall))
                        {
                            continue;
                        }
                        ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, intersection, app.ActiveUIDocument.Document));
                        element.SetWall(wall);
                        element.AddSubElement(new SE_LocalElement(intersection.Element));
                        element.Reject();
                        element.AddComment(Variables.msg_created);
                        element.Approve(true);
                    }
                    catch (System.Exception) { }
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
