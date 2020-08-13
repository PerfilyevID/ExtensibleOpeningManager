using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Forms;
using KPLN_Loader.Common;
using System;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandRejectRemark : IExecutableCommand
	{
		public CommandRejectRemark(ExtensibleElement element, ExtensibleRemark remark)
		{
			Element = element;
			Remark = remark;
		}
		private ExtensibleElement Element { get; }
		private ExtensibleRemark Remark { get; }
		public Result Execute(UIApplication app)
		{
			try
			{
				string[] value = Dialogs.ShowRemarkDialog(Common.Collections.RemarkType.Answer_No);
				if (value != null)
				{
					Element.RejectRemark(value[0], value[1], Remark);
				}
				else
				{
					return Result.Cancelled;
				}
				return Result.Succeeded;
			}
			catch (Exception)
			{
				return Result.Failed;
			}
		}
	}
}
