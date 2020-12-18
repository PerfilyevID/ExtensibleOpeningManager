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

namespace ExtensibleOpeningManager.Forms
{
    /// <summary>
    /// Логика взаимодействия для OffsetPicker.xaml
    /// </summary>
    public partial class OffsetPicker : Window
    {
        public OffsetPicker()
        {
#if Revit2020
            Owner = ModuleData.RevitWindow;
#endif
#if Revit2018
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = ModuleData.MainWindowHandle;
#endif
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Dialogs.Double = double.Parse((btn.Content as string).Split(new string[] { "мм" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            Close();
        }
    }
}
