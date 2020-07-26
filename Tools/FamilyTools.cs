﻿using Autodesk.Revit.DB;
using System;
using System.IO;
using System.Reflection;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Tools
{
    public static class FamilyTools
    {
        public static FamilySymbol GetSwapType(FamilyInstance instance)
        {
            if (instance.Symbol.FamilyName == Variables.family_ar_round)
            {
                return GetFamilySymbol(instance.Document, Variables.family_ar_square, instance.Symbol.Name);
            }
            if (instance.Symbol.FamilyName == Variables.family_ar_square)
            {
                return GetFamilySymbol(instance.Document, Variables.family_ar_round, instance.Symbol.Name);
            }
            if (instance.Symbol.FamilyName == Variables.family_kr_round)
            {
                return GetFamilySymbol(instance.Document, Variables.family_kr_square, instance.Symbol.Name);
            }
            if (instance.Symbol.FamilyName == Variables.family_kr_square)
            {
                return GetFamilySymbol(instance.Document, Variables.family_kr_round, instance.Symbol.Name);
            }
            if (instance.Symbol.FamilyName == Variables.family_mep_round)
            {
                return GetFamilySymbol(instance.Document, Variables.family_mep_square, instance.Symbol.Name);
            }
            if (instance.Symbol.FamilyName == Variables.family_mep_square)
            {
                return GetFamilySymbol(instance.Document, Variables.family_mep_round, instance.Symbol.Name);
            }
            return null;
        }
        public static FamilySymbol GetFamilySymbol(Document doc, string familyName, string symbolName)
        {
            foreach (Element element in new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment))
            {
                FamilySymbol searchSymbol = element as FamilySymbol;
                if (searchSymbol.FamilyName == familyName && searchSymbol.Name == symbolName)
                {
                    searchSymbol.Activate();
                    Print("Найден с первого раза", KPLN_Loader.Preferences.MessageType.Error);
                    return searchSymbol;
                }
            }
            try
            {
                Print("Загрузка...", KPLN_Loader.Preferences.MessageType.Error);
                doc.LoadFamily(string.Format(@"{0}\Source\RevitData\{1}.rfa", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString(), familyName));
                doc.Regenerate();
            }
            catch (Exception e)
            { PrintError(e); }
            foreach (Element element in new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment))
            {
                FamilySymbol searchSymbol = element as FamilySymbol;
                if (searchSymbol.FamilyName == familyName && searchSymbol.Name == symbolName)
                {
                    searchSymbol.Activate();
                    Print("Найден!", KPLN_Loader.Preferences.MessageType.Error);
                    return searchSymbol;
                }
            }
            foreach (Element element in new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment))
            {
                FamilySymbol searchSymbol = element as FamilySymbol;
                if (searchSymbol.FamilyName == familyName)
                {
                    searchSymbol.Activate();
                    Print("Найден по имени семейства", KPLN_Loader.Preferences.MessageType.Error);
                    return searchSymbol;
                }
            }
            Print("Null", KPLN_Loader.Preferences.MessageType.Error);
            return null;
        }
    }
}
