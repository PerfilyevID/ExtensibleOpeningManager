using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Filters;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensibleOpeningManager.Tools
{
    public static class UiTools
    {
        public static SE_LinkedWall PickWall(UIApplication uiapp, PickOptions options)
        {
            Reference reference;
            try
            {
                switch (options)
                {
                    case PickOptions.Local:
                        reference = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new LocalWallFilter(uiapp.ActiveUIDocument.Document), "Выберите стену : <Стены>");
                        break;
                    case PickOptions.References:
                        reference = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, new LinkedWallFilter(uiapp.ActiveUIDocument.Document), "Выберите стену : <Стены>");
                        break;
                    default:
                        return null;
                }
            }
            catch (Exception e) 
            {
                PrintError(e);
                return null;
            }
            switch (options)
            {
                case PickOptions.Local:
                    return new SE_LinkedWall(uiapp.ActiveUIDocument.Document.GetElement(reference) as Wall);
                case PickOptions.References:
                    RevitLinkInstance linkInstance = uiapp.ActiveUIDocument.Document.GetElement(reference) as RevitLinkInstance;
                    Document linkedDocument = linkInstance.GetLinkDocument();
                    Wall linkedWall = linkedDocument.GetElement(reference.LinkedElementId) as Wall;
                    return new SE_LinkedWall(linkInstance, linkedWall);
                default:
                    return null;
            }
        }
        public static ExtensibleSubElement PickInstance(UIApplication uiapp, PickTypeOptions typeOptions, PickOptions options)
        {
            Reference reference;
            if (typeOptions == PickTypeOptions.Element)
            {
                try
                {
                    switch (options)
                    {
                        case PickOptions.Local:
                            reference = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new LocalElementFilter(uiapp.ActiveUIDocument.Document), "Выберите элемент : <Системы>");
                            break;
                        case PickOptions.References:
                            reference = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, new LinkedElementFilter(uiapp.ActiveUIDocument.Document), "Выберите элемент : <Системы>");
                            break;
                        default:
                            return null;
                    }
                }
                catch (Exception e) 
                {
                    PrintError(e);
                    Print("+", KPLN_Loader.Preferences.MessageType.Code);
                    return null;
                }
            }
            else
            {
                try
                {
                    switch (options)
                    {
                        case PickOptions.Local:
                            reference = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new LocalInstanceFilter(), "Выберите отверстие/задание : <Оборудование>");
                            break;
                        case PickOptions.References:
                            reference = uiapp.ActiveUIDocument.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, new LinkedInstanceFilter(uiapp.ActiveUIDocument.Document), "Выберите отверстие/задание : <Оборудование>");
                            break;
                        default:
                            return null;
                    }
                }
                catch (Exception e)
                {
                    PrintError(e);
                    Print("+", KPLN_Loader.Preferences.MessageType.Code);
                    return null;
                }
            }
            switch (options)
            {
                case PickOptions.Local:
                    return new SE_LocalElement(uiapp.ActiveUIDocument.Document.GetElement(reference));
                case PickOptions.References:
                    RevitLinkInstance linkInstance = uiapp.ActiveUIDocument.Document.GetElement(reference) as RevitLinkInstance;
                    Document linkedDocument = linkInstance.GetLinkDocument();
                    Element linkedInstance = linkedDocument.GetElement(reference.LinkedElementId);
                    if (typeOptions == PickTypeOptions.Element)
                    {
                        return new SE_LinkedElement(linkInstance, linkedInstance);
                    }
                    else
                    {
                        return new SE_LinkedInstance(linkInstance, linkedInstance);
                    }
                default:
                    return null;
            }
        }
        public static List<ExtensibleSubElement> PickInstances(UIApplication uiapp, PickTypeOptions typeOptions, PickOptions options)
        {
            List<ExtensibleSubElement> subElements = new List<ExtensibleSubElement>();
            IList<Reference> references;
            if (typeOptions == PickTypeOptions.Element)
            {
                try
                {
                    switch (options)
                    {
                        case PickOptions.Local:
                            references = uiapp.ActiveUIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new LocalElementFilter(uiapp.ActiveUIDocument.Document), "Выберите элемент : <Системы>");
                            break;
                        case PickOptions.References:
                            references = uiapp.ActiveUIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, new LinkedElementFilter(uiapp.ActiveUIDocument.Document), "Выберите элемент : <Системы>");
                            break;
                        default:
                            return new List<ExtensibleSubElement>(); ;
                    }
                }
                catch (Exception)
                {
                    return new List<ExtensibleSubElement>();
                }
            }
            else
            {
                try
                {
                    switch (options)
                    {
                        case PickOptions.Local:
                            references = uiapp.ActiveUIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new LocalInstanceFilter(), "Выберите отверстие/задание : <Оборудование>");
                            break;
                        case PickOptions.References:
                            references = uiapp.ActiveUIDocument.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, new LinkedInstanceFilter(uiapp.ActiveUIDocument.Document), "Выберите отверстие/задание : <Оборудование>");
                            break;
                        default:
                            return new List<ExtensibleSubElement>();
                    }
                }
                catch (Exception) 
                {
                    return new List<ExtensibleSubElement>();
                }
            }
            switch (options)
            {
                case PickOptions.Local:
                    foreach (Reference refer in references)
                    { subElements.Add(new SE_LocalElement(uiapp.ActiveUIDocument.Document.GetElement(refer)));}
                    return subElements;
                case PickOptions.References:
                    foreach (Reference refer in references)
                    {
                        RevitLinkInstance linkInstance = uiapp.ActiveUIDocument.Document.GetElement(refer) as RevitLinkInstance;
                        Document linkedDocument = linkInstance.GetLinkDocument();
                        Element linkedInstance = linkedDocument.GetElement(refer.LinkedElementId);
                        subElements.Add(new SE_LinkedElement(linkInstance, linkedInstance));
                    }
                    return subElements;
                default:
                    return new List<ExtensibleSubElement>();
            }
        }
        public static View Get3DView(Document doc)
        {
            foreach (Autodesk.Revit.DB.View view in new FilteredElementCollector(doc).OfClass(typeof(Autodesk.Revit.DB.View)).WhereElementIsNotElementType())
            {
                try
                {
                    ViewFamilyType viewFamilyType = doc.GetElement(view.GetTypeId()) as ViewFamilyType;
                    if (view.get_Parameter(BuiltInParameter.VIEW_NAME).AsString() == Variables.default3dViewName && viewFamilyType.ViewFamily == ViewFamily.ThreeDimensional)
                    {
                        return view;
                    }
                }
                catch (Exception e) 
                {
                    PrintError(e);
                }
            }
            return null;
        }
        public static BuiltInCategory[] CategoryListEnvironment = new BuiltInCategory[] { BuiltInCategory.OST_Walls,
                                    BuiltInCategory.OST_Floors,
                                    BuiltInCategory.OST_Ceilings,
                                    BuiltInCategory.OST_Doors,
                                    BuiltInCategory.OST_Windows,
                                    BuiltInCategory.OST_StackedWalls,
                                    BuiltInCategory.OST_CurtainWallPanels,
                                    BuiltInCategory.OST_PlumbingFixtures,
                                    BuiltInCategory.OST_Stairs};
        public static BuiltInCategory[] CategoryListMEP = new BuiltInCategory[] { BuiltInCategory.OST_DuctCurves,
                                    BuiltInCategory.OST_PipeCurves,
                                    BuiltInCategory.OST_Conduit,
                                    BuiltInCategory.OST_CableTray,
                                    BuiltInCategory.OST_FlexDuctCurves,
                                    BuiltInCategory.OST_DuctFitting,
                                    BuiltInCategory.OST_ConduitFitting,
                                    BuiltInCategory.OST_PipeFitting};
        public static void ActivateView(View view, UIDocument uidoc)
        {
            uidoc.RequestViewChange(view);
            uidoc.Document.Regenerate();
        }
        public static void ZoomElement(BoundingBoxXYZ box, XYZ centroid, UIDocument uidoc)
        {
            try
            {
                XYZ offsetMin = new XYZ(-5, -5, -2);
                XYZ offsetMax = new XYZ(5, 5, 1);
                View3D activeView = uidoc.ActiveView as View3D;
                ViewFamily activeViewFamily = ViewFamily.Invalid;
                try
                {
                    ViewFamilyType viewFamilyType = uidoc.Document.GetElement(activeView.GetTypeId()) as ViewFamilyType;
                    activeViewFamily = viewFamilyType.ViewFamily;
                }
                catch (Exception e) { PrintError(e); }
                if (activeViewFamily == ViewFamily.ThreeDimensional)
                {
                    activeView.SetSectionBox(box);
                    XYZ forward_direction = VectorFromHorizVertAngles(135, -30);
                    XYZ up_direction = VectorFromHorizVertAngles(135, -30 + 90);
                    ViewOrientation3D orientation = new ViewOrientation3D(centroid, up_direction, forward_direction);
                    activeView.SetOrientation(orientation);
                    IList<UIView> views = uidoc.GetOpenUIViews();
                    foreach (UIView uvView in views)
                    {
                        if (uvView.ViewId.IntegerValue == activeView.Id.IntegerValue)
                        {
                            uvView.ZoomAndCenterRectangle(box.Min, box.Max);
                        }
                    }
                    return;
                }
            }
            catch (Exception) { }
        }
        private static XYZ VectorFromHorizVertAngles(double angleHorizD, double angleVertD)
        {
            double degToRadian = Math.PI * 2 / 360;
            double angleHorizR = angleHorizD * degToRadian;
            double angleVertR = angleVertD * degToRadian;
            double a = Math.Cos(angleVertR);
            double b = Math.Cos(angleHorizR);
            double c = Math.Sin(angleHorizR);
            double d = Math.Sin(angleVertR);
            return new XYZ(a * b, a * c, d);
        }
    }
}
