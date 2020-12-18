using System.IO;
using System.Reflection;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Source
{
    public class Source
    {
        public string Value { get; }
        private static string AssemblyPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
        public Source(Icon icon)
        {
            switch (icon)
            {
                case Icon.OpenManager:
                    Value = Path.Combine(AssemblyPath, @"Source\icon_manager.png");
                    break;
                case Icon.Settings:
                    Value = Path.Combine(AssemblyPath, @"Source\icon_setup.png");
                    break;
            }
        }
        public Source(ImageButton image)
        {
            switch (image)
            {
                case ImageButton.Approve:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Approve.png");
                    break;
                case ImageButton.Reject:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Reject.png");
                    break;
                case ImageButton.Group:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Group.png");
                    break;
                case ImageButton.Ungroup:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Ungroup.png");
                    break;
                case ImageButton.SetOffset:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Offset.png");
                    break;
                case ImageButton.Apply:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\ApproveInstance.png");
                    break;
                case ImageButton.ApplyWall:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\ApproveWall.png");
                    break;
                case ImageButton.ApplySubElements:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\ApproveHost.png");
                    break;
                case ImageButton.AddSubElements:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\SetMonitoring.png");
                    break;
                case ImageButton.SetWall:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\SetWall.png");
                    break;
                case ImageButton.Reset:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Reset.png");
                    break;
                case ImageButton.Update:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\Update.png");
                    break;
                case ImageButton.Swap:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\SwapType.png");
                    break;
                case ImageButton.FindSubelements:
                    Value = Path.Combine(AssemblyPath, @"Source\Buttons\FindSubelements.png");
                    break;
                default:
                    break;
            }
        }
        public Source(ImageMonitor image)
        {
            switch (image)
            {
                case ImageMonitor.Element_Approved:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Task_Approved.png");
                    break;
                case ImageMonitor.Element_Errored:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Task_Errored.png");
                    break;
                case ImageMonitor.Element_Unapproved:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Task_Unapproved.png");
                    break;
                case ImageMonitor.Request:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Request.png");
                    break;
                case ImageMonitor.Error:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Error.png");
                    break;
                case ImageMonitor.Ok:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Ok.png");
                    break;
                case ImageMonitor.Remove:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Remove.png");
                    break;
                case ImageMonitor.Update:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Update.png");
                    break;
                case ImageMonitor.Waiting:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Waiting.png");
                    break;
                case ImageMonitor.Warning:
                    Value = Path.Combine(AssemblyPath, @"Source\Monitor\Icon_Warning.png");
                    break;
                default:
                    break;
            }
        }
    }
}
