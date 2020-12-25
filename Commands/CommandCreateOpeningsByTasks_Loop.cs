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
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandCreateOpeningsByTasks_Loop : IExecutableCommand
    {
        private List<SE_LinkedInstance> LinkedInstances { get; set; }
        private UIApplication Application { get; set; }
        private SizeOptions SizeOptions { get; set; }
        private bool OptionGroupEntersected { get; set; } = false;
        private bool OptionRejectCreated { get; set; } = false;
        private bool OptionPlaceOnConcreteWals { get; set; } = false;
        private bool OptionPlaceOnArchitecturalWals { get; set; } = false;
        private List<ExtensibleElement> CreatedInstances { get; set; } = new List<ExtensibleElement>();
        public CommandCreateOpeningsByTasks_Loop(List<SE_LinkedInstance> linkedInstances, bool optionPlaceOnConcreteWals, bool optionPlaceOnArchitecturalWals, bool optionGroupEntersected, bool optionRejectCreated, SizeOptions sizeOptions)
        {
            SizeOptions = sizeOptions;
            OptionPlaceOnConcreteWals = optionPlaceOnConcreteWals;
            OptionPlaceOnArchitecturalWals = optionPlaceOnArchitecturalWals;
            OptionGroupEntersected = optionGroupEntersected;
            OptionRejectCreated = optionRejectCreated;
            LinkedInstances = linkedInstances;
        }
        private ExtensibleElement GroupElements(ExtensibleElement element_A, ExtensibleElement element_B)
        {
            try
            {
                List<ExtensibleElement> elements = new List<ExtensibleElement>() { element_A, element_B };
                SE_LinkedWall wall = element_A.Wall;
                if (CreationTools.HasInvalidIntersections(elements, wall)) { return null; }
                List<ExtensibleSubElement> subElements = new List<ExtensibleSubElement>();
                foreach (ExtensibleElement e in elements)
                {
                    foreach (ExtensibleSubElement s in e.SubElements)
                    {
                        subElements.Add(s);
                    }
                    e.Remove();
                }
                Application.ActiveUIDocument.Document.Regenerate();
                ExtensibleElement groupedElement = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.GroupInstance(element_A.Wall, elements, Application.ActiveUIDocument.Document));
                groupedElement.SetWall(wall);
                groupedElement.AddComment(Variables.msg_autocreated);
                groupedElement.Approve(true);
                foreach (ExtensibleSubElement s in subElements)
                {
                    groupedElement.AddSubElement(s);
                }
                groupedElement.Approve(false);
                if (OptionRejectCreated)
                {
                    groupedElement.Reject();
                }
                groupedElement.Instance.LookupParameter(Variables.parameter_offset_bounds).Set(UserPreferences.DefaultOffset / 304.8);
                return groupedElement;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private bool TryGroupElements(ExtensibleElement element_A)
        {
            if (element_A.Instance.Symbol.FamilyName == Variables.family_ar_round || element_A.Instance.Symbol.FamilyName == Variables.family_kr_round || element_A.Instance.Symbol.FamilyName == Variables.family_mep_round) return false;
            ExtensibleElement element_C = null;
            foreach (ExtensibleElement element_B in CreatedInstances)
            {
                if (element_B.Instance.Symbol.FamilyName == Variables.family_ar_round || element_B.Instance.Symbol.FamilyName == Variables.family_kr_round || element_B.Instance.Symbol.FamilyName == Variables.family_mep_round) continue;
                if (element_A.Wall.Wall.Id.IntegerValue == element_A.Wall.Wall.Id.IntegerValue && element_A.Id != element_B.Id)
                {
                    if (IntersectionTools.IntersectsSolid(element_A.Solid, element_B.Solid))
                    {
                        element_C = element_B;
                        break;
                    }
                }
            }
            if (element_C != null)
            {
                CreatedInstances.Remove(element_A);
                CreatedInstances.Remove(element_C);
                CreatedInstances.Add(GroupElements(element_A, element_C));
                return true;
            }
            return false;
        }
        public Result Execute(UIApplication app)
        {
            Application = app;
            try
            {
                foreach (ExtensibleSubElement task in LinkedInstances)
                {
                    int wallId = int.Parse(ExtensibleController.Read(task.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] { Variables.separator_element }, StringSplitOptions.None)[2], System.Globalization.NumberStyles.Integer);
                    bool isConcreteOpening = bool.Parse(ExtensibleController.Read(task.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] { Variables.separator_element }, StringSplitOptions.None)[8]);
                    if (isConcreteOpening && !UserPreferences.PlaceOnStructuralWalls)
                    {
                        continue;
                    }
                    if (!isConcreteOpening && !UserPreferences.PlaceOnArchitecturalWalls)
                    {
                        continue;
                    }
                    Element element = app.ActiveUIDocument.Document.GetElement(new ElementId(wallId));
                    if (element == null)
                    {
                        continue;
                    }
                    if (element.GetType() == typeof(Wall))
                    {
                        SE_LinkedWall wall = new SE_LinkedWall(element as Wall);
                        if (wall.IsConcreteTask && !OptionPlaceOnConcreteWals) { continue; }
                        if (!wall.IsConcreteTask && !OptionPlaceOnArchitecturalWals) { continue; }
                        if (IntersectionTools.IntersectsSolid(wall.Solid, task.Solid))
                        {
                            if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OpeningExist(task, wall))
                            {
                                continue;
                            }
                            try
                            {
                                ExtensibleElement instance = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, task, app.ActiveUIDocument.Document, SizeOptions));
                                if (instance == null) { continue; }
                                CreatedInstances.Add(instance);
                                instance.SetWall(wall);
                                instance.AddSubElement(task);
                                instance.Reject();
                                instance.AddComment(Variables.msg_autocreated);
                                instance.Approve(true);
                                if (OptionRejectCreated)
                                {
                                    instance.Reject();
                                }
                            }
                            catch (Exception e) { PrintError(e); }
                        }
                    }
                    else
                    {
                    }
                }
                if (OptionGroupEntersected)
                {
                    bool acted = true;
                    while (acted)
                    {
                        acted = false;
                        foreach (ExtensibleElement element_A in CreatedInstances.ToList())
                        {
                            if (TryGroupElements(element_A))
                            {
                                acted = true;
                                break;
                            }
                        }
                    }
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
