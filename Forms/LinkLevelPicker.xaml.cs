using Autodesk.Revit.DB;
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
    /// Логика взаимодействия для LinkLevelPicker.xaml
    /// </summary>
    public partial class LinkLevelPicker : Window
    {
        public static List<Level> PickedLevelInstances { get; set; }
        public LinkLevelPicker(List<Level> levels, string title)
        {
            Title = title;
            PickedLevelInstances = new List<Level>(); ;
#if Revit2020_AR || Revit2020_KR || Revit2020_MEP
            Owner = ModuleData.RevitWindow;
#endif
#if Revit2018_AR || Revit2018_KR || Revit2018_MEP
            
            WindowInteropHelper helper = new WindowInteropHelper(this);
            helper.Owner = ModuleData.MainWindowHandle;
#endif
            InitializeComponent();
            LinkControll.DataContext = levels;
            List<RLI_element> elements = new List<RLI_element>();
            foreach (Level i in levels.OrderBy(x => x.Elevation).ToList())
            {
                elements.Add(new RLI_element(i));
            }
            LinkControll.DataContext = new WPFSource<RLI_element>(elements);
        }

        private void OnBtnApply(object sender, RoutedEventArgs args)
        {
            List<Level> pickedLevels = new List<Level>();
            WPFSource<RLI_element> collection = LinkControll.DataContext as WPFSource<RLI_element>;
            foreach (RLI_element wpfElement in collection.Collection)
            {
                if (wpfElement.IsChecked && wpfElement.IsEnabled)
                {
                    pickedLevels.Add(wpfElement.Source as Level);
                }
            }
            PickedLevelInstances = pickedLevels;
            Close();
            /*
            if (revitLinkInstances.Count != 0)
            {
                try
                {
                    UiController.CurrentController.LoopController.Prepare(revitLinkInstances, this);
                }
                catch (Exception e)
                {
                    PrintError(e);
                    Close();
                }
            }
            */
        }
    }
}
