﻿using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using KPLN_Loader.Common;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandApplySubElements : IExecutableCommand
    {
        public CommandApplySubElements(ExtensibleElement instance)
        {
            Element = instance;
        }
        private ExtensibleElement Element { get; }
        public Result Execute(UIApplication app)
        {
            Element.ApplySubElements();
            return Result.Succeeded;
        }
    }
}
