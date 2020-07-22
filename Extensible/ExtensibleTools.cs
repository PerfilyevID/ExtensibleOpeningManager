using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using System;
using System.Collections.Generic;
using System.Linq;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Extensible
{
    public static class ExtensibleTools
    {
        public static void ApplySubElements(ExtensibleElement element)
        {
            List<string> parts = new List<string>();
            foreach (ExtensibleSubElement subElement in element.SubElements)
            {
                parts.Add(subElement.ToString());
            }
            ExtensibleController.Write(element.Instance, ExtensibleParameter.SubElementsCollection, string.Join(Variables.separator_element, parts));
        }
        public static void ApplyInstance(ExtensibleElement element)
        {
            ExtensibleController.Write(element.Instance, ExtensibleParameter.Instance, element.ToString());
        }
        public static void AddComment(FamilyInstance instance, ExtensibleComment comment)
        {
            List<string> parts = ExtensibleController.Read(instance, ExtensibleParameter.CommentsCollection).Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries).ToList();
            parts.Add(comment.ToString());
            ExtensibleController.Write(instance, ExtensibleParameter.CommentsCollection, string.Join(Variables.separator_element, parts));
        }
        public static void RemoveComment(FamilyInstance instance, ExtensibleComment comment)
        {
            List<string> reWriteComments = new List<string>();
            foreach (string c in ExtensibleController.Read(instance, ExtensibleParameter.CommentsCollection).Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = c.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.None);
                if (parts[0] == comment.Message && parts[1] == comment.User && parts[2] == comment.Time.ToString() && parts[3] == comment.Department.ToString())
                {
                    continue;
                }
                else
                {
                    reWriteComments.Add(c);
                }
            }
            ExtensibleController.Write(instance, ExtensibleParameter.CommentsCollection, string.Join(Variables.separator_element, reWriteComments));
        }
        public static void SetStatus(FamilyInstance instance, Status status)
        {
            ExtensibleController.Write(instance, ExtensibleParameter.Status, status.ToString());
        }
        public static void AddSubElement(FamilyInstance instance, ExtensibleSubElement subElement)
        {
            List<string> parts = ExtensibleController.Read(instance, ExtensibleParameter.SubElementsCollection).Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries).ToList();
            parts.Add(subElement.ToString());
            ExtensibleController.Write(instance, ExtensibleParameter.SubElementsCollection, string.Join(Variables.separator_element, parts));
        }
        public static void RemoveSubElement(FamilyInstance instance, ExtensibleSubElement subElement)
        {
            List<string> notRemovedElements = new List<string>();
            foreach (string c in ExtensibleController.Read(instance, ExtensibleParameter.SubElementsCollection).Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = c.Split(new string[] { Variables.separator_sub_sub_element }, StringSplitOptions.None);
                if (subElement.GetType() == typeof(SE_LinkedElement) || subElement.GetType() == typeof(SE_LinkedInstance))
                {
                    if (parts[1] == subElement.Element.Id.ToString() && parts[2] == subElement.LinkId.ToString())
                    {
                        continue;
                    }
                    else
                    {
                        notRemovedElements.Add(c);
                    }
                }
                if(subElement.GetType() == typeof(SE_LocalElement))
                {
                    if (parts[1] == subElement.Element.Id.ToString() && parts[0] == Variables.type_subelement_local_element)
                    {
                        continue;
                    }
                    else
                    {
                        notRemovedElements.Add(c);
                    }
                }

            }
            ExtensibleController.Write(instance, ExtensibleParameter.SubElementsCollection, string.Join(Variables.separator_element, notRemovedElements));
        }
        public static List<ExtensibleComment> GetSubElementComments(ExtensibleSubElement element)
        {
            List<ExtensibleComment> comments = new List<ExtensibleComment>();
            if (element.Element.GetType() != typeof(FamilyInstance)) { return comments; }
            foreach (ExtensibleComment comment in ExtensibleComment.TryParseCollection(ExtensibleController.Read(element.Element as FamilyInstance, ExtensibleParameter.CommentsCollection)))
            {
                comments.Add(comment);
            }
            return comments;
        }
        public static string GetSubElementMeta(FamilyInstance instance, ExtensibleSubElement subElement)
        {
            try
            {
                foreach (string part in ExtensibleController.Read(instance, ExtensibleParameter.SubElementsCollection).Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = part.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.None);
                    if (subElement.GetType() == typeof(SE_LinkedElement) || subElement.GetType() == typeof(SE_LinkedInstance))
                    {
                        if (parts[1] == subElement.Element.Id.ToString() && parts[2] == subElement.LinkId.ToString())
                        {
                            return part;
                        }

                    }
                    if (subElement.GetType() == typeof(SE_LocalElement))
                    {
                        if (parts[1] == subElement.Element.Id.ToString() && parts[0] == Variables.type_subelement_local_element)
                        {
                            return part;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PrintError(e);
            }
            return Variables.empty;
        }
        public static void SetWall(FamilyInstance instance, SE_LinkedWall wall)
        {
            ExtensibleController.Write(instance, ExtensibleParameter.Wall, wall.ToString());
        }
    }
}
