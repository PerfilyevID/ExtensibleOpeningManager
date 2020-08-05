using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Controll;
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
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Forms
{
    /// <summary>
    /// Логика взаимодействия для RevitLinkPicker.xaml
    /// </summary>
    public partial class RevitLinkPicker : Window
    {
        public RevitLinkPicker(List<RevitLinkInstance> instances)
        {
            Owner = ModuleData.RevitWindow;
            InitializeComponent();
            LinkControll.DataContext = instances;
            List<RLI_element> elements = new List<RLI_element>();
            foreach (RevitLinkInstance i in instances)
            {
                elements.Add(new RLI_element(i));
            }
            LinkControll.DataContext = new WPFSource<RLI_element>(elements);
        }

        private void OnBtnApply(object sender, RoutedEventArgs args)
        {
            List<RevitLinkInstance> revitLinkInstances = new List<RevitLinkInstance>();
            WPFSource<RLI_element> collection = LinkControll.DataContext as WPFSource<RLI_element>;
            foreach (RLI_element wpfElement in collection.Collection)
            {
                if (wpfElement.IsChecked && wpfElement.IsEnabled)
                {
                    revitLinkInstances.Add(wpfElement.Source as RevitLinkInstance);
                }
            }
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
        }
    }
}
