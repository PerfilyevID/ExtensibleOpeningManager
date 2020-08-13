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
                SE_LinkedWall wall = UiTools.PickWall(app, Collections.PickOptions.Local);
                Matrix.Matrix<ExtensibleSubElement> matrix;
                List<ExtensibleSubElement> context;
                try
                {
                    matrix = new Matrix.Matrix<ExtensibleSubElement>(CollectorTools.GetAllSubInstances(app.ActiveUIDocument.Document));
                    context = matrix.GetSubElements(wall);
                }
                catch (System.Exception e)
                {
                    PrintError(e);
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
                        if (isConcreteOpening)
                        {
                            if (UserPreferences.Department == Collections.Department.AR && ((subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_kr_round || (subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_kr_square))
                            {
                                ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, subElement, app.ActiveUIDocument.Document));
                                element.SetWall(wall);
                                element.AddSubElement(subElement);
                                element.Reject();
                                element.AddComment(Variables.msg_created);
                                element.Approve(true);
                                continue;
                            }
                            if (UserPreferences.Department == Collections.Department.AR)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (UserPreferences.Department == Collections.Department.KR && ((subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_ar_round || (subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_ar_square))
                            {
                                ExtensibleElement element = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, subElement, app.ActiveUIDocument.Document));
                                element.SetWall(wall);
                                element.AddSubElement(subElement);
                                element.Reject();
                                element.AddComment(Variables.msg_created);
                                element.Approve(true);
                                continue;
                            }
                            if (UserPreferences.Department == Collections.Department.KR)
                            {
                                continue;
                            }
                        }
                        ExtensibleElement selement = ExtensibleElement.GetExtensibleElementByInstance(CreationTools.CreateFamilyInstance(wall, subElement, app.ActiveUIDocument.Document));
                        selement.SetWall(wall);
                        selement.AddSubElement(subElement);
                        selement.Reject();
                        selement.AddComment(Variables.msg_created);
                        selement.Approve(true);
                    }
                    catch (Exception e) { PrintError(e); }
                }
                return Result.Succeeded;
            }
            catch (System.Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }

        }
    }
}