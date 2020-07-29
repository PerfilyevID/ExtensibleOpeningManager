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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtensibleOpeningManager.Forms
{
    /// <summary>
    /// Логика взаимодействия для Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        public Preferences()
        {
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
                this.cbxOffsett.Text = Math.Round(UserPreferences.DefaultOffset).ToString();
            }
            try
            {
                Double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
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
                Double d = Double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
                UserPreferences.MinWallWidth = d;
            }
            catch (Exception) { }
            if (UserPreferences.Department == Common.Collections.Department.MEP)
            {
                UserPreferences.SubDepartment = this.cbxSubDepartment.Text;
            }
            UserPreferences.DefaultOffset = double.Parse(this.cbxOffsett.Text, System.Globalization.NumberStyles.Integer);
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
            int s = this.tbxMinWidth.SelectionStart;
            int l = this.tbxMinWidth.SelectionLength;
            int x = 0;
            string text = this.tbxMinWidth.Text;
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
            
            if (ntext != this.tbxMinWidth.Text)
            {
                this.tbxMinWidth.Text = ntext;
                try
                {
                    this.tbxMinWidth.SelectionStart = s - x;
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
            bool NoneError = false;
            bool BothUnchecked = false;
            try
            {
                Double.Parse(this.tbxMinWidth.Text, System.Globalization.NumberStyles.Float);
            }
            catch (Exception)
            {
                ParseError = true;
            }
            if (this.tbxMinWidth.Text == "")
            {
                NoneError = true;
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
            if (ParseError || NoneError || BothUnchecked)
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
