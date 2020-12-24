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
    /// Логика взаимодействия для UpdateByDocument.xaml
    /// </summary>
    public partial class UpdateByDocument : Window
    {
        public UpdateByDocument()
        {
#if Revit2020_AR || Revit2020_KR || Revit2020_MEP
            Owner = ModuleData.RevitWindow;
#endif
#if Revit2018_AR || Revit2018_KR || Revit2018_MEP
            
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = ModuleData.MainWindowHandle;
#endif
            InitializeComponent();
        }
    }
}
