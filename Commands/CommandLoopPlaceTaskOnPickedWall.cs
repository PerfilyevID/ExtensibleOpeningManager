using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Matrix;
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
    public class CommandLoopPlaceTaskOnPickedWall : IExecutableCommand
    {
        public CommandLoopPlaceTaskOnPickedWall(SE_LinkedWall wall)
        {
            Wall = wall;
        }
        SE_LinkedWall Wall { get; set; }
        public Result Execute(UIApplication app)
        {
            Matrix.Matrix<Element> matrix = new Matrix.Matrix<Element>(CollectorTools.GetMepElements(app.ActiveUIDocument.Document));
            List<Intersection> context = matrix.GetContext(Wall);
            foreach (Intersection intersection in context)
            {
                if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).IntersectionExist(intersection, Wall))
                {
                    continue;
                }
                ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(Wall, intersection, app.ActiveUIDocument.Document));
                element.SetWall(Wall);
                element.AddSubElement(new SE_LocalElement(intersection.Element));
                element.Reject();
                element.AddComment(Variables.msg_created);
                element.Approve();
                UiController.GetControllerByDocument(app.ActiveUIDocument.Document).LoopController.CreatedElements.Add(element);
            }
            return Result.Succeeded;
        }
    }
}
