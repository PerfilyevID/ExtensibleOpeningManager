using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CommandParseSelectedGroup : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                /*
                Group group = null;
                try
                {
                    Reference reference = commandData.Application.ActiveUIDocument.Selection.PickObject(ObjectType.Element, new GroupSelectionFilter(), "Выберите элемент : <Группа>");
                    group = commandData.Application.ActiveUIDocument.Document.GetElement(reference) as Group;
                }
                catch (Exception)
                { }
                if (group != null)
                {
                    GroupParser.ParseGroup(commandData.Application.ActiveUIDocument.Document, group);
                }
                */
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
    public class GroupSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == -2000095 && elem.Location != null)
            { return true; }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
