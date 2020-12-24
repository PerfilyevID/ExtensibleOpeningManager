using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Controll;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Tools.Instances;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Commands
{
    public class CommandTryAutoLink : IExecutableCommand
    {
        private List<RevitLinkInstance> Links = new List<RevitLinkInstance>();
        private ExtensibleElement Element { get; }
        public CommandTryAutoLink(ExtensibleElement element, List<RevitLinkInstance> links)
        {
            Element = element;
            foreach (RevitLinkInstance link in links)
            {
                Links.Add(link);
            }
        }
        public Result Execute(UIApplication app)
        {
            try
            {
                Document doc = app.ActiveUIDocument.Document;
                foreach (RevitLinkInstance Link in Links)
                {
                    try
                    {
                        Matrix.Matrix<Element> mepMatrix = new Matrix.Matrix<Element>(CollectorTools.GetMepElements(app.ActiveUIDocument.Document));
                        List<Intersection> mepContext = mepMatrix.GetContext(Element);
                        Matrix.Matrix<SE_LinkedWall> wallMatrix = new Matrix.Matrix<SE_LinkedWall>(CollectorTools.GetLinkedWalls(app.ActiveUIDocument.Document, Link));
                        List<Intersection> wallContext = wallMatrix.GetContext(Element);
                        int maxEl = -1;
                        Intersection maxWall = null;
                        foreach (Intersection wall in wallContext)
                        {
                            List<Intersection> context = new List<Intersection>();
                            foreach (Intersection mep in mepContext)
                            {
                                if (wall.Intersects(mep))
                                {
                                    if (!UiController.GetControllerByDocument(app.ActiveUIDocument.Document).IntersectionExist(mep, new SE_LinkedWall(Link, wall.Element as Wall)))
                                    {
                                        context.Add(mep);
                                    }
                                }
                            }
                            if (maxEl < context.Count)
                            {
                                maxEl = context.Count;
                                maxWall = wall;
                            }
                        }
                        if (maxEl > 0)
                        {
                            List<Intersection> context = new List<Intersection>();
                            foreach (Intersection mep in mepContext)
                            {
                                if (maxWall.Intersects(mep))
                                {
                                    if (!UiController.GetControllerByDocument(app.ActiveUIDocument.Document).IntersectionExist(mep, new SE_LinkedWall(Link, maxWall.Element as Wall)))
                                    {
                                        context.Add(mep);
                                    }
                                }
                            }
                            Element.Apply();
                            Element.SetWall(new SE_LinkedWall(Link, maxWall.Element as Wall));
                            foreach (Intersection i in context)
                            {
                                Element.AddSubElement(new SE_LocalElement(i.Element));
                            }
                            try
                            {
                                UiController controller = UiController.GetControllerByDocument(Element.Instance.Document);
                                controller.UpdateComments(controller.Selection[0].AllComments);
                            }
                            catch (Exception) { }
                            Element.AddComment(Variables.msg_autoJoined);
                            Element.Reject();
                            return Result.Succeeded;
                        }
                    }
                    catch (Exception) { }
                }
                return Result.Cancelled;
            }
            catch (Exception e)
            {
                PrintError(e);
                return Result.Failed;
            }
        }
    }
}
