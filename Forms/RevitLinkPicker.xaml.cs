using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Controll;
using System;
using System.Collections.Generic;
using System.Windows;
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
            #if Revit2020
            Owner = ModuleData.RevitWindow;
            #endif
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
            Dialogs.ShowDialog("При работе с данным инструментом необходимо предварительно подготовить и открыть 3D вид с настроенной графикой отображения специально для расстановки отверстий", "Совет");
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
