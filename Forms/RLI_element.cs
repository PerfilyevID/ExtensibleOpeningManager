using Autodesk.Revit.DB;

namespace ExtensibleOpeningManager.Forms
{
    public class RLI_element
    {
        public string Name { get; private set; }
        public string ToolTip { get; private set; }
        public bool IsEnabled { get; set; }
        public bool IsChecked { get; set; }
        public object Source { get; set; }
        public RLI_element(RevitLinkInstance instance)
        {
            Source = instance;
            Document doc = instance.GetLinkDocument();
            IsChecked = false;
            Name = string.Format("{0} [{1}]", instance.Name, instance.Id.ToString());
            if (doc != null)
            {
                IsEnabled = true;
                if (doc.IsWorkshared)
                {
                    ToolTip = doc.GetWorksharingCentralModelPath().CentralServerPath;
                }
                else
                {
                    ToolTip = doc.PathName;
                }
            }
            else
            {
                IsEnabled = false;
                ToolTip = "Документ не найден";
            }
        }
        public RLI_element(Level level)
        {
            Source = level;
            IsChecked = true;
            IsEnabled = true;
            string sign = string.Empty;
            if (level.Elevation >= 0)
            {
                sign = "+";
            }
            Name = string.Format("{0}: [на отм. {1}{2}mm]", level.Name, sign, (System.Math.Round(level.Elevation * 304.8, 2)).ToString());
            ToolTip = string.Format("Id элемента - {0}", level.Id.ToString());
        }

    }
}
