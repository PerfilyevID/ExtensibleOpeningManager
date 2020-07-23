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
using ExtensibleOpeningManager.Tools;
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
                AddPushButtonData("Открыть менеджер отверстий", "Открыть\nменеджер", "...", string.Format("{0}.{1}", assembly, "ExternalCommands.CommandShowDockablePane"), panel, new Source.Source(Common.Collections.Icon.OpenManager));
                AddPushButtonData("Пользовательскте настройки модуля", "Настройки", "...", string.Format("{0}.{1}", assembly, "ExternalCommands.CommandShowPreferences"), panel, new Source.Source(Common.Collections.Icon.Settings));
                #endregion
                RegisterPane(application);
                application.Idling += new EventHandler<IdlingEventArgs>(OnIdling);
                application.ControlledApplication.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
                application.ViewActivated += new EventHandler<ViewActivatedEventArgs>(OnViewActivated);
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
            UiController.GetControllerByDocument(args.GetDocument()).UpdateRevitLinkInstances();
            if (args.GetDeletedElementIds().Count != 0)
            {
                if (UiController.GetControllerByDocument(args.GetDocument()).ElementsInLinksCollection(args.GetDeletedElementIds()))
                {
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
                    UiController.GetControllerByDocument(args.GetDocument()).UpdateAllElements();
                    DockablePreferences.Page.UpdateItemscontroll();
                    return;
                }
                if (UiController.GetControllerByDocument(args.GetDocument()).ElementsInExtensibleCollection(args.GetModifiedElementIds()))
                {
                    UiController.GetControllerByDocument(args.GetDocument()).OnElementsChanged(args.GetModifiedElementIds());
                    documentHasBeenModified = true;
                }
            }
            if (documentHasBeenModified) { DockablePreferences.Page.UpdateItemscontroll(); }
        }
        public static void OnViewActivated(object sender, ViewActivatedEventArgs args)
        {
            if (!args.Document.IsFamilyDocument)
            { 
                UiController.SetActiveDocument(args.Document);
                UiController.CurrentController.UpdateEnability();
            }
        }
        private void AddPushButtonData(string name, string text, string description, string className, RibbonPanel panel, Source.Source imageSource, string url = @"https://kpln.kdb24.ru/article/76440/")
        {
            PushButtonData data = new PushButtonData(name, text, Assembly.GetExecutingAssembly().Location, className);
            PushButton button = panel.AddItem(data) as PushButton;
            button.ToolTip = description;
            button.LongDescription = string.Format("{0}", Assembly.GetExecutingAssembly().Location);
            button.ItemText = text;
            button.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, url));
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
                dockablePaneProviderData.InitialState.MinimumWidth = 200;
                dockablePaneProviderData.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
                application.RegisterDockablePane(new DockablePaneId(DockablePreferences.PageGuid), "Мониторинг : Отверстия", DockablePreferences.Page as IDockablePaneProvider);
            }
            catch (Exception e) { PrintError(e); }
        }
    }
}
