using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common.MonitorElements
{
    public class MonitorGroup
    {
        public string Header { get; }
        public Source.Source Source { get; }
        public string Count { get; set; }
        public ObservableCollection<MonitorElement> Collection { get; }
        public VisibleStatus Status { get; }
        public bool IsExpanded { get; set; }
        public MonitorGroup(VisibleStatus status, List<ExtensibleElement> elements)
        {
            Status = status;
            Collection = new ObservableCollection<MonitorElement>();
            switch (status)
            {
                case VisibleStatus.Ok:
                    Header = "Утверждено";
                    Source = new Source.Source(ImageMonitor.Ok);
                    break;
                case VisibleStatus.Alert:
                    Header = "Ошибки";
                    Source = new Source.Source(ImageMonitor.Error);
                    break;
                case VisibleStatus.Warning:
                    Header = "Предупреждения";
                    Source = new Source.Source(ImageMonitor.Warning);
                    break;
                default:
                    Header = Variables.empty;
                    break;
            }
            foreach (ExtensibleElement element in elements)
            {
                if (element.VisibleStatus == status)
                {
                    Collection.Add(new MonitorElement(element));
                }
            }
            Count = Collection.Count.ToString();
        }
        private MonitorElement GetElementById(int element)
        {
            foreach (MonitorElement m in Collection)
            {
                if (m.Id == element)
                {
                    return m;
                }
            }
            return null;
        }
        private int GetIndexById(int element)
        {
            int step = 0;
            foreach (MonitorElement m in Collection)
            {
                if (m.Id == element)
                {
                    return step;
                }
                step++;
            }
            return -1;
        }
        public void RemoveId(ElementId elementid)
        {
            MonitorElement m = GetElementById(elementid.IntegerValue);
            if (m != null)
            {
                Collection.Remove(m);
            }
            Count = Collection.Count.ToString();
            CollectionViewSource.GetDefaultView(DockablePreferences.Page.monitorView.ItemsSource).Refresh();
        }
        public void UpdateElement(ExtensibleElement element)
        {
            int m = GetIndexById(element.Id);
            if (m != -1)
            {
                bool IsEx = Collection[m].IsExpanded;
                Collection[m] = new MonitorElement(element) { IsExpanded = IsEx };
            }
            Count = Collection.Count.ToString();
            CollectionViewSource.GetDefaultView(DockablePreferences.Page.monitorView.ItemsSource).Refresh();
        }
        public void AddElement(ExtensibleElement element, bool isExpanded = false)
        {
            if (element.VisibleStatus == Status)
            {
                Collection.Add(new MonitorElement(element) { IsExpanded = isExpanded });
            }
            Count = Collection.Count.ToString();
            CollectionViewSource.GetDefaultView(DockablePreferences.Page.monitorView.ItemsSource).Refresh();
        }
    }
}
