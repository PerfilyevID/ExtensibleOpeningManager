using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ExtensibleOpeningManager.Filters
{
    public class LinkedWallFilter : ISelectionFilter
    {
        private Document Doc { get; set; }
        public LinkedWallFilter(Document doc)
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
            if (element.Category.Id.IntegerValue == -2000011)
            {
                Wall wall = element as Wall;
                if ((wall.Location as LocationCurve).Curve.GetType() != typeof(Line))
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
    }
}
