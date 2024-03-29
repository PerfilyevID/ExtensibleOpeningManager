﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager;
using ExtensibleOpeningManager.Commands;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Common.MonitorElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Forms;
using ExtensibleOpeningManager.Source;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static KPLN_Loader.Output.Output;

namespace DockableDialog.Forms
{
    public partial class DockableManager : Page, IDockablePaneProvider
    {
        public DockableManager()
        {
            InitializeComponent();
            btnAddSubElement.DataContext = new Source( ExtensibleOpeningManager.Common.Collections.ImageButton.AddSubElements);
            btnApplySubElements.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.ApplySubElements);
            btnApplyWall.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.ApplyWall);
            btnApprove.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Approve);
            btnGroup.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Group);
            btnReject.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Reject);
            btnReset.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Reset);
            btnSetOffset.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.SetOffset);
            btnSetWall.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.SetWall);
            btnSwap.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Swap);
            btnUngroup.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Ungroup);
            btnUpdate.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.Update);
            btnFindSubelements.DataContext = new Source(ExtensibleOpeningManager.Common.Collections.ImageButton.FindSubelements);
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Tabbed;
            data.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
        }
        public void UpdateItemscontroll()
        {
            CollectionViewSource.GetDefaultView(monitorView.ItemsSource).Refresh();
        }
        private void OnItemDoubleClick(object sender, MouseButtonEventArgs args)
        {
            try
            {
                TreeViewItem item = sender as TreeViewItem;
                if (item == null){ return; }
                MonitorElement moniterElement = item.DataContext as MonitorElement;
                if (moniterElement == null) { return; }
                if (moniterElement.Element == null) { return; }
                if (moniterElement.Element.Solid == null) { return; }
                ModuleData.CommandQueue.Enqueue(new CommandZoomElement(moniterElement.Element));
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }
        private void OnBtnApprove(object sender, RoutedEventArgs e)
        {
            foreach (ExtensibleElement element in UiController.CurrentController.Selection)
            {
                ModuleData.CommandQueue.Enqueue(new CommandApprove(element));
            }
        }
        private void OnBtnReject(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandReject(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnSetOffset(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandSetOffset(UiController.CurrentController.Selection));
        }
        private void OnBtnGroup(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandGroupInstances(UiController.CurrentController.Selection));
        }
        private void OnBtnUngroup(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandUngroup(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnReset(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandReset(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnUpdate(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandUpdate(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnApplySubElements(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandApplySubElements(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnApplyWall(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandApplyWall(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnAddSubElement(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandAddSubElement(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnSetWall(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandSetWall(UiController.CurrentController.Selection[0]));
        }
        private void OnBtnSwap(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandSwapType(UiController.CurrentController.Selection[0]));
        }
        //До тех пор пока не появится плагин для КР
        /*
        private void OnBtnPlaceOnKR(object sender, RoutedEventArgs e)
        {

        }
        private void OnBtnPlaceOnAR(object sender, RoutedEventArgs e)
        {

        }
        */
        private void OnBtnPlaceOnMEP(object sender, RoutedEventArgs args)
        {
            List<RevitLinkInstance> links = CollectorTools.GetRevitLinks(UiController.CurrentController.Document);
            if (links.Count != 0)
            {
                try
                {
                    UpdateByDocument form = new UpdateByDocument(UiController.CurrentController.Document, links);
                    form.ShowDialog();
                }
                catch (Exception e)
                {
                    PrintError(e);
                }
                
            }
            else
            {
                Dialogs.ShowDialog("В проекте отсутствуют подгруженные связи", "Ошибка");
            }
        }
        private void OnBtnLoop(object sender, RoutedEventArgs args)
        {
            List<RevitLinkInstance> links = CollectorTools.GetRevitLinks(UiController.CurrentController.Document);
            if (links.Count != 0)
            {
                RevitLinkPicker linkPicker = new RevitLinkPicker(links, "Расчет пересечений", true);
                linkPicker.ShowDialog();
                if (RevitLinkPicker.PickedRevitLinkInstances != null)
                {
                    if (RevitLinkPicker.PickedRevitLinkInstances.Count != 0)
                    {
                        if (RevitLinkPicker.PickedRevitLinkInstances.Count > 1)
                        {
                            try
                            {
                                UiController.CurrentController.LoopController.Prepare(RevitLinkPicker.PickedRevitLinkInstances);
                            }
                            catch (Exception e)
                            {
                                PrintError(e);
                            }
                        }
                        if (RevitLinkPicker.PickedRevitLinkInstances.Count == 1)
                        {
                            RevitLinkInstance link = RevitLinkPicker.PickedRevitLinkInstances[0];
                            Document doc = link.GetLinkDocument();
                            List<Level> levels = new List<Level>();
                            foreach (Level l in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements())
                            {
                                levels.Add(l);
                            }
                            LinkLevelPicker levelPicker = new LinkLevelPicker(levels, "Расчет пересечений по выбранным уровням");
                            levelPicker.ShowDialog();
                            if (LinkLevelPicker.PickedLevelInstances != null)
                            {
                                if (LinkLevelPicker.PickedLevelInstances.Count != 0)
                                {
                                    try
                                    {
                                        UiController.CurrentController.LoopController.Prepare(link, LinkLevelPicker.PickedLevelInstances);
                                    }
                                    catch (Exception e)
                                    {
                                        PrintError(e);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Dialogs.ShowDialog("В проекте отсутствуют подгруженные связи", "Ошибка");
            }
        }
        private void OnBtnLoopDeny(object sender, RoutedEventArgs e)
        {
            UiController.CurrentController.LoopController.Reject();
        }
        private void OnBtnLoopApply(object sender, RoutedEventArgs e)
        {
            UiController.CurrentController.LoopController.Apply();
        }
        private void OnBtnLoopNext(object sender, RoutedEventArgs e)
        {
            UiController.CurrentController.LoopController.Next();
        }
        private void OnBtnLoopSkip(object sender, RoutedEventArgs e)
        {
            UiController.CurrentController.LoopController.Skip();
        }
        private void OnBtnPlaceOnSelected(object sender, RoutedEventArgs e)
        {
            if (UserPreferences.Department == ExtensibleOpeningManager.Common.Collections.Department.MEP)
            {
                ModuleData.CommandQueue.Enqueue(new CommandPlaceTaskOnPickedWall());
            }
            if (UserPreferences.Department == ExtensibleOpeningManager.Common.Collections.Department.AR)
            {
                ModuleData.CommandQueue.Enqueue(new CommandPlaceOpeningByTaskOnPickedWall());
            }
            if (UserPreferences.Department == ExtensibleOpeningManager.Common.Collections.Department.KR)
            {
                ModuleData.CommandQueue.Enqueue(new CommandPlaceOpeningByTaskOnPickedWall());
            }
        }

        private void OnBtnAddComment(object sender, RoutedEventArgs e)
        {
            if (tbMessage.Text != "")
            {
                if (UiController.CurrentController.Selection[0].Status == Collections.Status.Null)
                {
                    Dialogs.ShowDialog("Сперва необходимо назначить мониторинг!", "Предупреждение");
                    tbMessage.Text = "";
                    return;
                }
                ModuleData.CommandQueue.Enqueue(new CommandAddComment(UiController.CurrentController.Selection[0], this.tbMessage.Text));
                tbMessage.Text = "";
            }
        }

        private void OnSubDepartmentChanged(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                switch (cbxDepartment.SelectedIndex)
                {
                    case 0:
                        UserPreferences.SubDepartment = "ОВ";
                        break;
                    case 1:
                        UserPreferences.SubDepartment = "ВК";
                        break;
                    case 2:
                        UserPreferences.SubDepartment = "СС";
                        break;
                    case 3:
                        UserPreferences.SubDepartment = "ЭОМ";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) { PrintError(e); }
        }

        private void OnSubItemRemoveBtnClick(object sender, RoutedEventArgs e)
        {
            MonitorSubElement monitorElement = (sender as Button).DataContext as MonitorSubElement;
            object storedObject = monitorElement.Object;
            if (monitorElement.Type == Collections.MonitorSubElementType.Element)
            {
                if (storedObject.GetType() == typeof(SE_LocalElement) || storedObject.GetType() == typeof(SE_LinkedElement) || storedObject.GetType() == typeof(SE_LinkedInstance))
                {
                    ExtensibleSubElement subElement = storedObject as ExtensibleSubElement;
                    ModuleData.CommandQueue.Enqueue(new CommandRemoveSubElement(subElement.Parent.Instance, subElement));
                }
            }
            if (monitorElement.Type == Collections.MonitorSubElementType.Wall)
            {
                ModuleData.CommandQueue.Enqueue(new CommandSetWall(monitorElement.Parent.Element));
            }          
        }

        private void OnBtnPlaceOnSelectedTask(object sender, RoutedEventArgs e)
        {
            ModuleData.CommandQueue.Enqueue(new CommandPlaceOpeningByTask());
        }

        private void OnSubItemAddRemarkBtnClick(object sender, RoutedEventArgs e)
        {
            object storedObject = ((sender as Button).DataContext as MonitorSubElement).Object;
            if (storedObject.GetType() == typeof(SE_LinkedInstance))
            {
                ExtensibleSubElement subElement = storedObject as ExtensibleSubElement;
                ModuleData.CommandQueue.Enqueue(new CommandAddRemark(subElement.Parent, subElement as SE_LinkedInstance));
            }
        }

        private void OnBtnFindSubelements(object sender, RoutedEventArgs args)
        {
            List<RevitLinkInstance> links = CollectorTools.GetRevitLinks(UiController.CurrentController.Document);
            if (links.Count != 0)
            {
                RevitLinkPicker linkPicker = new RevitLinkPicker(links, "Поиск привязок", false);
                linkPicker.ShowDialog();
                if (RevitLinkPicker.PickedRevitLinkInstances != null)
                {
                    if (RevitLinkPicker.PickedRevitLinkInstances.Count != 0)
                    {
                        try
                        {
                            foreach (ExtensibleElement el in UiController.CurrentController.Selection)
                            {
                                ModuleData.CommandQueue.Enqueue(new CommandTryAutoLink(el, RevitLinkPicker.PickedRevitLinkInstances));
                            }
                        }
                        catch (Exception e)
                        {
                            PrintError(e);
                        }
                    }
                }
            }
            else
            {
                Dialogs.ShowDialog("В проекте отсутствуют подгруженные связи", "Ошибка");
            }
        }
    }
}
