using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace ExtensibleOpeningManager.Forms
{
    /// <summary>
    /// Логика взаимодействия для Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        public Preferences()
        {
#if Revit2020_AR || Revit2020_KR || Revit2020_MEP
            Owner = ModuleData.RevitWindow;
#endif
#if Revit2018_AR || Revit2018_KR || Revit2018_MEP
            
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = ModuleData.MainWindowHandle;
#endif
            InitializeComponent();
            if (UserPreferences.Department == Common.Collections.Department.MEP)
            {
                this.cbxDepartment.Text = "MEP";
                this.cbxDepartment.IsEnabled = false;
                this.cbxSubDepartment.Text = UserPreferences.SubDepartment;
                this.chbxArchitecture.IsChecked = UserPreferences.PlaceOnArchitecturalWalls;
                this.chbxConcrete.IsChecked = UserPreferences.PlaceOnStructuralWalls;
                this.infoBuild.Text = ModuleData.Build;
                this.infoDate.Text = ModuleData.Date;
                this.infoVer.Text = ModuleData.Version;
                this.tbxMinWidth.Text = (Math.Round(UserPreferences.MinWallWidth, 2)).ToString();
                this.tbxMinOpeningHeight.Text = (Math.Round(UserPreferences.MinInstanceHeight, 2)).ToString();
                this.tbxMinOpeningWidth.Text = (Math.Round(UserPreferences.MinInstanceWidth, 2)).ToString();
                this.cbxOffsett.Text = Math.Round(UserPreferences.DefaultOffset).ToString();
            }
            if (UserPreferences.Department == Common.Collections.Department.AR)
            {
                this.cbxDepartment.Text = "AR";
                this.cbxDepartment.IsEnabled = false;
                this.cbxSubDepartment.IsEnabled = false;
                this.chbxArchitecture.IsChecked = UserPreferences.PlaceOnArchitecturalWalls;
                this.chbxConcrete.IsChecked = UserPreferences.PlaceOnStructuralWalls;
                this.infoBuild.Text = ModuleData.Build;
                this.infoDate.Text = ModuleData.Date;
                this.infoVer.Text = ModuleData.Version;
                this.tbxMinWidth.Text = (Math.Round(UserPreferences.MinWallWidth, 2)).ToString();
                this.tbxMinOpeningHeight.Text = (Math.Round(UserPreferences.MinInstanceHeight, 2)).ToString();
                this.tbxMinOpeningWidth.Text = (Math.Round(UserPreferences.MinInstanceWidth, 2)).ToString();
                this.cbxOffsett.Text = Math.Round(UserPreferences.DefaultOffset).ToString();
            }
            if (UserPreferences.Department == Common.Collections.Department.KR)
            {
                this.cbxDepartment.Text = "KR";
                this.cbxDepartment.IsEnabled = false;
                this.cbxSubDepartment.IsEnabled = false;
                this.chbxArchitecture.IsChecked = UserPreferences.PlaceOnArchitecturalWalls;
                this.chbxConcrete.IsChecked = UserPreferences.PlaceOnStructuralWalls;
                this.infoBuild.Text = ModuleData.Build;
                this.infoDate.Text = ModuleData.Date;
                this.infoVer.Text = ModuleData.Version;
                this.tbxMinWidth.Text = (Math.Round(UserPreferences.MinWallWidth, 2)).ToString();
                this.tbxMinOpeningHeight.Text = (Math.Round(UserPreferences.MinInstanceHeight, 2)).ToString();
                this.tbxMinOpeningWidth.Text = (Math.Round(UserPreferences.MinInstanceWidth, 2)).ToString();
                this.cbxOffsett.Text = Math.Round(UserPreferences.DefaultOffset).ToString();
            }
            try
            {
                double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
                this.tbxMinWidthHeader.Foreground = Brushes.Black;
                if ((bool)this.chbxArchitecture.IsChecked || (bool)this.chbxConcrete.IsChecked)
                {
                    this.btnApply.IsEnabled = true;
                    this.chbxArchitectureHeader.Foreground = Brushes.Black;
                    this.chbxConcreteHeader.Foreground = Brushes.Black;
                }
                else
                {
                    this.btnApply.IsEnabled = false;
                    this.chbxArchitectureHeader.Foreground = Brushes.Red;
                    this.chbxConcreteHeader.Foreground = Brushes.Red;
                }
            }
            catch (Exception)
            {
                try
                {
                    this.tbxMinWidthHeader.Foreground = Brushes.Red;
                    this.btnApply.IsEnabled = false;
                }
                catch (Exception) { }
            }
        }

        private void OnBtnApplyClick(object sender, RoutedEventArgs e)
        {

            UserPreferences.PlaceOnArchitecturalWalls = (bool)this.chbxArchitecture.IsChecked;
            UserPreferences.PlaceOnStructuralWalls = (bool)this.chbxConcrete.IsChecked;
            try
            {
                double d = double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
                double h = double.Parse(this.tbxMinOpeningHeight.Text, System.Globalization.NumberStyles.Float);
                double w = double.Parse(this.tbxMinOpeningWidth.Text, System.Globalization.NumberStyles.Float);
                UserPreferences.MinWallWidth = d;
                UserPreferences.MinInstanceHeight = h;
                UserPreferences.MinInstanceWidth = w;
            }
            catch (Exception) { }
            if (UserPreferences.Department == Common.Collections.Department.MEP)
            {
                UserPreferences.SubDepartment = this.cbxSubDepartment.Text;
            }
            UserPreferences.DefaultOffset = double.Parse(this.cbxOffsett.Text, System.Globalization.NumberStyles.Integer);
            UserPreferences.TrySaveParameters();
            this.Close();
        }

        private void OnBtnCloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnBtnManualClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(ModuleData.ManualPage);
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
            bool ParseError = false;
            bool ParseError2 = false;
            bool ParseError3 = false;
            bool NoneError = false;
            bool NoneError2 = false;
            bool NoneError3 = false;
            bool BothUnchecked = false;
            try
            {
                Double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError = true;
            }
            try
            {
                Double.Parse(this.tbxMinOpeningWidth.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError2 = true;
            }
            try
            {
                Double.Parse(this.tbxMinOpeningHeight.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError3 = true;
            }
            if (this.tbxMinWidth.Text == "")
            {
                NoneError = true;
            }
            if (this.tbxMinOpeningWidth.Text == "")
            {
                NoneError2 = true;
            }
            if (this.tbxMinOpeningHeight.Text == "")
            {
                NoneError3 = true;
            }
            if (!(bool)this.chbxArchitecture.IsChecked && !(bool)this.chbxConcrete.IsChecked)
            {
                BothUnchecked = true;
            }
            if (BothUnchecked)
            {
                this.chbxArchitectureHeader.Foreground = Brushes.Red;
                this.chbxConcreteHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.chbxArchitectureHeader.Foreground = Brushes.Black;
                this.chbxConcreteHeader.Foreground = Brushes.Black;
            }
            if (ParseError || NoneError)
            {
                this.tbxMinWidthHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.tbxMinWidthHeader.Foreground = Brushes.Black;
            }
            if (ParseError2 || NoneError2)
            {
                this.tbxMinOpeningWidthHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.tbxMinOpeningWidthHeader.Foreground = Brushes.Black;
            }
            if (ParseError3 || NoneError3)
            {
                this.tbxMinOpeningHeightHeader.Foreground = Brushes.Red;
            }
            else
            {
                this.tbxMinOpeningHeightHeader.Foreground = Brushes.Black;
            }
            if (ParseError || ParseError2 || ParseError3 || NoneError || NoneError2 || NoneError3 || BothUnchecked)
            {
                this.btnApply.IsEnabled = false;
            }
            else
            {
                this.btnApply.IsEnabled = true;
            }
        }
        private void OnChbxUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEnability();
            }
            catch (Exception)
            { }
        }

        private void OnChbxChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateEnability();
            }
            catch (Exception)
            { }
        }
    }
}
