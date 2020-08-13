using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Forms;
using KPLN_Loader.Common;
using static ExtensibleOpeningManager.ModuleData;
using static KPLN_Loader.Output.Output;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Interop;
using System.Windows;
using ExtensibleOpeningManager.Commands;
using System.Windows.Media.Imaging;

namespace ExtensibleOpeningManager
{
    public class Module : IExternalModule
    {
        public Result Close()
        {
            return Result.Succeeded;
        }
        public Result Execute(UIControlledApplication application, string tabName)
        {
            try
            {
                IntPtr MainWindowHandle = application.MainWindowHandle;
                IntPtr handle = MainWindowHandle;
                HwndSource hwndSource = HwndSource.FromHwnd(handle);
                RevitWindow = hwndSource.RootVisual as Window;
                string assembly = Assembly.GetExecutingAssembly().Location.Split(new string[] { "\\" }, StringSplitOptions.None).Last().Split('.').First();
                #region buttons
                RibbonPanel panel = application.CreateRibbonPanel(tabName, "Мониторинг отверстий");
                string description_manager = "...";
                switch (UserPreferences.Department)
                {
                    case Common.Collections.Department.MEP:
                        description_manager = "Открыть панель для расстановки и мониторинга расставленных заданий на отверстия";
                        break;
                    case Common.Collections.Department.AR:
                        description_manager = "Открыть панель для расстановки и мониторинга расставленных отверстий";
                        break;
                    case Common.Collections.Department.KR:
                        description_manager = "Открыть панель для расстановки и мониторинга расставленных отверстий";
                        break;
                    default:
                        break;
                }
                AddPushButtonData("Открыть менеджер отверстий", "Открыть\nменеджер", description_manager, string.Format("{0}.{1}", assembly, "ExternalCommands.CommandShowDockablePane"), panel, new Source.Source(Common.Collections.Icon.OpenManager), false);
                AddPushButtonData("Пользовательскте настройки модуля", "Настройки", "Открыть окно пользовательских настроек", string.Format("{0}.{1}", assembly, "ExternalCommands.CommandShowPreferences"), panel, new Source.Source(Common.Collections.Icon.Settings), true);
                #endregion
                RegisterPane(application);
                application.Idling += new EventHandler<IdlingEventArgs>(OnIdling);
                application.ControlledApplication.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
                application.ViewActivated += new EventHandler<ViewActivatedEventArgs>(OnViewActivated);
                UserPreferences.TryLoadParameters();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }

        }
        private void OnIdling(object sender, IdlingEventArgs args)
        {
            UIApplication uiapp = sender as UIApplication;
            UIControlledApplication controlledApplication = sender as UIControlledApplication;
            try
            {
                if (uiapp.ActiveUIDocument != null)
                {
                    if (uiapp.ActiveUIDocument.Document.IsFamilyDocument)
                    {
                        new CommandHidePane().Execute(uiapp);
                    }
                    else
                    {
                        if (SystemClosed)
                        {
                            new CommandShowPane().Execute(uiapp);
                        }
                    }
                }
            }
            catch (Exception e) { PrintError(e); }
            if (uiapp.ActiveUIDocument.Document.IsDetached || uiapp.ActiveUIDocument.Document.IsFamilyDocument)
            {
                CommandQueue.Clear();
                return; 
            }
            while (CommandQueue.Count != 0)
            {
                using (Transaction t = new Transaction(uiapp.ActiveUIDocument.Document, ModuleName))
                {
                    t.Start();
                    try
                    {
                        Result result = CommandQueue.Dequeue().Execute(uiapp);
                        try
                        {
                            uiapp.ActiveUIDocument.Document.Regenerate();
                            UiController.GetControllerByDocument(uiapp.ActiveUIDocument.Document).UpdateEnability();
                        }
                        catch (Exception e) { PrintError(e); }
                        if (result != Result.Succeeded)
                        {
                            t.RollBack();
                            break;
                        }
                        else
                        {
                            t.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        PrintError(e);
                        t.RollBack();
                        break;
                    }
                }
            }
            UiController.GetControllerByDocument(uiapp.ActiveUIDocument.Document).SetSelection(uiapp.ActiveUIDocument.Selection.GetElementIds());
        }
        private void OnDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            UIApplication uiapp = sender as UIApplication;
            UIControlledApplication controlledApplication = sender as UIControlledApplication;
            bool documentHasBeenModified = false;
            if (args.GetDeletedElementIds().Count != 0)
            {
                if (UiController.GetControllerByDocument(args.GetDocument()).ElementsInLinksCollection(args.GetDeletedElementIds()))
                {
                    UiController.GetControllerByDocument(args.GetDocument()).UpdateUpperElements();
                    UiController.GetControllerByDocument(args.GetDocument()).UpdateAllElements();
                    DockablePreferences.Page.UpdateItemscontroll();
                    return;
                }
                UiController.GetControllerByDocument(args.GetDocument()).OnElementsRemoved(args.GetDeletedElementIds());
                documentHasBeenModified = true;
            }
            if (args.GetAddedElementIds().Count != 0)
            {
                UiController.GetControllerByDocument(args.GetDocument()).OnElementsCreated(args.GetAddedElementIds());
                documentHasBeenModified = true;
            }
            if (args.GetModifiedElementIds().Count != 0)
            {
                if (UiController.GetControllerByDocument(args.GetDocument()).ElementsInLinksCollection(args.GetModifiedElementIds()))
                {
                    UiController.GetControllerByDocument(args.GetDocument()).UpdateUpperElements();
                    UiController.GetControllerByDocument(args.GetDocument()).UpdateAllElements();
                    DockablePreferences.Page.UpdateItemscontroll();
                }
                if (UiController.GetControllerByDocument(args.GetDocument()).ElementsInExtensibleCollection(args.GetModifiedElementIds()))
                {
                    UiController.GetControllerByDocument(args.GetDocument()).OnElementsChanged(args.GetModifiedElementIds());
                    documentHasBeenModified = true;
                }
            }
            if (documentHasBeenModified) { DockablePreferences.Page.UpdateItemscontroll(); }
            UiController.GetControllerByDocument(args.GetDocument()).UpdateRevitLinkInstances();
        }
        public static void OnViewActivated(object sender, ViewActivatedEventArgs args)
        {
            try
            {
                if (!args.Document.IsFamilyDocument)
                {

                    UiController.SetActiveDocument(args.CurrentActiveView.Document);
                    UiController.CurrentController.UpdateEnability();
                }
            }
            catch (Exception e)
            { PrintError(e); }
        }
        private void AddPushButtonData(string name, string text, string description, string className, RibbonPanel panel, Source.Source imageSource, bool avclass, string url = null)
        {
            PushButtonData data = new PushButtonData(name, text, Assembly.GetExecutingAssembly().Location, className);
            PushButton button = panel.AddItem(data) as PushButton;
            button.ToolTip = description;
            if (avclass)
            {
                button.AvailabilityClassName = "ExtensibleOpeningManager.Availability.StaticAvailable";
            }
            button.LongDescription = string.Format("Верстия: {0}\nСборка: {1}-{2}", ModuleData.Version, ModuleData.Build, ModuleData.Date);
            button.ItemText = text;
            if (url == null)
            {
                button.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, ModuleData.ManualPage));
            }
            else
            {
                button.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, url));
            }
            button.LargeImage = new BitmapImage(new Uri(imageSource.Value));
        }
        private static void RegisterPane(UIControlledApplication application)
        {
            try
            {
                DockablePaneProviderData dockablePaneProviderData = new DockablePaneProviderData();
                dockablePaneProviderData.FrameworkElement = DockablePreferences.Page;
                dockablePaneProviderData.InitialState = new DockablePaneState();
                dockablePaneProviderData.InitialState.DockPosition = DockPosition.Tabbed;
                dockablePaneProviderData.InitialState.MinimumWidth = 400;
                dockablePaneProviderData.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
                application.RegisterDockablePane(new DockablePaneId(DockablePreferences.PageGuid), "Мониторинг : Отверстия", DockablePreferences.Page as IDockablePaneProvider);
            }
            catch (Exception e) { PrintError(e); }
        }
    }
}
