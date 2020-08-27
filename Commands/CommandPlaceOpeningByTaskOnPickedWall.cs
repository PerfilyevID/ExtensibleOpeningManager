using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandPlaceOpeningByTaskOnPickedWall : IExecutableCommand
    {
        public Result Execute(UIApplication app)
        {
            try
            {
                SE_LinkedWall wall;
                try { wall = UiTools.PickWall(app, Collections.PickOptions.Local); }
                catch (Exception) { return Result.Cancelled; }
                Matrix.Matrix<ExtensibleSubElement> matrix;
                List<ExtensibleSubElement> context;
                try
                {
                    foreach (var i in CollectorTools.GetAllSubInstances(app.ActiveUIDocument.Document))
                    {
                    }
                    matrix = new Matrix.Matrix<ExtensibleSubElement>(CollectorTools.GetAllSubInstances(app.ActiveUIDocument.Document));
                    context = matrix.GetSubElements(wall);
                }
                catch (System.Exception)
                {
                    return Result.Failed;
                }
                foreach (ExtensibleSubElement subElement in context)
                {
                    try
                    {
                        if (UiController.GetControllerByDocument(app.ActiveUIDocument.Document).OpeningExist(subElement, wall))
                        {
                            continue;
                        }
                        string[] valueParts = ExtensibleController.Read(subElement.Element as FamilyInstance, Collections.ExtensibleParameter.Wall).Split(new string[] { Variables.separator_element }, StringSplitOptions.None);
                        int wallId = int.Parse(valueParts[2], System.Globalization.NumberStyles.Integer);
                        if (wallId != wall.Wall.Id.IntegerValue && ((subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_mep_round || (subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_mep_square))
                        {
                            continue;
                        }
                        bool isConcreteOpening = bool.Parse(valueParts[8]);
                        if (isConcreteOpening && !UserPreferences.PlaceOnStructuralWalls)
                        {
                            continue;
                        }
                        if (!isConcreteOpening && !UserPreferences.PlaceOnArchitecturalWalls)
                        {
                            continue;
                        }
                        ExtensibleElement selement = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, subElement, app.ActiveUIDocument.Document));
                        selement.SetWall(wall);
                        selement.AddSubElement(subElement);
                        selement.Reject();
                        selement.AddComment(Variables.msg_created);
                        selement.Approve(true);
                    }
                    catch (Exception) { }
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