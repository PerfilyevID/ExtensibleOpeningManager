using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Tools;
using KPLN_Loader.Common;
using System;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandAddSubElement : IExecutableCommand
    {
        public CommandAddSubElement(ExtensibleElement instance)
        {
            Element = instance;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            try
            {
                if (UserPreferences.Department == Collections.Department.MEP)
                {
                    Element.AddSubElement(UiTools.PickInstance(app, Collections.PickTypeOptions.Element, Collections.PickOptions.Local));
                    return Result.Succeeded;
                }
                else
                {
                    Element.AddSubElement(UiTools.PickInstance(app, Collections.PickTypeOptions.Instance, Collections.PickOptions.References));
                    return Result.Succeeded;
                }
            }
            catch (Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }
        }
    }
}
