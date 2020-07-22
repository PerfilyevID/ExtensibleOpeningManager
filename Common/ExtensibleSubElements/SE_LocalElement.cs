using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Extensible;
using ExtensibleOpeningManager.Matrix;
using System;
using System.Collections.Generic;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common.ExtensibleSubElements
{
    public class SE_LocalElement : ExtensibleSubElement
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
                    { Variables.type_subelement_local_element,
                    Element.Id.ToString(),
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
                    if (ExtensibleTools.GetSubElementMeta(Parent.Instance, this).ToString() != this.ToString())
                    {
                        return Collections.SubStatus.Changed;
                    }
                    return Collections.SubStatus.Applied;
                }
            }
        }
        public SE_LocalElement(Element element)
        {
            Element = element;
            try
            {
                GeometryElement geometryElement;
                if (element.GetType() == typeof(FamilyInstance))
                {
                    FamilyInstance instance = element as FamilyInstance;
                    geometryElement = instance.Symbol.get_Geometry(new Options() { IncludeNonVisibleObjects = false, DetailLevel = ViewDetailLevel.Fine });
                }
                else
                {
                    geometryElement = element.get_Geometry(new Options() { IncludeNonVisibleObjects = false, DetailLevel = ViewDetailLevel.Fine });
                }
                Solid = MatrixElement.GetSolidOfElement(element);
            }
            catch (Exception) { }
        }
    }
}
