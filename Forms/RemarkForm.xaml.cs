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
            Owner = ModuleData.RevitWindow;
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
