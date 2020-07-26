using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public LoopController(Document doc)
        {
            IsActive = false;
            try
            {
                Matrix = new Matrix<Element>(CollectorTools.GetMepElements(doc));
            }
            catch (Exception) { }
            Document = doc;
        }
        public void Prepare(List<RevitLinkInstance> revitLinks)
        {
            CreatedElements.Clear();
            UiController controller = UiController.GetControllerByDocument(Document);
            IsActive = true;
            Walls = new Queue<SE_LinkedWall>();
            Matrix = new Matrix<Element>(CollectorTools.GetMepElements(Document));
            int max = 0;
            List<SE_LinkedWall> walls = new List<SE_LinkedWall>();
            foreach (RevitLinkInstance instance in revitLinks)
            {
                max += CollectorTools.GetLinkedWalls(Document, instance).Count;
            }
            string format = "{0} из " + max.ToString() + " элементов обработано";
            using (Progress_Single progress = new Progress_Single("Подготовка", format, max))
            {
                foreach (RevitLinkInstance instance in revitLinks)
                {
                    foreach (SE_LinkedWall wall in CollectorTools.GetLinkedWalls(Document, instance))
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
                }
            }
            foreach (SE_LinkedWall wall in walls)
            {
                Walls.Enqueue(wall);
            }
            if (Walls.Count == 0)
            {
                Dialogs.ShowDialog("Новых пересечений не найдено!", "Предупреждение");
                IsActive = false;
                Matrix = null;
            }
            UiController.CurrentController.UpdateEnability();
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
