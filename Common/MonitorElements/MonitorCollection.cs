using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Common.MonitorElements
{
    public class MonitorCollection
    {
        public ObservableCollection<MonitorGroup> Collection = new ObservableCollection<MonitorGroup>();
        public MonitorCollection()
        {
            Collection.Add(new MonitorGroup(Collections.VisibleStatus.Ok, new List<ExtensibleElement>()));
            Collection.Add(new MonitorGroup(Collections.VisibleStatus.Warning, new List<ExtensibleElement>()));
            Collection.Add(new MonitorGroup(Collections.VisibleStatus.Alert, new List<ExtensibleElement>()));
        }
        public void AddElement(ExtensibleElement element, bool isExpanded=false)
        {
            foreach (MonitorGroup g in Collection)
            {
                if (g.Status == element.VisibleStatus)
                {
                    g.AddElement(element, isExpanded);
                }
            }
        }
        public void UpdateElement(ExtensibleElement element)
        {
            foreach (MonitorGroup g in Collection)
            {
                foreach (MonitorElement me in g.Collection)
                {
                    if (me.Id == element.Id)
                    {
                        if (element.VisibleStatus != g.Status)
                        {
                            bool isExpanded = g.IsExpanded;
                            g.RemoveId(new ElementId(element.Id));
                            AddElement(element, isExpanded);
                        }
                        else
                        {
                            UpdateElementLocally(element);
                        }
                        return;
                    }
                }
            }
        }
        private void UpdateElementLocally(ExtensibleElement element)
        {
            foreach (MonitorGroup g in Collection)
            {
                if (g.Status == element.VisibleStatus)
                {
                    g.UpdateElement(element);
                }
            }
        }
        public void RemoveId(ElementId id)
        {
            foreach (MonitorGroup g in Collection)
            {
                g.RemoveId(id);
            }
        }
    }
}
