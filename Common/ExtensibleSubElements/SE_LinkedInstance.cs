using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Extensible;
using System;
using System.Collections.Generic;

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
        public override ElementId LinkId
        {
            get
            {
                return LinkId;
            }
        }
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
                    Element.LevelId.ToString()});
            }
            catch (Exception)
            {
                return Variables.empty;
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
                    if (ExtensibleTools.GetSubElementMeta(Parent.Instance, this) != this.ToString())
                    {
                        return Collections.SubStatus.Changed;
                    }
                    return Collections.SubStatus.Applied;
                }
            }
        }
        public SE_LinkedInstance(Reference reference, Document doc)
        {
            RevitLinkInstance linkInstance = doc.GetElement(reference) as RevitLinkInstance;
            Document linkedDocument = linkInstance.GetLinkDocument();
            Transform transform = linkInstance.GetTotalTransform();
            Element = linkedDocument.GetElement(reference.LinkedElementId) as Element;
            foreach (GeometryObject geometry in Element.get_Geometry(new Options() { DetailLevel = ViewDetailLevel.Fine, IncludeNonVisibleObjects = false }).GetTransformed(transform))
            {
                if (geometry.GetType() == typeof(Solid))
                {
                    if (Solid == null)
                    {
                        Solid = geometry as Solid;
                    }
                    else
                    {
                        if (Solid.Volume < (geometry as Solid).Volume)
                        {
                            Solid = geometry as Solid;
                        }
                    }
                }
            }
        }
        public SE_LinkedInstance(RevitLinkInstance linkInstance, Element element)
        {
            Transform transform = linkInstance.GetTotalTransform();
            Element = element;
            try
            {
                foreach (GeometryObject geometry in Element.get_Geometry(new Options() { DetailLevel = ViewDetailLevel.Fine, IncludeNonVisibleObjects = false }).GetTransformed(transform))
                {
                    if (geometry.GetType() == typeof(Solid))
                    {
                        if (Solid == null)
                        {
                            Solid = geometry as Solid;
                        }
                        else
                        {
                            if (Solid.Volume < (geometry as Solid).Volume)
                            {
                                Solid = geometry as Solid;
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
