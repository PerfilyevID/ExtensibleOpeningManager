using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ExtensibleOpeningManager.Filters
{
    public class LocalWallFilter : ISelectionFilter
    {
        private Document Doc { get; set; }
        public LocalWallFilter(Document doc)
        {
            Doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == -2000011)
            {
                Wall wall = elem as Wall;
                if ((wall.Location as LocationCurve).Curve.GetType() == typeof(Arc))
                {
                    return false;
                }
                if (wall.Width >= UserPreferences.MinWallWidth / 304.8)
                {
                    return true;
                }
                else
                {
                    return false;
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
