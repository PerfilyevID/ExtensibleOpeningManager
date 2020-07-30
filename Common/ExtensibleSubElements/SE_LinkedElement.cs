using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Matrix;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common.ExtensibleSubElements
{
    public class SE_LinkedElement : ExtensibleSubElement
    {
        public override List<ExtensibleComment> Comments
        {
            get
            {
                return new List<ExtensibleComment>();
            }
        }
        public override ElementId LinkId { get; protected set; }
        public override string ToString()
        {
            try
            {
                return string.Join(Variables.separator_sub_element, new string[]
                    { Variables.type_subelement_linked_element,
                    Element.Id.ToString(),
                    LinkId.ToString(),
                    ExtensibleConverter.ConvertLocation(Element.Location),
                    Element.GetTypeId().ToString(),
                    ExtensibleConverter.ConvertDouble(Solid.Volume),
                    ExtensibleConverter.ConvertDouble(Solid.SurfaceArea),
                    Element.LevelId.ToString()});
            }
            catch (Exception e)
            {
                PrintError(e);
                if (Value != null)
                { 
                    return Value; 
                }
                else
                { 
                    return string.Empty; 
                }
            }
        }
        public override Collections.SubStatus Status
        {
            get
            {
                if (Element == null)
                {
                    return Collections.SubStatus.NotFound;
                }
                if (Parent == null)
                {
                    return Collections.SubStatus.NotFound;
                }
                else
                {
                    try
                    {
                        if (ExtensibleTools.GetSubElementMeta(Parent.Instance, this) != this.ToString())
                        {
                            return Collections.SubStatus.Changed;
                        }
                        return Collections.SubStatus.Applied;
                    }
                    catch (Exception)
                    {
                        return Collections.SubStatus.NotFound;
                    }

                }
            }
        }
        public SE_LinkedElement(Reference reference, Document doc)
        {
            RevitLinkInstance linkInstance = doc.GetElement(reference) as RevitLinkInstance;
            Document linkedDocument = linkInstance.GetLinkDocument();
            Transform transform = linkInstance.GetTotalTransform();
            Element = linkedDocument.GetElement(reference.LinkedElementId) as Element;
            Id = Element.Id.IntegerValue;
            Solid = SolidUtils.CreateTransformed(GeometryTools.GetSolidOfElement(Element), transform);
            LinkId = linkInstance.Id;
        }
        private string Value { get; set; }
        public SE_LinkedElement(string value)
        {
            Id = int.Parse(value.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries)[0], System.Globalization.NumberStyles.Integer);
            Element = null;
            Solid = null;
            Value = value;
            Value = this.ToString();
        }
        public SE_LinkedElement(RevitLinkInstance linkInstance, Element element)
        {
            Id = element.Id.IntegerValue;
            Transform transform = linkInstance.GetTotalTransform();
            Element = element;
            Solid = SolidUtils.CreateTransformed(GeometryTools.GetSolidOfElement(Element), transform);
            LinkId = linkInstance.Id;
            Value = this.ToString();
        }
    }
}
