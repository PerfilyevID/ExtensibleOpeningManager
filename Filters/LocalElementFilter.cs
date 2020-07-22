using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace ExtensibleOpeningManager.Filters
{
    public class LocalElementFilter : ISelectionFilter
    {
        private static BuiltInCategory[] CategoryListMEP = new BuiltInCategory[] { BuiltInCategory.OST_DuctCurves,
                                    BuiltInCategory.OST_PipeCurves,
                                    BuiltInCategory.OST_Conduit,
                                    BuiltInCategory.OST_CableTray,
                                    BuiltInCategory.OST_FlexDuctCurves,
                                    BuiltInCategory.OST_DuctFitting,
                                    BuiltInCategory.OST_ConduitFitting,
                                    BuiltInCategory.OST_PipeFitting};
        public static List<int> GetAllowedCategories(Document doc)
        {
            List<int> categories = new List<int>();
            foreach (BuiltInCategory cat in CategoryListMEP)
            {
                categories.Add(Category.GetCategory(doc, cat).Id.IntegerValue);
            }
            return categories;
        }
        private Document Doc { get; set; }
        public LocalElementFilter(Document doc)
        {
            Doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            if (GetAllowedCategories(Doc).Contains(elem.Category.Id.IntegerValue))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
