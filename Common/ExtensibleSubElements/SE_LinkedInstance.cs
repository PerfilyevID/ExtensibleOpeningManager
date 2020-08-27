using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Tools;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common.ExtensibleSubElements
{
    public class SE_LinkedInstance : ExtensibleSubElement
    {
        public override List<ExtensibleComment> Comments
        {
            get
            {
                return ExtensibleTools.GetSubElementComments(this);
            }
        }
        public List<ExtensibleRemark> GetRemarks(ExtensibleElement parent)
        {
            try
            {
                return ExtensibleTools.GetSubElementRemarks(this, parent);
            }
            catch (Exception)
            {
                return new List<ExtensibleRemark>();
            }
        }
        public override ElementId LinkId { get; protected set; }
        public override string ToString()
        {
            try
            {
                return string.Join(Variables.separator_sub_element, new string[]
                    { Variables.type_subelement_linked_instance,
                    Element.Id.ToString(),
                    LinkId.ToString(),
                    ExtensibleConverter.ConvertLocation(Element.Location),
                    Element.GetTypeId().ToString(),
                    ExtensibleConverter.ConvertDouble(Solid.Volume),
                    ExtensibleConverter.ConvertDouble(Element.LookupParameter(Variables.parameter_height).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Element.LookupParameter(Variables.parameter_offset_bounds).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Element.LookupParameter(Variables.parameter_thickness).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Element.LookupParameter(Variables.parameter_width).AsDouble()),
                    ExtensibleConverter.ConvertPoint((Element as FamilyInstance).FacingOrientation),
                    Element.LevelId.ToString(),
                    Guid});
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
                        return ExtensibleTools.GetLinkedElementStatus(Element as FamilyInstance);
                    }
                    catch (Exception)
                    {
                        return Collections.SubStatus.NotFound;
                    }
                }
            }
        }
        private string Value { get; set; }
        private string Guid { get; set; }
        public SE_LinkedInstance(string value)
        {
            Id = int.Parse(value.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries)[1], System.Globalization.NumberStyles.Integer);
            Element = null;
            Solid = null;
            Value = value;
            LinkId = ElementId.InvalidElementId;
        }
        public SE_LinkedInstance(Reference reference, Document doc)
        {
            RevitLinkInstance linkInstance = doc.GetElement(reference) as RevitLinkInstance;
            Document linkedDocument = linkInstance.GetLinkDocument();
            Transform transform = linkInstance.GetTotalTransform();
            Element = linkedDocument.GetElement(reference.LinkedElementId) as Element;
            Id = Element.Id.IntegerValue;
            if (transform != null) { Solid = SolidUtils.CreateTransformed(GeometryTools.GetSolidOfElement(Element, ViewDetailLevel.Fine), transform); }
            else { Solid = GeometryTools.GetSolidOfElement(Element, ViewDetailLevel.Fine); }
            
            try
            {
                Guid = ExtensibleController.Read(Element as FamilyInstance, Collections.ExtensibleParameter.Document);
            }
            catch (Exception)
            {
                Guid = "-";
            }
            LinkId = linkInstance.Id;
            Value = this.ToString();
        }
        public SE_LinkedInstance(RevitLinkInstance linkInstance, Element element)
        {
            Transform transform = linkInstance.GetTotalTransform();
            Element = element;
            Id = Element.Id.IntegerValue;
            LinkId = linkInstance.Id;
            if ((element as FamilyInstance).Symbol.FamilyName == Variables.family_mep_round || (element as FamilyInstance).Symbol.FamilyName == Variables.family_mep_square)
            {
                if (transform != null) { Solid = SolidUtils.CreateTransformed(GeometryTools.GetSolidOfElement(element, ViewDetailLevel.Fine), transform); }
                else { Solid = GeometryTools.GetSolidOfElement(element, ViewDetailLevel.Fine); }
            }
            else
            {
                if (transform != null) { Solid = SolidUtils.CreateTransformed(GeometryTools.GetSolidOfElement(element, ViewDetailLevel.Coarse), transform); }
                else { Solid = GeometryTools.GetSolidOfElement(element, ViewDetailLevel.Coarse); }
            }
            try
            {
                Guid = ExtensibleController.Read(Element as FamilyInstance, Collections.ExtensibleParameter.Document);
            }
            catch (Exception)
            {
                Guid = "-";
            }
            Value = this.ToString();
        }
    }
}
