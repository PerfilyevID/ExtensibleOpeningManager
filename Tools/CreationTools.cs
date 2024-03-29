﻿using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Common;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Forms;
using ExtensibleOpeningManager.Tools.Instances;
using System.Collections.Generic;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Tools
{
    public static class CreationTools
    {
        public static bool HasInvalidIntersections(List<ExtensibleElement> extensibleElements, SE_LinkedWall wall)
        {
            foreach (ExtensibleElement el in extensibleElements)
            {
                foreach (ExtensibleSubElement sEl in el.SubElements)
                {
                    if (sEl.GetType() != typeof(SE_LinkedInstance))
                    {
                        Intersection i = new Intersection(sEl.Element, IntersectionTools.SolidIntersection(wall.Solid, sEl.Solid));
                        if (!i.IsValid) 
                        {
                            Dialogs.ShowDialog("Не удалось определить пересечение одного или нескольких субэлементов с основой!", "Ошибка");
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static FamilyInstance GroupInstance(SE_LinkedWall wall, List<ExtensibleElement> elements, Document doc)
        {
            string defaultType = "";
            HashSet<string> types = new HashSet<string>();
            string defaultTypesDescription = "";
            List<Intersection> Intersections = new List<Intersection>();
            foreach (ExtensibleElement element in elements)
            {
                foreach (ExtensibleSubElement subElement in element.SubElements)
                {
                    if (subElement.GetType() == typeof(SE_LinkedInstance))
                    {
                        types.Add((subElement.Element as FamilyInstance).Symbol.Name);
                        defaultType = (subElement.Element as FamilyInstance).Symbol.Name;
                    }
                    if (subElement.GetType() == typeof(SE_LinkedInstance))
                    {
                        if ((subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_mep_round)
                        {
                            Intersections.Add(new Intersection(subElement.Element, subElement.Solid));
                        }
                        else
                        {
                            Intersections.Add(new Intersection(subElement.Element, IntersectionTools.SolidIntersection(wall.Solid, subElement.Solid)));
                        }
                    }
                    else
                    {
                        Intersections.Add(new Intersection(subElement.Element, IntersectionTools.SolidIntersection(wall.Solid, subElement.Solid)));
                    }
                }
            }
            if (UserPreferences.Department == Department.MEP)
            {
                PlaceParameters creationParameters = new PlaceParameters(wall, Intersections, doc);
                if (creationParameters.Width >= UserPreferences.MinInstanceWidth / 304.8 && creationParameters.Height >= UserPreferences.MinInstanceHeight / 304.8)
                {
                    return CreateFamilyInstance(creationParameters, doc, SymbolType.Square);
                }
                return null;
            }
            else 
            {
                if (types.Count > 1)
                {
                    foreach (var i in types)
                    {
                        if (defaultTypesDescription != "")
                        { defaultTypesDescription += ", "; }
                        defaultTypesDescription += i; }
                    PlaceParameters creationParameters = new PlaceParameters(wall, Intersections, doc);
                    if (creationParameters.Width >= UserPreferences.MinInstanceWidth / 304.8 && creationParameters.Height >= UserPreferences.MinInstanceHeight / 304.8)
                    {
                        FamilyInstance createdInstance = CreateFamilyInstance(wall, creationParameters, doc, SymbolType.Square, "Несколько категорий");
                        try
                        {
                            createdInstance.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set(defaultTypesDescription);
                        }
                        catch (System.Exception) { }
                        return createdInstance;
                    }
                    return null;
                }
                else
                {
                    PlaceParameters creationParameters = new PlaceParameters(wall, Intersections, doc);
                    if (creationParameters.Width >= UserPreferences.MinInstanceWidth / 304.8 && creationParameters.Height >= UserPreferences.MinInstanceHeight / 304.8)
                    {
                        return CreateFamilyInstance(wall, creationParameters, doc, SymbolType.Square, defaultType);
                    }
                    return null;
                }
            }
        }
        public static FamilyInstance CreateFamilyInstance(SE_LinkedWall wall, ExtensibleSubElement subElement, Document doc, SizeOptions SizeOptions = null)
        {
            Intersection intersection = new Intersection(subElement.Element, IntersectionTools.SolidIntersection(wall.Solid, subElement.Solid));
            if (UserPreferences.Department != Department.MEP)
            {
                string symbolType = (subElement.Element as FamilyInstance).Symbol.Name;
                PlaceParameters creationParameters = null;
                SymbolType type = SymbolType.Square;
                if (subElement.GetType() == typeof(SE_LinkedInstance))
                {
                    if ((subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_mep_round || (subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_ar_round || (subElement.Element as FamilyInstance).Symbol.FamilyName == Variables.family_kr_round)
                    {
                        type = SymbolType.Round;
                        intersection = new Intersection(subElement.Element, subElement.Solid);
                        creationParameters = new PlaceParameters(wall, intersection, doc);
                    }
                    else
                    {
                        creationParameters = new PlaceParameters(wall, intersection, doc);
                    }
                }
                if (SizeOptions == null)
                {
                    if (creationParameters.Width >= UserPreferences.MinInstanceWidth / 304.8 && creationParameters.Height >= UserPreferences.MinInstanceHeight / 304.8)
                    {
                        return CreateFamilyInstance(wall, creationParameters, doc, type, symbolType);
                    }
                }
                else
                {
                    if (creationParameters.Width >= SizeOptions.MinWidth && creationParameters.Height >= SizeOptions.MinHeight && creationParameters.Width <= SizeOptions.MaxWidth && creationParameters.Height <= SizeOptions.MaxHeight)
                    {
                        return CreateFamilyInstance(wall, creationParameters, doc, type, symbolType);
                    }
                }
            }
            else
            {
                PlaceParameters creationParameters = new PlaceParameters(wall, intersection, doc);
                if (SizeOptions == null)
                {
                    if (creationParameters.Width >= UserPreferences.MinInstanceWidth / 304.8 && creationParameters.Height >= UserPreferences.MinInstanceHeight / 304.8)
                    {
                        return CreateFamilyInstance(creationParameters, doc, SymbolType.Square);
                    }

                }
                else
                {
                    if (creationParameters.Width >= SizeOptions.MinWidth && creationParameters.Height >= SizeOptions.MinHeight && creationParameters.Width <= SizeOptions.MaxWidth && creationParameters.Height <= SizeOptions.MaxHeight)
                    {
                        return CreateFamilyInstance(creationParameters, doc, SymbolType.Square);
                    }
                }

            }
            return null;
        }
        public static FamilyInstance CreateFamilyInstance(SE_LinkedWall wall, Intersection intersection, Document doc)
        {
            PlaceParameters creationParameters = new PlaceParameters(wall, intersection, doc);
            if (creationParameters.Width >= UserPreferences.MinInstanceWidth / 304.8 && creationParameters.Height >= UserPreferences.MinInstanceHeight / 304.8)
            {
                return CreateFamilyInstance(creationParameters, doc, SymbolType.Square);
            }
            return null;
        }
        private static FamilyInstance CreateFamilyInstance(PlaceParameters parameters, Document doc, SymbolType type, string subType = null)
        {
            FamilyInstance instance = doc.Create.NewFamilyInstance(parameters.Position, GetCreationSymbol(doc, type, subType), parameters.Level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            doc.Regenerate();
            ElementTransformUtils.RotateElement(doc, instance.Id, Line.CreateBound(parameters.Position, new XYZ(parameters.Position.X, parameters.Position.Y, parameters.Position.Z + 1)), parameters.GetAngle(instance));
            instance.LookupParameter(Variables.parameter_height).Set(parameters.Height);
            instance.LookupParameter(Variables.parameter_width).Set(parameters.Width);
            instance.LookupParameter(Variables.parameter_thickness).Set(parameters.Thickness);
            double offsetUp = parameters.OffsetUp;
            if (offsetUp < 0) { offsetUp = 0; }
            double offsetDown = parameters.OffsetDown;
            if (offsetDown < 0) { offsetDown = 0; }
            instance.LookupParameter(Variables.parameter_offset_up).Set(offsetUp);
            instance.LookupParameter(Variables.parameter_offset_down).Set(offsetDown);
            instance.LookupParameter(Variables.parameter_offset_bounds).Set(parameters.Offset);
            doc.Regenerate();
            instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(parameters.Elevation);
            doc.Regenerate();
            return instance;
        }
        private static FamilyInstance CreateFamilyInstance(SE_LinkedWall wall, PlaceParameters parameters, Document doc, SymbolType type, string subType = null)
        {
            FamilyInstance instance = doc.Create.NewFamilyInstance(parameters.Position, GetCreationSymbol(doc, type, subType), wall.Wall, parameters.Level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            doc.Regenerate();
            instance.LookupParameter(Variables.parameter_height).Set(parameters.Height);
            if (type != SymbolType.Round)
            {
                instance.LookupParameter(Variables.parameter_width).Set(parameters.Width);
            }
            instance.LookupParameter(Variables.parameter_thickness).Set(parameters.Thickness);
            double offsetUp = parameters.OffsetUp;
            if (offsetUp < 0) { offsetUp = 0; }
            double offsetDown = parameters.OffsetDown;
            if (offsetDown < 0) { offsetDown = 0; }
            instance.LookupParameter(Variables.parameter_offset_up).Set(offsetUp);
            instance.LookupParameter(Variables.parameter_offset_down).Set(offsetDown);
            instance.LookupParameter(Variables.parameter_offset_bounds).Set(parameters.Offset);
            doc.Regenerate();
            instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(parameters.Elevation);
            doc.Regenerate();
            return instance;
        }
        public static FamilySymbol GetCreationSymbol(Document doc, SymbolType type, string subType)
        {
            switch (UserPreferences.Department)
            {
                case Department.AR:
                    if (subType == null)
                    {
                        if (type == SymbolType.Round) 
                        { 
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_ar_round, "АР"); 
                        }
                        if (type == SymbolType.Square)
                        { 
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_ar_square, "АР");
                        }                    
                    }
                    else
                    {
                        if (type == SymbolType.Round) 
                        { 
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_ar_round, subType); 
                        }
                        if (type == SymbolType.Square)
                        { 
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_ar_square, subType); 
                        }
                    }
                    break;
                case Department.KR:
                    if (subType == null)
                    {
                        if (type == SymbolType.Round) 
                        {
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_kr_round, "КР"); 
                        }
                        if (type == SymbolType.Square)
                        { 
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_kr_square, "КР"); 
                        }
                    }
                    else
                    {
                        if (type == SymbolType.Round) 
                        {
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_kr_round, subType);
                        }
                        if (type == SymbolType.Square) 
                        { 
                            return FamilyTools.GetFamilySymbol(doc, Variables.family_kr_square, subType);
                        }
                    }
                    break;
                case Department.MEP:
                    if (type == SymbolType.Round)
                    { 
                        return FamilyTools.GetFamilySymbol(doc, Variables.family_mep_round, UserPreferences.SubDepartment);
                    }
                    if (type == SymbolType.Square)
                    { 
                        return FamilyTools.GetFamilySymbol(doc, Variables.family_mep_square, UserPreferences.SubDepartment); 
                    }
                    break;
                default:
                    break;
            }
            throw new System.Exception("Не удалось определить типоразмер элемента!");
        }
    }
}
