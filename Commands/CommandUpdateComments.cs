using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandUpdateComments : IExecutableCommand
    {
        public CommandUpdateComments(ExtensibleElement element)
        {
            Messages = element.AllComments;
            Document = element.Instance.Document;
        }
        private List<ExtensibleMessage> Messages { get; set; }
        private Document Document { get; set; }
        public Result Execute(UIApplication app)
        {
            try
            {
                UiController.GetControllerByDocument(Document).UpdateComments(Messages);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
