using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Common.MonitorElements
{
    public class MonitorSubElement
    {
        public string Name { get; }
        public string ToolTip { get; }
        public int Id { get; }
        public int LinkId { get; }
        public Source.Source Source { get; }
        public bool IsExpanded { get; set; }
        public MonitorSubElement()
        {
            Source = new Source.Source(Collections.ImageMonitor.Element_Errored);
            ToolTip = "Отсутствуют субэлементы";
            Id = -1;
            LinkId = -1;
            Name = "<Пусто>";
        }
        public MonitorSubElement(SE_LinkedWall wall, WallStatus status)
        {
            switch (status)
            {
                case WallStatus.Ok:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Approved);
                    ToolTip = "Без предупреждений";
                    Id = -1;
                    LinkId = -1;
                    Name = string.Format("Стена: {1}: <{0}>", wall.Wall.Name, wall.Wall.Id.ToString());
                    break;
                case WallStatus.NotFound:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Errored);
                    ToolTip = "Не найдена";
                    Id = -1;
                    LinkId = -1;
                    Name = "Стена: <Не найдена>";
                    break;
                case WallStatus.NotCommited:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Unapproved);
                    ToolTip = "Неутвержденные изменения";
                    Id = -1;
                    LinkId = -1;
                    Name = string.Format("Стена: {1}: <{0}>", wall.Wall.Name, wall.Wall.Id.ToString());
                    break;
                default:
                    break;
            }

        }
        public MonitorSubElement(ExtensibleSubElement subElement)
        {
            switch (subElement.Status)
            {
                case Collections.SubStatus.Applied:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Approved);
                    ToolTip = "Утвержден";
                    break;
                case Collections.SubStatus.Changed:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Unapproved);
                    ToolTip = "Неутвержденные изменения";
                    break;
                case Collections.SubStatus.NotFound:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Errored);
                    ToolTip = "Элемент или его документ не найден";
                    break;
                case Collections.SubStatus.Rejected:
                    Source = new Source.Source(Collections.ImageMonitor.Element_Unapproved);
                    ToolTip = "Элемент не одобрен";
                    break;
                default:
                    break;
            }
            if (subElement.Status != Collections.SubStatus.NotFound)
            {
                Id = subElement.Element.Id.IntegerValue;
                LinkId = subElement.LinkId.IntegerValue;
                if (subElement.GetType() == typeof(SE_LinkedInstance))
                {
                    Name = string.Format("{2}: {0} [{1}.rfa]", (subElement.Element as FamilyInstance).Symbol.Name, (subElement.Element as FamilyInstance).Symbol.FamilyName, subElement.Element.Id.ToString());
                }
                else
                {
                    Name = string.Format("{1}: <{0}>", subElement.Element.Category.Name, subElement.Element.Id.ToString());
                }
            }
            else
            {
                Id = -1;
                LinkId = -1;
                Name = "Не найден";
            }
        }
    }
}
