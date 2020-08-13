using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using ExtensibleOpeningManager.Commands;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Common.MonitorElements;
using ExtensibleOpeningManager.Forms;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Controll
{
    public class UiController
    {
        public readonly Guid GUID = Guid.NewGuid();
        public static readonly List<UiController> Controllers = new List<UiController>();
        public static UiController CurrentController { get; set; }
        public List<ExtensibleElement> Elements = new List<ExtensibleElement>();
        public List<ExtensibleElement> Selection = new List<ExtensibleElement>();
        public List<ExtensibleMessage> Comments = new List<ExtensibleMessage>();
        public List<RevitLinkInstance> Links = new List<RevitLinkInstance>();
        public List<ExtensibleSubElement> UpperElements = new List<ExtensibleSubElement>();
        public List<int> LinksIds = new List<int>();
        public MonitorCollection MonitorCollection { get; set; }
        public string SelectionSet = "";
        public string AllSelectionSet = "";
        public void UpdateUpperElements()
        {
            try
            {
                UpperElements = CollectorTools.GetUpperInstances(Document);
            }
            catch (Exception e)
            {
                PrintError(e);
                UpperElements = new List<ExtensibleSubElement>();
            }
        }
        public UiController(Document document)
        {
            Document = document;
            MonitorCollection = new MonitorCollection();
            Elements = CollectorTools.GetInstances(document);
            foreach (ExtensibleElement el in Elements)
            {
                try
                {
                    MonitorCollection.AddElement(el);
                }
                catch (Exception e) { PrintError(e); }
            }
            Document.DocumentClosing += new EventHandler<DocumentClosingEventArgs>(OnDocumentClose);
            LoopController = new LoopController(Document);
            UpdateUpperElements();
            ModuleData.CommandQueue.Enqueue(new CommandUpdateAllElements(this));
        }
        public static UiController GetControllerByDocument(Document doc)
        {
            foreach (UiController controller in Controllers)
            {
                if (doc.IsWorkshared && controller.Document.IsWorkshared)
                {
                    if (doc.GetWorksharingCentralModelPath().CentralServerPath == controller.Document.GetWorksharingCentralModelPath().CentralServerPath)
                    {
                        return controller;
                    }
                }
                if (!doc.IsWorkshared && !controller.Document.IsWorkshared)
                {
                    if (doc.PathName == controller.Document.PathName)
                    {
                        return controller;
                    }
                }
            }
            UiController newController = new UiController(doc);
            Controllers.Add(newController);
            return newController;
        }
        public Document Document { get; }
        public LoopController LoopController { get; set; }
        public ExtensibleElement GetElementOfSubElement(ElementId id)
        {
            foreach (ExtensibleElement el in Elements)
            {
                try
                {
                    foreach (ExtensibleSubElement sel in el.SubElements)
                    {
                        try
                        {
                            if (sel.GetType() == typeof(SE_LocalElement) && sel.Id == id.IntegerValue)
                            {
                                return el;
                            }
                        }
                        catch (Exception e) { PrintError(e); }
                    }
                }
                catch (Exception e) { PrintError(e); }
            }
            return null;
        }
        public void OnDocumentClose(object sender, DocumentClosingEventArgs args)
        {
            if (!args.IsCancelled())
            {
                CurrentController = null;
                Controllers.Remove(this);
            }
        }
        public void UpdateAllElements()
        {
            try
            {
                List<ExtensibleElement> newElementCollection = new List<ExtensibleElement>();
                foreach (ExtensibleElement el in Elements)
                {
                    ExtensibleElement elem = ExtensibleElement.GetExtensibleElementByInstance(el.Instance);
                    newElementCollection.Add(elem);
                }
                Elements = newElementCollection;
                foreach (ExtensibleElement element in newElementCollection)
                {
                    MonitorCollection.UpdateElement(element);
                }
                UpdateEnability();
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }
        public bool ElementsInLinksCollection(ICollection<ElementId> elements)
        {
            foreach (ElementId id in elements)
            {
                foreach (int instance in LinksIds)
                {
                    if (instance == id.IntegerValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool ElementsInExtensibleCollection(ICollection<ElementId> elements)
        {
            foreach (ElementId id in elements)
            {
                foreach (ExtensibleElement el in Elements)
                {
                    try
                    {
                        foreach (ExtensibleSubElement sel in el.SubElements)
                        {
                            try
                            {
                                if (sel.GetType() == typeof(SE_LocalElement))
                                {
                                    if (sel.Id == id.IntegerValue)
                                    {
                                        return true;
                                    }
                                }
                            }
                            catch (Exception e) { PrintError(e); }
                        }
                    }
                    catch (Exception e) { PrintError(e); }
                    if (el.Id == id.IntegerValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void SetActiveDocument(Document doc)
        {
            UiController controller = GetControllerByDocument(doc);
            if (CurrentController == null)
            {
                CurrentController = controller;
                controller.SelectionChanged();
                CurrentController.UpdateMonitor();
                return;
            }
            if (!controller.GUID.Equals(CurrentController.GUID))
            {
                CurrentController = controller;
                controller.SelectionChanged();
                CurrentController.UpdateMonitor();
            }
        }
        public bool IntersectionExist(Intersection intersection)
        {
            foreach (ExtensibleElement element in Elements)
            {
                try
                {
                    foreach (ExtensibleSubElement s in element.SubElements)
                    {
                        try
                        {
                            if (s.GetType() == typeof(SE_LocalElement))
                            {
                                if (s.Id == intersection.Element.Id.IntegerValue)
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception e) { PrintError(e); }
                    }
                }
                catch (Exception e) { PrintError(e); }
            }
            return false;
        }
        public bool IntersectionExist(Intersection intersection, SE_LinkedWall wall)
        {
            foreach (ExtensibleElement element in Elements)
            {
                try
                {
                    if (element.Wall.Wall.Id.IntegerValue == wall.Wall.Id.IntegerValue)
                    {
                        foreach (ExtensibleSubElement s in element.SubElements)
                        {
                            try
                            {
                                if (s.GetType() == typeof(SE_LocalElement))
                                {
                                    if (s.Id == intersection.Element.Id.IntegerValue)
                                    {
                                        return true;
                                    }
                                }
                            }
                            catch (Exception e) { PrintError(e); }
                        }
                    }
                }
                catch (Exception e) { PrintError(e); }
            }
            return false;
        }
        public bool OpeningExist(ExtensibleSubElement subElement, SE_LinkedWall wall)
        {
            foreach (ExtensibleElement element in Elements)
            {
                try
                {
                    if (element.Wall.Wall.Id.IntegerValue == wall.Wall.Id.IntegerValue)
                    {
                        foreach (ExtensibleSubElement s in element.SubElements)
                        {
                            try
                            {
                                if (s.GetType() == typeof(SE_LinkedInstance))
                                {
                                    if (s.Id == subElement.Id)
                                    {
                                        return true;
                                    }
                                }
                            }
                            catch (Exception e) { PrintError(e); }
                        }
                    }
                }
                catch (Exception) { }
            }
            return false;
        }
        public void SetSelection(ICollection<ElementId> elements)
        {
            try
            {
                List<string> allSelectionSet = new List<string>();
                foreach (ElementId id in elements)
                {
                    allSelectionSet.Add(id.ToString());
                }
                allSelectionSet.Sort();
                if (AllSelectionSet == string.Join(Variables.separator_element, allSelectionSet))
                {
                    return;
                }
                else
                {
                    AllSelectionSet = string.Join(Variables.separator_element, allSelectionSet);
                }
            }
            catch (Exception e) { PrintError(e); }
            List<string> newSelectionSetCollection = new List<string>();
            List<ExtensibleElement> newSelection = new List<ExtensibleElement>();
            foreach (ElementId id in elements)
            {
                try
                {
                    Element element = Document.GetElement(id);
                    if (element.GetType() == typeof(FamilyInstance))
                    {
                        FamilyInstance instance = element as FamilyInstance;
                        if (UserPreferences.Department == Collections.Department.AR && (instance.Symbol.FamilyName == Variables.family_ar_round || instance.Symbol.FamilyName == Variables.family_ar_square))
                        {
                            newSelectionSetCollection.Add(instance.Id.ToString());
                            newSelection.Add(ExtensibleElement.GetExtensibleElementByInstance(instance));
                            continue;
                        }
                        if (UserPreferences.Department == Collections.Department.KR && (instance.Symbol.FamilyName == Variables.family_kr_round || instance.Symbol.FamilyName == Variables.family_kr_square))
                        {
                            newSelectionSetCollection.Add(instance.Id.ToString());
                            newSelection.Add(ExtensibleElement.GetExtensibleElementByInstance(instance));
                            continue;
                        }
                        if (UserPreferences.Department == Collections.Department.MEP && (instance.Symbol.FamilyName == Variables.family_mep_round || instance.Symbol.FamilyName == Variables.family_mep_square))
                        {
                            newSelectionSetCollection.Add(instance.Id.ToString());
                            newSelection.Add(ExtensibleElement.GetExtensibleElementByInstance(instance));
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    PrintError(e);
                }
            }
            newSelectionSetCollection.Sort();
            if (SelectionSet != string.Join(Variables.separator_element, newSelectionSetCollection))
            {
                SelectionSet = string.Join(Variables.separator_element, newSelectionSetCollection);
                Selection = newSelection;
                SelectionChanged();
            }
        }
        private void SelectionChanged()
        {
            try
            {
                if (Selection.Count == 0)
                {
                    UpdateSubMonitor();
                    DockablePreferences.Page.tabMain.SelectedIndex = 0;
                    ClearComments();
                }
                if (Selection.Count == 1)
                {
                    UpdateSubMonitor();
                    UpdateEnability();
                    DockablePreferences.Page.tabMain.SelectedIndex = 1;
                    UpdateComments(Selection[0].AllComments);
                }
                if (Selection.Count > 1)
                {
                    UpdateSubMonitor();
                    UpdateEnability();
                    DockablePreferences.Page.tabMain.SelectedIndex = 1;
                    ClearComments();
                }
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }
        public void UpdateRevitLinkInstances()
        {
            Links = CollectorTools.GetRevitLinks(Document);
            LinksIds.Clear();
            foreach (RevitLinkInstance instance in Links)
            {
                LinksIds.Add(instance.Id.IntegerValue);
            }
        }
        public void UpdateComments(List<ExtensibleMessage> comments)
        {
            Comments = comments;
            DockablePreferences.Page.chatPanel.Children.Clear();
            foreach (ExtensibleMessage comment in comments)
            {
                DockablePreferences.Page.chatPanel.Children.Add(comment.GetUiElement());
            }
            DockablePreferences.Page.btnChatMessage.IsEnabled = true;
        }
        public void ClearComments()
        {
            Comments.Clear();
            DockablePreferences.Page.chatPanel.Children.Clear();
            DockablePreferences.Page.btnChatMessage.IsEnabled = false;
        }
        public void UpdateMonitor()
        {
            DockablePreferences.Page.monitorView.ItemsSource = MonitorCollection.Collection;
            DockablePreferences.Page.UpdateItemscontroll();
        }
        public MonitorElement GetMonitorElement(ExtensibleElement element)
        {
            foreach (MonitorGroup group in MonitorCollection.Collection)
            {
                if (group.Status == element.VisibleStatus)
                {
                    foreach (MonitorElement el in group.Collection)
                    {
                        if (el.Id == element.Id)
                        {
                            return el;
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateSubMonitor()
        {
            if (Selection.Count == 1)
            {
                DockablePreferences.Page.SubItemsControll.ItemsSource = null;
                try
                {
                    DockablePreferences.Page.SubItemsControll.ItemsSource = GetMonitorElement(Selection[0]).Collection;
                }
                catch (Exception) { }
            }
            else
            {
                DockablePreferences.Page.SubItemsControll.ItemsSource = null;
            }
        }
        public void UpdateEnability()
        {
            try
            {
                switch (UserPreferences.Department)
                {
                    case Collections.Department.AR:
                        DockablePreferences.Page.cbxDepartment.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.lblDepartmentHeader.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.sepDep.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceAR.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceKR.Visibility = System.Windows.Visibility.Collapsed;//collapsed until loop is enabled
                        DockablePreferences.Page.btnPlaceMEP.Visibility = System.Windows.Visibility.Collapsed;//collapsed until loop is enabled
                        DockablePreferences.Page.sep.Visibility = System.Windows.Visibility.Collapsed;//collapsed until loop is enabled
                        DockablePreferences.Page.btnPlaceOnSelectedTask.Visibility = System.Windows.Visibility.Visible;
                        DockablePreferences.Page.btnPlaceOnSelectedTask2.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case Collections.Department.KR:
                        DockablePreferences.Page.cbxDepartment.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.lblDepartmentHeader.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.sepDep.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceAR.Visibility = System.Windows.Visibility.Collapsed;//collapsed until loop is enabled
                        DockablePreferences.Page.btnPlaceKR.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceMEP.Visibility = System.Windows.Visibility.Collapsed;//collapsed until loop is enabled
                        DockablePreferences.Page.sep.Visibility = System.Windows.Visibility.Collapsed;//collapsed until loop is enabled
                        DockablePreferences.Page.btnPlaceOnSelectedTask.Visibility = System.Windows.Visibility.Visible;
                        DockablePreferences.Page.btnPlaceOnSelectedTask2.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case Collections.Department.MEP:
                        LoopController.UpdateButtonToolTips();
                        DockablePreferences.Page.cbxDepartment.Visibility = System.Windows.Visibility.Visible;
                        DockablePreferences.Page.cbxDepartment.Text = UserPreferences.SubDepartment;
                        DockablePreferences.Page.lblDepartmentHeader.Visibility = System.Windows.Visibility.Visible;
                        DockablePreferences.Page.sepDep.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceAR.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceKR.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceMEP.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.sep.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceOnSelectedTask.Visibility = System.Windows.Visibility.Collapsed;
                        DockablePreferences.Page.btnPlaceOnSelectedTask2.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
                if (UserPreferences.Department == Collections.Department.MEP)
                {
                    DockablePreferences.Page.btnLoop.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoop2.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopApply.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopDeny.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopNext.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopSkip.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopApply2.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopDeny2.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopNext2.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.btnLoopSkip2.Visibility = System.Windows.Visibility.Visible;
                    DockablePreferences.Page.sep.Visibility = System.Windows.Visibility.Visible;
                    if (LoopController.IsActive)
                    {
                        DockablePreferences.Page.btnLoop.IsEnabled = false;
                        DockablePreferences.Page.btnLoop2.IsEnabled = false;
                        if (LoopController.CreatedElements.Count != 0)
                        {
                            DockablePreferences.Page.btnLoopApply.IsEnabled = true;
                            DockablePreferences.Page.btnLoopDeny.IsEnabled = true;
                            DockablePreferences.Page.btnLoopSkip.IsEnabled = true;
                            DockablePreferences.Page.btnLoopApply2.IsEnabled = true;
                            DockablePreferences.Page.btnLoopDeny2.IsEnabled = true;
                            DockablePreferences.Page.btnLoopSkip2.IsEnabled = true;
                        }
                        else
                        {
                            DockablePreferences.Page.btnLoopApply.IsEnabled = false;
                            DockablePreferences.Page.btnLoopDeny.IsEnabled = false;
                            DockablePreferences.Page.btnLoopSkip.IsEnabled = false;
                            DockablePreferences.Page.btnLoopApply2.IsEnabled = false;
                            DockablePreferences.Page.btnLoopDeny2.IsEnabled = false;
                            DockablePreferences.Page.btnLoopSkip2.IsEnabled = false;
                        }
                        DockablePreferences.Page.btnLoopNext.IsEnabled = true;
                        DockablePreferences.Page.btnLoopNext2.IsEnabled = true;
                    }
                    else
                    {
                        DockablePreferences.Page.btnLoop.IsEnabled = true;
                        DockablePreferences.Page.btnLoop2.IsEnabled = true;
                        DockablePreferences.Page.btnLoopApply.IsEnabled = false;
                        DockablePreferences.Page.btnLoopDeny.IsEnabled = false;
                        DockablePreferences.Page.btnLoopNext.IsEnabled = false;
                        DockablePreferences.Page.btnLoopSkip.IsEnabled = false;
                        DockablePreferences.Page.btnLoopApply2.IsEnabled = false;
                        DockablePreferences.Page.btnLoopDeny2.IsEnabled = false;
                        DockablePreferences.Page.btnLoopNext2.IsEnabled = false;
                        DockablePreferences.Page.btnLoopSkip2.IsEnabled = false;
                    }
                }
                else
                {
                    DockablePreferences.Page.btnLoop.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoop2.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopApply.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopDeny.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopNext.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopSkip.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopApply2.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopDeny2.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopNext2.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.btnLoopSkip2.Visibility = System.Windows.Visibility.Collapsed;
                    DockablePreferences.Page.sep.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (Selection.Count == 0) { return; }
                UpdateSubMonitor();
                HashSet<int> wallIds = new HashSet<int>();
                bool isNullSelection = Selection.Count == 0;
                bool isSingleSelection = Selection.Count == 1;
                bool isNotApproved = false;
                bool isNotRejected = false;
                bool isByOpening = false;
                bool isNotCommitedWall = false;
                if (Selection.Count != 0)
                {
                    isNotApproved = Selection[0].Status != Collections.Status.Applied;
                    isNotRejected = Selection[0].Status != Collections.Status.Rejected;
                    isNotCommitedWall = Selection[0].WallStatus == Collections.WallStatus.NotCommited;
                }
                bool isNotCommitedSubElements = false;
                bool isMultipleSubelements = false;
                bool isChanged = false;
                bool isRoundInSelection = false;
                bool isNotFoundElementsInElement = false;
                foreach (ExtensibleElement el in Selection)
                {
                    foreach (ExtensibleSubElement subelement in el.SubElements)
                    {
                        try
                        {
                            if (subelement.GetType() == typeof(SE_LinkedInstance))
                            {
                                string familyName = (subelement.Element as FamilyInstance).Symbol.FamilyName;
                                if (familyName == Variables.family_ar_round ||
                                    familyName == Variables.family_ar_square ||
                                    familyName == Variables.family_kr_round ||
                                    familyName == Variables.family_kr_square)
                                {
                                    isByOpening = true;
                                }
                            }
                        }
                        catch (Exception) { }
                    }
                    
                    if (el.HasUnfoundSubElements() || el.WallStatus == Collections.WallStatus.NotFound)
                    {
                        isNotFoundElementsInElement = true;
                    }
                    if (el.Instance != null)
                    {
                        if (el.Instance.Symbol.FamilyName == Variables.family_ar_round ||
                            el.Instance.Symbol.FamilyName == Variables.family_kr_round ||
                            el.Instance.Symbol.FamilyName == Variables.family_mep_round)
                        {
                            isRoundInSelection = true;
                        }
                    }
                    string[] parts_saved = el.SavedData.Split(new string[] { Variables.separator_element }, StringSplitOptions.None);
                    string[] parts_current = el.ToString().Split(new string[] { Variables.separator_element }, StringSplitOptions.None);
                    if (parts_saved[0] != parts_current[0] ||
                        parts_saved[2] != parts_current[2] ||
                        parts_saved[3] != parts_current[3] ||
                        parts_saved[4] != parts_current[4] ||
                        parts_saved[5] != parts_current[5] ||
                        parts_saved[6] != parts_current[6] ||
                        parts_saved[7] != parts_current[7] ||
                        parts_saved[8] != parts_current[8] ||
                        parts_saved[9] != parts_current[9] ||
                        parts_saved[10] != parts_current[10])
                    {
                        isChanged = true;
                    }
                    if (el.Wall != null)
                    {
                        if (el.WallStatus != Collections.WallStatus.NotFound)
                        {
                            wallIds.Add(el.Wall.Wall.Id.IntegerValue);
                        }
                    }
                    if (el.SubElements.Count > 1)
                    {
                        isMultipleSubelements = true;
                    }
                    if (el.HasUncommitedSubElements()) { isNotCommitedSubElements = true; }
                }
                bool isWallExist = Selection[0].WallStatus != Collections.WallStatus.NotFound;
                bool isSelectionOfSingleWall = wallIds.Count == 1;

                bool isAbleToUpdate = false;
                if (isSingleSelection && Selection.Count != 0)
                {
                    isAbleToUpdate = Selection[0].IsAbleToUpdate();
                }
                DockablePreferences.Page.btnAddSubElement.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnApplySubElements.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnApplyWall.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnApprove.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnGroup.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnReject.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnReset.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnSetOffset.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnSetWall.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnSwap.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnUngroup.Visibility = System.Windows.Visibility.Collapsed;
                DockablePreferences.Page.btnUpdate.Visibility = System.Windows.Visibility.Collapsed;
                if (isSingleSelection && !isByOpening)
                {
                    DockablePreferences.Page.btnAddSubElement.Visibility = System.Windows.Visibility.Visible;
                }
                if (isNotCommitedSubElements && isSingleSelection)
                {
                    DockablePreferences.Page.btnApplySubElements.Visibility = System.Windows.Visibility.Visible;
                }
                if (isNotCommitedWall && isWallExist)
                {
                    DockablePreferences.Page.btnApplyWall.Visibility = System.Windows.Visibility.Visible;
                }
                if (isNotApproved || isChanged)
                {
                    DockablePreferences.Page.btnApprove.Visibility = System.Windows.Visibility.Visible;
                }
                if (isSelectionOfSingleWall && !isRoundInSelection && !isSingleSelection && !isNotFoundElementsInElement && !isByOpening)
                {
                    DockablePreferences.Page.btnGroup.Visibility = System.Windows.Visibility.Visible;
                }
                if (isNotRejected && !isChanged)
                {
                    DockablePreferences.Page.btnReject.Visibility = System.Windows.Visibility.Visible;
                }
                if (isChanged && isSingleSelection && !isNotFoundElementsInElement)
                {
                    DockablePreferences.Page.btnReset.Visibility = System.Windows.Visibility.Visible;
                }
                DockablePreferences.Page.btnSetOffset.Visibility = System.Windows.Visibility.Visible;
                if (!isWallExist && isSingleSelection)
                {
                    DockablePreferences.Page.btnSetWall.Visibility = System.Windows.Visibility.Visible;
                }
                if (isSingleSelection && !isMultipleSubelements && UserPreferences.Department == Collections.Department.MEP)
                {
                    DockablePreferences.Page.btnSwap.Visibility = System.Windows.Visibility.Visible;
                }
                if (isMultipleSubelements && isSingleSelection && !isNotFoundElementsInElement)
                {
                    DockablePreferences.Page.btnUngroup.Visibility = System.Windows.Visibility.Visible;
                }
                if (isAbleToUpdate && isSingleSelection && !isNotFoundElementsInElement)
                {
                    DockablePreferences.Page.btnUpdate.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (Exception) { }
        }
        public void OnManualElementChanged(int id)
        {
            try
            {
                ICollection<ElementId> ids = new List<ElementId>() { new ElementId(id) };
                OnElementsChanged(ids);
                UpdateEnability();
            }
            catch (System.Exception) { }
        }
        public void OnManualElementChanged(Element element)
        {
            try
            {
                ICollection<ElementId> ids = new List<ElementId>() { element.Id };
                OnElementsChanged(ids);
                UpdateEnability();
            }
            catch (System.Exception) { }
        }
        public void OnManualElementChanged(ElementId id)
        {
            try
            {
                ICollection<ElementId> ids = new List<ElementId>() { id };
                OnElementsChanged(ids);
                UpdateEnability();
            }
            catch (System.Exception) { }
        }
        public void OnElementsCreated(ICollection<ElementId> elements)
        {
            List<ExtensibleElement> newSelection = new List<ExtensibleElement>();
            foreach (ElementId id in elements)
            {
                Element element = Document.GetElement(id);
                if (element.GetType() == typeof(FamilyInstance))
                {
                    FamilyInstance instance = element as FamilyInstance;
                    if (UserPreferences.Department == Collections.Department.AR && (instance.Symbol.FamilyName == Variables.family_ar_round || instance.Symbol.FamilyName == Variables.family_ar_square))
                    {
                        newSelection.Add(ExtensibleElement.GetExtensibleElementByInstance(instance));
                    }
                    if (UserPreferences.Department == Collections.Department.KR && (instance.Symbol.FamilyName == Variables.family_kr_round || instance.Symbol.FamilyName == Variables.family_kr_square))
                    {
                        newSelection.Add(ExtensibleElement.GetExtensibleElementByInstance(instance));
                    }
                    if (UserPreferences.Department == Collections.Department.MEP && (instance.Symbol.FamilyName == Variables.family_mep_round || instance.Symbol.FamilyName == Variables.family_mep_square))
                    {
                        newSelection.Add(ExtensibleElement.GetExtensibleElementByInstance(instance));
                    }
                }
            }
            foreach (ExtensibleElement element in newSelection)
            {
                Elements.Add(element);
                MonitorCollection.AddElement(element);
            }

        }
        public void OnElementsChanged(ICollection<ElementId> elements)
        {
            try
            {
                List<ExtensibleElement> newElementCollection = new List<ExtensibleElement>();
                List<ExtensibleElement> updatedElements = new List<ExtensibleElement>();
                foreach (ExtensibleElement el in Elements)
                {
                    if (!InList(elements, el.Id))
                    {
                        newElementCollection.Add(el);
                        foreach (ExtensibleSubElement sel in el.SubElements)
                        {
                            if (sel.GetType() == typeof(SE_LocalElement))
                            {
                                if (InList(elements, sel.Id))
                                {
                                    updatedElements.Add(ExtensibleElement.GetExtensibleElementByInstance(el.Instance));
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ExtensibleElement elem = ExtensibleElement.GetExtensibleElementByInstance(el.Instance);
                        newElementCollection.Add(elem);
                        updatedElements.Add(elem);
                    }
                }
                foreach (ElementId id in elements)
                {
                    ExtensibleElement el = GetElementOfSubElement(id);
                    if (el != null)
                    {
                        updatedElements.Add(ExtensibleElement.GetExtensibleElementByInstance(el.Instance));
                    }
                }
                Elements = newElementCollection;
                foreach (ExtensibleElement element in updatedElements)
                {
                    MonitorCollection.UpdateElement(element);
                }
                UpdateEnability();
            }
            catch (Exception e)
            {
                PrintError(e);
            }

        }
        private bool InList(ICollection<ElementId> list, int id)
        {
            foreach (ElementId i in list)
            {
                if (i.IntegerValue == id) { return true; }
            }
            return false;
        }
        public void OnElementsRemoved(ICollection<ElementId> elements)
        {
            List<ExtensibleElement> newSelection = new List<ExtensibleElement>();
            List<ExtensibleElement> updatedElements = new List<ExtensibleElement>();
            foreach (ExtensibleElement el in Elements)
            {
                if (!InList(elements, el.Id))
                {
                    newSelection.Add(el);
                    foreach (ExtensibleSubElement sel in el.SubElements)
                    {
                        if (sel.GetType() == typeof(SE_LocalElement))
                        {
                            if (InList(elements, sel.Id))
                            {
                                updatedElements.Add(el);
                                break;
                            }
                        }
                    }
                }
            }
            Elements = newSelection;
            foreach (ExtensibleElement element in updatedElements)
            {
                MonitorCollection.UpdateElement(element);
            }
            foreach (ElementId element in elements)
            {
                MonitorCollection.RemoveId(element);
            }
        }
    }
}
