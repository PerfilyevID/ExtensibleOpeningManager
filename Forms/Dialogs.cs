using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Forms
{
    public static class Dialogs
    {
        public static double Double = 0;
        public static string Header = "";
        public static string Body = "";
        public static double PickOffset()
        {
            Double = 0;
            OffsetPicker offsetPicker = new OffsetPicker();
            offsetPicker.ShowDialog();
            return Double;
        }
        public static void ShowDialog(string message, string header)
        {
            TaskDialog TD = new TaskDialog(header);
            TD.CommonButtons = TaskDialogCommonButtons.Ok;
            TD.MainContent = message;
            TD.TitleAutoPrefix = false;
            TD.Show();
        }
        public static string[] ShowRemarkDialog(RemarkType type)
        {
            Header = "";
            Body = "";
            RemarkForm RF = new RemarkForm(type);
            RF.ShowDialog();
            if (Header != "" && Body != "")
            {
                return new string[] { Header, Body };
            }
            return null;
        }
    }
}
