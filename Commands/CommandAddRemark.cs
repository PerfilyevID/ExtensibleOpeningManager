using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Forms;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandAddRemark : IExecutableCommand
    {
		public CommandAddRemark(ExtensibleElement element, SE_LinkedInstance subElement)
		{
			Element = element;
			SubElement = subElement;
		}
		private ExtensibleElement Element { get; }
		private SE_LinkedInstance SubElement { get; }
		public Result Execute(UIApplication app)
        {
			try
			{
				string[] value = Dialogs.ShowRemarkDialog(Common.Collections.RemarkType.Request);
				if (value != null)
				{
					Element.AddRemark(value[0], value[1], Collections.RemarkType.Request, SubElement);
				}
				else
				{
					return Result.Cancelled;
				}
				return Result.Succeeded;
			}
			catch (Exception e)
			{
				PrintError(e);
				return Result.Failed;
			}
        }
    }
}
