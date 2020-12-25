using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Tools.Instances
{
    public class SizeOptions
    {
        public double MinWidth { get; }
        public double MinHeight { get; }
        public double MaxWidth { get; }
        public double MaxHeight { get; }
        public SizeOptions(double minWidth, double minHeight, double maxWidth, double maxHeight)
        {
            MinWidth = minWidth / 304.8;
            MinHeight = minHeight / 304.8;
            MaxWidth = maxWidth / 304.8;
            MaxHeight = maxHeight / 304.8;
        }
    }
}
