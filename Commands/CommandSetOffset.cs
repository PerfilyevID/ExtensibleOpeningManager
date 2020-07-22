using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Forms;
using KPLN_Loader.Common;
using System.Collections.Generic;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandSetOffset : IExecutableCommand
    {
        public CommandSetOffset(List<ExtensibleElement> element)
        {
            Elements = element;
        }
        private List<ExtensibleElement> Elements { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                double value = Dialogs.PickOffset() / 304.8;
                foreach (ExtensibleElement element in Elements)
                {
                    element.Instance.LookupParameter(Variables.parameter_offset_bounds).Set(value);
                }
                return Result.Succeeded;
            }
            catch (System.Exception)
            {
                return Result.Failed;
            }

        }
    }
}
