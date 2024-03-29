﻿using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Controll;
using KPLN_Loader.Common;
using System;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandLoopApprove : IExecutableCommand
    {
        public CommandLoopApprove(LoopController controller)
        {
            Controller = controller;
        }
        private LoopController Controller { get; set; }
        public Result Execute(UIApplication app)
        {
            try
            {
                foreach (ExtensibleElement element in Controller.CreatedElements)
                {
                    try
                    {
                        element.Apply();
                    }
                    catch (Exception) { }
                }
                Controller.CreatedElements.Clear();
                Controller.Next();
                return Result.Succeeded;
            }
            catch (Exception)
            {
                Controller.CreatedElements.Clear();
                Controller.Next();
                return Result.Failed;
            }
        }
    }
}
