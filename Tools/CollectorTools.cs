﻿using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Filters;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Tools
{
    public static class CollectorTools
    {
        public static RevitLinkInstance GetRevitLinkById(ElementId id, Document doc)
        {
            foreach (Element e in GetRevitLinks(doc))
            { 
                if (e.Id.IntegerValue == id.IntegerValue && e.GetType() == typeof(RevitLinkInstance))
                {
                    return e as RevitLinkInstance;
                }
            }
            return null;
        }
        public static List<RevitLinkInstance> GetRevitLinks(Document doc)
        {
            List<RevitLinkInstance> instances = new List<RevitLinkInstance>();
            foreach (Element e in new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).WhereElementIsNotElementType().ToElements())
            {
                try
                {
                    instances.Add(e as RevitLinkInstance);
                }
                catch (Exception ex) { PrintError(ex); }
            }
            return instances;
        }
        private static BuiltInCategory[] CategoryListMEP = new BuiltInCategory[] { BuiltInCategory.OST_DuctCurves,
                                    BuiltInCategory.OST_PipeCurves,
                                    BuiltInCategory.OST_Conduit,
                                    BuiltInCategory.OST_CableTray,
                                    BuiltInCategory.OST_FlexDuctCurves,
                                    BuiltInCategory.OST_DuctFitting,
                                    BuiltInCategory.OST_ConduitFitting,
                                    BuiltInCategory.OST_PipeFitting};
        public static List<Element> GetMepElements(Document doc)
        {
            List<Element> instances = new List<Element>();
            foreach (BuiltInCategory category in CategoryListMEP)
            {
                foreach (Element e in new FilteredElementCollector(doc).OfCategory(category).WhereElementIsNotElementType().ToElements())
                {
                    try
                    {
                        instances.Add(e);
                    }
                    catch (Exception ex) { PrintError(ex); }
                }
            }
            return instances;
        }
        public static List<ExtensibleElement> GetInstances(Document doc)
        {
            List<ExtensibleElement> instances = new List<ExtensibleElement>();
            foreach (Element e in new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().ToElements())
            {
                try
                {
                    string name = (e as FamilyInstance).Symbol.FamilyName;
                    if (UserPreferences.Department == Collections.Department.AR && (name == Variables.family_ar_round || name == Variables.family_ar_square))
                    {
                        instances.Add(ExtensibleElement.GetExtensibleElementByInstance(e as FamilyInstance));
                        continue;
                    }
                    if (UserPreferences.Department == Collections.Department.KR && (name == Variables.family_kr_round || name == Variables.family_kr_square))
                    {
                        instances.Add(ExtensibleElement.GetExtensibleElementByInstance(e as FamilyInstance));
                        continue;
                    }
                    if (UserPreferences.Department == Collections.Department.MEP && (name == Variables.family_mep_round || name == Variables.family_mep_square))
                    {
                        instances.Add(ExtensibleElement.GetExtensibleElementByInstance(e as FamilyInstance));
                        continue;
                    }
                }
                catch (Exception ex) { PrintError(ex); }
            }
            return instances;
        }
        public static List<Wall> GetWalls(Document doc)
        {
            List<Wall> instances = new List<Wall>();
            foreach (Element e in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements())
            {
                try
                {
                    if (LocalFilter.Passes(e as Wall))
                    {
                        instances.Add(e as Wall);
                    }
                }
                catch (Exception ex) { PrintError(ex); }
            }
            return instances;
        }
        public static List<SE_LinkedWall> GetLinkedWalls(Document doc, RevitLinkInstance instance)
        {
            List<SE_LinkedWall> instances = new List<SE_LinkedWall>();
            foreach (Element e in new FilteredElementCollector(instance.GetLinkDocument()).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements())
            {
                try
                {
                    if (LocalFilter.Passes(e as Wall))
                    {
                        instances.Add(new SE_LinkedWall(instance, e as Wall));
                    }
                }
                catch (Exception ex) { PrintError(ex); }
            }
            return instances;
        }
        public static List<Level> GetLevels(Document doc)
        {
            List<Level> instances = new List<Level>();
            foreach (Element e in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements())
            {
                try
                {
                    instances.Add(e as Level);
                }
                catch (Exception ex) { PrintError(ex); }
            }
            return instances;
        }
    }
}