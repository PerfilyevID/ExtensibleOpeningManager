using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Matrix;
using System;
using System.Collections.Generic;

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
        public override ElementId LinkId
        {
            get
            {
                return ElementId.InvalidElementId;
            }
        }
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
        public SE_LinkedElement(Reference reference, Document doc)
        {
            RevitLinkInstance linkInstance = doc.GetElement(reference) as RevitLinkInstance;
            Document linkedDocument = linkInstance.GetLinkDocument();
            Transform transform = linkInstance.GetTotalTransform();
            Element = linkedDocument.GetElement(reference.LinkedElementId) as Element;
            Solid = SolidUtils.CreateTransformed(MatrixElement.GetSolidOfElement(Element), transform);
        }
        public SE_LinkedElement(RevitLinkInstance linkInstance, Element element)
        {
            Transform transform = linkInstance.GetTotalTransform();
            Element = element;
            Solid = SolidUtils.CreateTransformed(MatrixElement.GetSolidOfElement(Element), transform);
        }
    }
}
