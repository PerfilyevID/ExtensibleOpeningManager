using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ExtensibleOpeningManager.Filters
{
    public class LocalInstanceFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == -2001140 && elem.GetType() == typeof(FamilyInstance))
            {
                if (UserPreferences.Department == Common.Collections.Department.AR)
                {
                    FamilyInstance instance = elem as FamilyInstance;
                    if (instance.Symbol.FamilyName != Variables.family_kr_round || instance.Symbol.FamilyName != Variables.family_kr_square || instance.Symbol.FamilyName == Variables.family_mep_round || instance.Symbol.FamilyName == Variables.family_mep_square)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (UserPreferences.Department == Common.Collections.Department.KR)
                {
                    FamilyInstance instance = elem as FamilyInstance;
                    if (instance.Symbol.FamilyName == Variables.family_ar_round || instance.Symbol.FamilyName == Variables.family_ar_square || instance.Symbol.FamilyName == Variables.family_mep_round || instance.Symbol.FamilyName == Variables.family_mep_square)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (UserPreferences.Department == Common.Collections.Department.MEP)
                {
                    FamilyInstance instance = elem as FamilyInstance;
                    if (instance.Symbol.FamilyName == Variables.family_ar_round || instance.Symbol.FamilyName == Variables.family_ar_square || instance.Symbol.FamilyName != Variables.family_kr_round || instance.Symbol.FamilyName != Variables.family_kr_square)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
