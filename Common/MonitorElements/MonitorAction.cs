using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Common.MonitorElements
{
    public class MonitorAction
    {
        public MonitorAction(string buttonText, string toolTip, Brush color)
        {
            Color = color;
            ButtonText = buttonText;
            ToolTip = toolTip;
        }
        public string ButtonText { get; private set; }
        public bool IsEnabled { get; set; }
        public Brush Color { get; private set; }
        public string ToolTip { get; private set; }
    }
}
