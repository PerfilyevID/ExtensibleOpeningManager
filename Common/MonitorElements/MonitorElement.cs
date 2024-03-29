﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using static ExtensibleOpeningManager.Common.Collections;

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
        public ExtensibleElement Element { get; set; }
        public MonitorElement(ExtensibleElement element)
        {
            Element = element;
            List<string> tipParts = new List<string>();
            if (element.HasUnfoundSubElements() || element.WallStatus == WallStatus.NotFound || element.Status == Status.Null)
            {
                Source = new Source.Source(Collections.ImageMonitor.Error);
                if (element.Status == Status.Null)
                { tipParts.Add("Элемент: Мониторинг отсутствует"); }
                if (element.WallStatus == WallStatus.NotFound)
                { tipParts.Add("Стена-основа: Не найдена"); }
                if (element.HasUnfoundSubElements())
                { tipParts.Add("Субэлементы: Ненайденные элементы"); }
            }
            else
            {
                if (element.HasUncommitedSubElements() || element.ToString() != element.SavedData || element.Status != Status.Applied || element.WallStatus == WallStatus.NotCommited || element.ActiveRemarks.Count != 0)
                {
                    Source = new Source.Source(Collections.ImageMonitor.Warning);
                    if (element.Status != Status.Applied)
                    { tipParts.Add("Элемент: Не одобрен"); }
                    else if (element.ToString() != element.SavedData)
                    { tipParts.Add("Элемент: Неутвержденные изменения"); }
                    if (element.WallStatus == WallStatus.NotCommited)
                    { tipParts.Add("Стена-основа: Неутвержденные изменения"); }
                    if (element.HasUncommitedSubElements())
                    { tipParts.Add("Субэлементы: Неутвержденные изменения"); }
                }
                else
                {
                    if (element.ActiveRemarks.Count != 0)
                    {
                        Source = new Source.Source(Collections.ImageMonitor.Warning);
                    }
                    else
                    {
                        Source = new Source.Source(Collections.ImageMonitor.Ok);
                    }
                }
            }
            if (element.ActiveRemarks.Count != 0)
            {
                tipParts.Add("Есть незакрытые замечания");
            }
            if (tipParts.Count == 0)
            {
                tipParts.Add("Без предупреждений!");
            }
            ToolTip = string.Join("\n", tipParts);
            Collection = new ObservableCollection<MonitorSubElement>();
            Id = element.Id;
            Name = string.Format("{0}: {1} [{2}.rfa]", element.Id, element.Instance.Symbol.Name, element.Instance.Symbol.FamilyName);
            Collection.Add(new MonitorSubElement(element.Wall, element.WallStatus, this, element.Status != Status.Null));
            if (element.SubElements.Count == 0)
            {
                Collection.Add(new MonitorSubElement(this));
            }
            else
            {
                foreach (ExtensibleSubElement subElement in element.SubElements)
                {
                    Collection.Add(new MonitorSubElement(subElement, this));
                }
            }
            foreach (ExtensibleRemark rmrk in element.ActiveRemarks)
            {
                Collection.Add(new MonitorSubElement(this, rmrk));
            }
        }
    }
}
