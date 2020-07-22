using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ExtensibleOpeningManager.Filters
{
    public class LinkedInstanceFilter : ISelectionFilter
    {
        private Document Doc { get; set; }
        public LinkedInstanceFilter(Document doc)
        {
            Doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            return true;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            RevitLinkInstance linkInstance = Doc.GetElement(reference) as RevitLinkInstance;
            Document linkDocument = linkInstance.GetLinkDocument();
            Element element = linkDocument.GetElement(reference.LinkedElementId);
            if (element.Category.Id.IntegerValue == -2001140 && element.GetType() == typeof(FamilyInstance))
            {
                if (UserPreferences.Department == Common.Collections.Department.AR)
                {
                    FamilyInstance instance = element as FamilyInstance;
                    if (instance.Symbol.FamilyName != Variables.family_ar_round && instance.Symbol.FamilyName != Variables.family_ar_square)
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
                    FamilyInstance instance = element as FamilyInstance;
                    if (instance.Symbol.FamilyName != Variables.family_kr_round && instance.Symbol.FamilyName != Variables.family_kr_square)
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
                    FamilyInstance instance = element as FamilyInstance;
                    if (instance.Symbol.FamilyName != Variables.family_mep_round && instance.Symbol.FamilyName != Variables.family_mep_square)
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
    }
}
