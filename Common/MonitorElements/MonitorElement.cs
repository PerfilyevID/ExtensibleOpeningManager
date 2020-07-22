using System.Collections.Generic;
using System.Collections.ObjectModel;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common.MonitorElements
{
    public class MonitorElement
    {
        public int Id { get; }
        public string Name { get; }
        public string ToolTip { get; }
        public Source.Source Source { get; }
        public ObservableCollection<MonitorSubElement> Collection { get; }
        public bool IsExpanded { get; set; }
        public MonitorElement(ExtensibleElement element)
        {
            switch (element.Status)
            {
                case Collections.Status.Applied:
                    Source = new Source.Source(Collections.ImageMonitor.Ok);
                    ToolTip = "Элемент утвержден";
                    break;
                case Collections.Status.Rejected:
                    Source = new Source.Source(Collections.ImageMonitor.Error);
                    ToolTip = "Элемент не утвержден";
                    break;
                default:
                    Source = new Source.Source(Collections.ImageMonitor.Error);
                    ToolTip = "Мониторинг отсутствует";
                    break;
            }
            Collection = new ObservableCollection<MonitorSubElement>();
            Id = element.Id;
            Name = string.Format("{0}: {1} [{2}.rfa]", element.Id, element.Instance.Symbol.Name, element.Instance.Symbol.FamilyName);
            Collection.Add(new MonitorSubElement(element.Wall, element.WallStatus));
            if (element.SubElements.Count == 0)
            {
                Collection.Add(new MonitorSubElement());
            }
            else
            {
                foreach (ExtensibleSubElement subElement in element.SubElements)
                {
                    Collection.Add(new MonitorSubElement(subElement));
                }
            }
        }
    }
}
