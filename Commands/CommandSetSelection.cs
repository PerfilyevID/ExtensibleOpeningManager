using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandSetSelection : IExecutableCommand
    {
        public CommandSetSelection()
        {
            Selection = new List<ElementId>() { };
        }
        public CommandSetSelection(Element element)
        {
            Selection = new List<ElementId>() { element.Id };
        }
        private ICollection<ElementId> Selection { get; set; }
        public Result Execute(UIApplication app)
        {
            try
            {
                app.ActiveUIDocument.Selection.SetElementIds(Selection);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }

        }
    }
}
