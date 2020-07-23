using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using KPLN_Loader.Common;
using System;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandAddComment : IExecutableCommand
    {
        public CommandAddComment(ExtensibleElement element, string message)
        {
            Element = element;
            Message = message;
        }
        private ExtensibleElement Element { get; }
        private string Message { get; }
        public Result Execute(UIApplication app)
        {
            Element.AddComment(Message);
            try
            {
                UiController controller = UiController.GetControllerByDocument(Element.Instance.Document);
                controller.UpdateComments(controller.Selection[0].AllComments);
            }
            catch (Exception e)
            { PrintError(e); }
            return Result.Succeeded;
        }
    }
}
