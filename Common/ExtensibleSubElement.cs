using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common
{
    public abstract class ExtensibleSubElement
    {
        public virtual ExtensibleElement Parent { get; protected set; }
        public virtual ElementId LinkId { get; protected set; }
        public virtual string SavedData { get; protected set; }
        public virtual Solid Solid { get; protected set; }
        public virtual SubStatus Status { get; protected set; }
        public virtual List<ExtensibleComment> Comments { get; protected set; }
        public virtual Element Element { get; protected set; }
        public virtual void SetParent(ExtensibleElement element)
        {
            SavedData = ExtensibleTools.GetSubElementMeta(element.Instance, this);
            Parent = element;
        }
        public static List<ExtensibleSubElement> TryParseCollection(ExtensibleElement element, string value)
        {
            List<ExtensibleSubElement> subElements = new List<ExtensibleSubElement>();
            foreach (string part in value.Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string[] parts = part.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts[0] == Variables.type_subelement_linked_instance)
                    {
                        ElementId linkId = new ElementId(int.Parse(parts[2]));
                        ElementId elementId = new ElementId(int.Parse(parts[1]));
                        RevitLinkInstance linkInstance = null;
                        try
                        {
                            linkInstance = CollectorTools.GetRevitLinkById(linkId, element.Instance.Document);
                        }
                        catch (Exception) { }
                        Element linkElement = null;
                        try
                        {
                            linkElement = linkInstance.GetLinkDocument().GetElement(elementId);
                        }
                        catch (Exception) { }
                        ExtensibleSubElement subEl = new SE_LinkedInstance(linkInstance, linkElement);
                        subEl.SetParent(element);
                        subElements.Add(subEl);
                    }
                    if (parts[0] == Variables.type_subelement_local_element)
                    {
                        ElementId elementId = new ElementId(int.Parse(parts[1]));
                        Element linkElement = null;
                        try
                        {
                            linkElement = element.Instance.Document.GetElement(elementId);
                        }
                        catch (Exception) { }
                        ExtensibleSubElement subEl = new SE_LocalElement(linkElement);
                        subEl.SetParent(element);
                        subElements.Add(subEl);
                    }
                    if (parts[0] == Variables.type_subelement_linked_element)
                    {
                        ElementId linkId = new ElementId(int.Parse(parts[2]));
                        ElementId elementId = new ElementId(int.Parse(parts[1]));
                        RevitLinkInstance linkInstance = null;
                        try
                        {
                            linkInstance = CollectorTools.GetRevitLinkById(linkId, element.Instance.Document);
                        }
                        catch (Exception) { }
                        Element linkElement = null;
                        try
                        {
                            linkElement = linkInstance.GetLinkDocument().GetElement(elementId);
                        }
                        catch (Exception) { }
                        ExtensibleSubElement subEl = new SE_LinkedElement(linkInstance, linkElement);
                        subEl.SetParent(element);
                        subElements.Add(subEl);
                    }
                }
                catch (Exception e) { PrintError(e); }
            }
            return subElements;
        }
    }
}