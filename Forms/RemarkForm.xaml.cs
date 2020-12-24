using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Forms
{
    /// <summary>
    /// Логика взаимодействия для RemarkForm.xaml
    /// </summary>
    public partial class RemarkForm : Window
    {
        public RemarkForm(RemarkType type)
        {
#if Revit2020_AR || Revit2020_KR || Revit2020_MEP
            Owner = ModuleData.RevitWindow;
#endif
#if Revit2018_AR || Revit2018_KR || Revit2018_MEP
            
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = ModuleData.MainWindowHandle;
#endif
            InitializeComponent();
            switch (type)
            {
                case RemarkType.Request:
                    Title = "Замечание: «Добавить»";
                    break;
                case RemarkType.Answer_Ok:
                    Title = "Замечание: «Утвердить»";
                    break;
                case RemarkType.Answer_No:
                    Title = "Замечание: «Отклонить»";
                    break;
                default:
                    break;
            }
        }

        private void OnApply(object sender, RoutedEventArgs e)
        {
            if (tbBody.Text != string.Empty && tbHeader.Text != string.Empty)
            {
                Dialogs.Body = this.tbBody.Text;
                Dialogs.Header = this.tbHeader.Text;
            }
            Close();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbBody.Text != string.Empty && tbHeader.Text != string.Empty)
            {
                btnApply.IsEnabled = true;
            }
            else
            {
                btnApply.IsEnabled = false;
            }
        }
    }
}
