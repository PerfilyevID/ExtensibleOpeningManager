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
        public CommandLoopPlaceTaskOnPickedWall(SE_LinkedWall wall, Matrix<Element> matrix)
        {
            Wall = wall;
            Matrix = matrix;
        }
        private SE_LinkedWall Wall { get; set; }
        private Matrix<Element> Matrix { get; set; }
        public Result Execute(UIApplication app)
        {
            List<Intersection> context = Matrix.GetContext(Wall);
            foreach (Intersection intersection in context)
            {
                if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).IntersectionExist(intersection))
                {
                    continue;
                }
                ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(Wall, intersection, app.ActiveUIDocument.Document));
                element.SetWall(Wall);
                element.AddSubElement(new SE_LocalElement(intersection.Element));
                element.Reject();
                element.AddComment(Variables.msg_created);
                element.Approve();
            }
            return Result.Succeeded;
        }
    }
}
