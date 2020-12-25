using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Commands;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtensibleOpeningManager.Tools.Instances;
using ExtensibleOpeningManager.Common;

namespace ExtensibleOpeningManager.Forms
{
    /// <summary>
    /// Логика взаимодействия для UpdateByDocument.xaml
    /// </summary>
    public partial class UpdateByDocument : Window
    {
        private Document Doc { get; set; }
        public UpdateByDocument(Document doc, List<RevitLinkInstance> links)
        {
            Doc = doc;
#if Revit2020_AR || Revit2020_KR || Revit2020_MEP
            Owner = ModuleData.RevitWindow;
#endif
#if Revit2018_AR || Revit2018_KR || Revit2018_MEP

            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = ModuleData.MainWindowHandle;
#endif
            InitializeComponent();
            List<RLI_element> elements = new List<RLI_element>();
            foreach (RevitLinkInstance i in links)
            {
                elements.Add(new RLI_element(i));
            }
            LinkControll.DataContext = new WPFSource<RLI_element>(elements);
            try
            {
                UpdateEnability();
            }
            catch (Exception) { }
        }
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void OnHelp(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ModuleData.ManualPage);
        }
        private void OnOk(object sender, RoutedEventArgs e)
        {
            List<SE_LinkedWall> walls = new List<SE_LinkedWall>();
            List<RevitLinkInstance> revitLinkInstances = new List<RevitLinkInstance>();
            List<SE_LinkedInstance> linkedInstances = new List<SE_LinkedInstance>();
            List<ExtensibleElement> localElements = CollectorTools.GetInstances(Doc);
            //
            foreach (SE_LinkedWall wall in CollectorTools.GetWalls(Doc))
            {
                bool isConcrete = wall.Wall.Name.StartsWith("00_");
                if (wallsArchitecture.IsChecked == true && !isConcrete)
                {
                    walls.Add(wall);
                }
                if (wallsConcrete.IsChecked == true && isConcrete)
                {
                    walls.Add(wall);
                }
            }
            WPFSource<RLI_element> collection = LinkControll.DataContext as WPFSource<RLI_element>;
            foreach (RLI_element wpfElement in collection.Collection)
            {
                if (wpfElement.IsChecked && wpfElement.IsEnabled)
                {
                    revitLinkInstances.Add(wpfElement.Source as RevitLinkInstance);
                }
            }
            foreach (RevitLinkInstance link in revitLinkInstances)
            {
                foreach (SE_LinkedInstance instance in CollectorTools.GetMepSubInstances(link))
                {
                    linkedInstances.Add(instance);
                }
            }
            foreach (ExtensibleElement el in localElements)
            {
                if (chbxChangedUpdate.IsChecked == true)
                {
                    if (el.IsAbleToUpdate() && (el.HasUncommitedChangesInSubElements() || el.HasUncommitedSubElements()))
                    {
                        ModuleData.CommandQueue.Enqueue(new CommandUpdate(el));
                        if (chbxChangedReject.IsChecked == true)
                        {
                            ModuleData.CommandQueue.Enqueue(new CommandReject(el));
                        }
                        else
                        {
                            ModuleData.CommandQueue.Enqueue(new CommandApprove(el));
                        }
                    }
                }
                if (chbxChangedRemoveNotFound.IsChecked == true)
                {
                    bool changed = false;
                    foreach (ExtensibleSubElement suel in el.SubElements.ToList())
                    {
                        if (suel.Status == Collections.SubStatus.NotFound)
                        {
                            changed = true;
                            ModuleData.CommandQueue.Enqueue(new CommandRemoveSubElement(el.Instance, suel));
                        }
                    }
                    if (changed)
                    {
                        if (chbxChangedReject.IsChecked == true)
                        {
                            ModuleData.CommandQueue.Enqueue(new CommandReject(el));
                        }
                        else
                        {
                            ModuleData.CommandQueue.Enqueue(new CommandApprove(el));
                        }
                    }
                }
            }
            if (linkedInstances.Count != 0)
            {
                SizeOptions SizeOptions = new SizeOptions(double.Parse(tbxMinWidth.Text), double.Parse(tbxMinHeight.Text), double.Parse(tbxMaxWidth.Text), double.Parse(tbxMaxHeight.Text));
                if (chbxNewCreate.IsChecked == true)
                {
                    ModuleData.CommandQueue.Enqueue(new CommandCreateOpeningsByTasks_Loop(linkedInstances, (bool)wallsConcrete.IsChecked, (bool)wallsArchitecture.IsChecked, (bool)chbxNewGroup.IsChecked, (bool)chbxNewReject.IsChecked, SizeOptions));
                }
            }
            Close();
        }
        private void OnCheckedN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender == chbxNewReject)
                {
                    chbxNewApply.IsChecked = false;
                }
                if (sender == chbxNewApply)
                {
                    chbxNewReject.IsChecked = false;
                }
                if (sender == chbxChangedReject)
                {
                    chbxChangedApply.IsChecked = false;
                }
                if (sender == chbxChangedApply)
                {
                    chbxChangedReject.IsChecked = false;
                }
            }
            catch (Exception) { }
        }
        private void OnUncheckedN(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender == chbxNewReject)
                {
                    chbxNewApply.IsChecked = true;
                }
                if (sender == chbxNewApply)
                {
                    chbxNewReject.IsChecked = true;
                }
                if (sender == chbxChangedReject)
                {
                    chbxChangedApply.IsChecked = true;
                }
                if (sender == chbxChangedApply)
                {
                    chbxChangedReject.IsChecked = true;
                }
            }
            catch (Exception) { }

        }
        private void OnTbChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int s = tb.SelectionStart;
            int l = tb.SelectionLength;
            int x = 0;
            string text = tb.Text;
            string ntext = "";
            bool alreadyGotPoint = false;
            int signsAfterPoint = 0;
            foreach (char c in text)
            {
                if ("0123456789,.<>бю".Contains(c))
                {
                    if (c == '.' || c == '<' || c == '>' || c == 'б' || c == 'ю')
                    {
                        if (!alreadyGotPoint)
                        {
                            alreadyGotPoint = true;
                            ntext += ',';
                        }
                        else
                        {
                            x++;
                        }
                    }
                    else
                    {
                        if (c == ',')
                        {
                            if (!alreadyGotPoint)
                            {
                                alreadyGotPoint = true;
                                ntext += c;
                            }
                            else
                            {
                                x++;
                            }
                        }
                        else
                        {
                            if (alreadyGotPoint)
                            {
                                if (signsAfterPoint < 3)
                                {
                                    signsAfterPoint++;
                                    ntext += c;
                                }
                                else
                                {
                                    x++;
                                }

                            }
                            else
                            {
                                ntext += c;
                            }
                        }
                    }
                }
                else
                {
                    x++;
                }
            }

            if (ntext != tb.Text)
            {
                tb.Text = ntext;
                try
                {
                    tb.SelectionStart = s - x;
                }
                catch (Exception) { }
            }
            try
            {
                UpdateEnability();
            }
            catch (Exception) { }
        }
        private void UpdateEnability()
        {
            bool ParseError1 = false;
            bool ParseError2 = false;
            bool ParseError3 = false;
            bool ParseError4 = false;
            bool NoneError1 = false;
            bool NoneError2 = false;
            bool NoneError3 = false;
            bool NoneError4 = false;
            bool BothUnchecked = false;
            /*
            bool DocumentChecked = false;
            try
            {
                WPFSource<RLI_element> collection = LinkControll.DataContext as WPFSource<RLI_element>;
                foreach (RLI_element wpfElement in collection.Collection)
                {
                    if (wpfElement.IsChecked && wpfElement.IsEnabled)
                    {
                        DocumentChecked = true;
                        break;
                    }
                }
            }
            catch (Exception) { }
            */
            try
            {
                Double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError1 = true;
            }
            try
            {
                Double.Parse(this.tbxMinHeight.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError2 = true;
            }
            try
            {
                Double.Parse(this.tbxMaxWidth.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError3 = true;
            }
            try
            {
                Double.Parse(this.tbxMaxHeight.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError4 = true;
            }
            if (this.tbxMinWidth.Text == "")
            {
                NoneError1 = true;
            }
            if (this.tbxMinHeight.Text == "")
            {
                NoneError2 = true;
            }
            if (this.tbxMaxWidth.Text == "")
            {
                NoneError3 = true;
            }
            if (this.tbxMinWidth.Text == "")
            {
                NoneError4 = true;
            }
            if (!(bool)this.wallsArchitecture.IsChecked && !(bool)this.wallsConcrete.IsChecked)
            {
                BothUnchecked = true;
            }
            if (BothUnchecked)
            {
                this.wallsHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.wallsHeader.Foreground = Brushes.Black;
            }
            if (ParseError1 || NoneError1 || ParseError2 || NoneError2)
            {
                this.minHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.minHeader.Foreground = Brushes.Black;
            }

            if (ParseError3 || NoneError3 || ParseError4 || NoneError4)
            {
                this.maxHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.maxHeader.Foreground = Brushes.Black;
            }

            if (ParseError1 || ParseError2 || ParseError3 || ParseError4 || NoneError1 || NoneError2 || NoneError3 || NoneError4 || BothUnchecked)
            {
                this.btnOk.IsEnabled = false;
            }
            else
            {
                this.btnOk.IsEnabled = true;
            }
        }

        private void OnWallCheck(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEnability();
            }
            catch (Exception) { }
        }

        private void OnWallUncheck(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEnability();
            }
            catch (Exception) { }
        }

        private void OnDocUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEnability();
            }
            catch (Exception) { }
        }

        private void OnDocChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEnability();
            }
            catch (Exception) { }
        }
    }
}
