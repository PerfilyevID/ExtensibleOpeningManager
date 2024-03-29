﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Forms;
using System;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CommandShowDockablePane : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (commandData.Application.ActiveUIDocument.Document.IsFamilyDocument)
                {
                    return Result.Cancelled;
                }
                Document doc = commandData.Application.ActiveUIDocument.Document;
                commandData.Application.GetDockablePane(new DockablePaneId(DockablePreferences.PageGuid)).Show();
                ModuleData.SystemClosed = false;
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
