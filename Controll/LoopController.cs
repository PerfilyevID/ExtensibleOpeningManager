using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using ExtensibleOpeningManager.Matrix;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Forms;
using ExtensibleOpeningManager.Tools.Instances;
using ExtensibleOpeningManager.Commands;
using static KPLN_Loader.Output.Output;
using ExtensibleOpeningManager.Common;

namespace ExtensibleOpeningManager.Controll
{
    public class LoopController
    {
        public readonly List<ExtensibleElement> CreatedElements = new List<ExtensibleElement>();
        public bool IsActive { get; private set; }
        private Matrix<Element> Matrix { get; set; }
        private Document Document { get; set; }
        private Queue<SE_LinkedWall> Walls { get; set; }
        private int Max { get; set; }
        public LoopController(Document doc)
        {
            IsActive = false;
            Document = doc;
        }
        public void Prepare(List<RevitLinkInstance> revitLinks, RevitLinkPicker sender)
        {
            sender.Close();
            CreatedElements.Clear();
            UiController controller = UiController.GetControllerByDocument(Document);
            IsActive = true;
            Walls = new Queue<SE_LinkedWall>();
            Matrix = new Matrix<Element>(CollectorTools.GetMepElements(Document), true);
            int max = 0;
            List<SE_LinkedWall> walls = new List<SE_LinkedWall>();
            foreach (RevitLinkInstance instance in revitLinks)
            {
                try
                {
                    max += CollectorTools.GetLinkedWalls(Document, instance).Count;
                }
                catch (Exception)
                { }
            }
            string format = "{0} из " + max.ToString() + " стен обработано";
            using (Progress_Single progress = new Progress_Single("Поиск новых пересечений", format, max))
            {
                foreach (RevitLinkInstance instance in revitLinks)
                {
                    try
                    {
                        foreach (SE_LinkedWall wall in CollectorTools.GetLinkedWalls(Document, instance))
                        {
                            try
                            {
                                progress.Increment();
                                List<Intersection> intersections = Matrix.GetContext(wall);
                                if (intersections.Count != 0)
                                {
                                    foreach (Intersection i in intersections)
                                    {
                                        if (!controller.IntersectionExist(i, wall))
                                        {
                                            walls.Add(wall);
                                            max++;
                                            break;
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                PrintError(e);
                                break;
                            }
                        }
                    }
                    catch (Exception e) { PrintError(e); }
                }
            }
            foreach (SE_LinkedWall wall in walls)
            {
                Walls.Enqueue(wall);
            }
            Max = Walls.Count;
            if (Walls.Count == 0)
            {
                Dialogs.ShowDialog("Новых пересечений не найдено!", "Предупреждение");
                IsActive = false;
                Matrix = null;
            }
            UiController.CurrentController.UpdateEnability();
        }
        public void UpdateButtonToolTips()
        {
            if (IsActive)
            {
                DockablePreferences.Page.btnLoopNext.Content = string.Format("Далее {0}/{1}", (Max - Walls.Count).ToString(), Max.ToString());
                DockablePreferences.Page.btnLoopNext2.Content = string.Format("Далее {0}/{1}", (Max - Walls.Count).ToString(), Max.ToString());
            }
            else
            {
                DockablePreferences.Page.btnLoopNext.Content = "Далее";
                DockablePreferences.Page.btnLoopNext2.Content = "Далее";
            }
        }
        public void Next()
        {

            CreatedElements.Clear();
            if (Walls.Count != 0)
            {
                SE_LinkedWall currentWall = Walls.Dequeue();
                ModuleData.CommandQueue.Enqueue(new CommandZoomElement(currentWall));
                ModuleData.CommandQueue.Enqueue(new CommandLoopPlaceTaskOnPickedWall(currentWall));
                UiController.CurrentController.UpdateEnability();
            }
            else
            {
                Dialogs.ShowDialog("Больше пересечений не найдено!", "Предупреждение");
                IsActive = false;
                Matrix = null;
                UiController.CurrentController.UpdateEnability();
            }
        }
        public void Apply()
        {
            ModuleData.CommandQueue.Enqueue(new CommandLoopApprove(this));
        }
        public void Reject()
        {
            ModuleData.CommandQueue.Enqueue(new CommandLoopReject(this));
        }
        public void Skip()
        {
            ModuleData.CommandQueue.Enqueue(new CommandLoopSkip(this));
        }
    }
}
