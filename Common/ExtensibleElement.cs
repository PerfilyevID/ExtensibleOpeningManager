using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Extensible;
using static KPLN_Loader.Output.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using static ExtensibleOpeningManager.Common.Collections;
using ExtensibleOpeningManager.Tools.Instances;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Tools;
using ExtensibleOpeningManager.Controll;

namespace ExtensibleOpeningManager.Common
{
    public class ExtensibleElement
    {
        public int Id { get; set; }
        public List<ExtensibleSubElement> SubElements { get; set; }
        public SE_LinkedWall Wall { get; set; }
        public ElementId WallLinkId { get; set; }
        public Solid Solid {get; set;}
        public FamilyInstance Instance { get; set; }
        public Status Status { get; set; }
        public WallStatus WallStatus 
        {
            get
            {
                if (Wall == null) { return WallStatus.NotFound; }
                if (Wall.Wall == null) { return WallStatus.NotFound; }
                else
                {
                    if (Wall.ToString() != ExtensibleController.Read(Instance, ExtensibleParameter.Wall))
                    {
                        return WallStatus.NotCommited;
                    }
                    else
                    {
                        return WallStatus.Ok;
                    }
                }
            }
        }
        public VisibleStatus VisibleStatus
        {
            get
            {
                if (WallStatus != WallStatus.Ok || GotWarnings())
                {
                    return VisibleStatus.Alert;
                }
                else
                {
                    if (Status == Status.Applied)
                    {
                        return VisibleStatus.Ok;
                    }
                    else
                    {
                        return VisibleStatus.Warning;
                    }
                }
            }
        }
        public List<ExtensibleComment> AllComments
        {
            get
            {
                List<ExtensibleComment> comments = new List<ExtensibleComment>();
                foreach (ExtensibleComment comment in Comments)
                {
                    comments.Add(comment);
                }
                foreach (ExtensibleSubElement subElement in SubElements)
                {
                    foreach (ExtensibleComment comment in subElement.Comments)
                    {
                        comments.Add(comment);
                    }
                }
                return comments.OrderBy(o => o.Time).ToList();
            }
        }
        public List<ExtensibleComment> Comments { get; set; }
        public string SavedData { get; set; }
        public bool GotWarnings()
        {
            foreach (ExtensibleSubElement subElement in SubElements)
            {
                if (subElement.Status != SubStatus.Applied)
                {
                    return true;
                }
            }
            return false;
        }
        public void AddComment(string message)
        {
            ExtensibleComment comment = new ExtensibleComment(message, this);
            Comments.Add(comment);
            ExtensibleTools.AddComment(Instance, comment);
        }
        public void RemoveComment(ExtensibleComment comment)
        {
            ExtensibleTools.RemoveComment(Instance, comment);
            Comments.Remove(comment);
            UiController.CurrentController.UpdateComments(Comments);
        }
        public void Remove()
        {
            Instance.Document.Delete(Instance.Id);
        }
        private double GetAngle(FamilyInstance instance, XYZ lastOrientation)
        {
            XYZ instanceFacingOrientation = instance.FacingOrientation;
            double wallAngle = PlaceParameters.ConvertRadiansToDegrees(Math.Atan2(lastOrientation.X, lastOrientation.Y));
            if (wallAngle < 0) { wallAngle += 360; }
            double instanceAngle = PlaceParameters.ConvertRadiansToDegrees(Math.Atan2(instanceFacingOrientation.X, instanceFacingOrientation.Y));
            if (instanceAngle < 0) { instanceAngle += 360; }
            double trueAngle = instanceAngle - wallAngle;
            if (trueAngle < 0) { trueAngle += 360; }
            if (trueAngle > 360) { trueAngle -= 360; }
            double Angle = PlaceParameters.ConvertToRadians(trueAngle);
            return Angle;
        }
        public void Reset()
        {
            string[] values = SavedData.Split(new string[] { Variables.separator_element }, StringSplitOptions.None);
            if (values.Count() == 11)
            {
                try
                {
                    double offsetUp = Instance.LookupParameter(Variables.parameter_offset_up).AsDouble();
                    double offsetDown = Instance.LookupParameter(Variables.parameter_offset_down).AsDouble();
                    try { Instance.Symbol = Instance.Document.GetElement(new ElementId(int.Parse(values[4]))) as FamilySymbol; }
                    catch (Exception) { }
                    Instance.Document.Regenerate();
                    Instance.LookupParameter(Variables.parameter_offset_up).Set(offsetUp);
                    Instance.LookupParameter(Variables.parameter_offset_down).Set(offsetDown);
                    Instance.LookupParameter(Variables.parameter_height).Set(double.Parse(values[5]));
                    Instance.LookupParameter(Variables.parameter_offset_bounds).Set(double.Parse(values[6]));
                    Instance.LookupParameter(Variables.parameter_thickness).Set(double.Parse(values[7]));
                    Instance.LookupParameter(Variables.parameter_width).Set(double.Parse(values[8]));
                    Instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(new ElementId(int.Parse(values[10])));
                    XYZ currentLocation = (Instance.Location as LocationPoint).Point;
                    XYZ lastLocation = ExtensibleConverter.ConvertToPoint(values[2]);
                    XYZ lastOrientation = ExtensibleConverter.ConvertToPoint(values[3]);
                    ElementTransformUtils.MoveElement(Instance.Document, Instance.Id, new XYZ(lastLocation.X - currentLocation.X, lastLocation.Y - currentLocation.Y, lastLocation.Z - currentLocation.Z));
                    Instance.Document.Regenerate();
                    ElementTransformUtils.RotateElement(Instance.Document, Instance.Id, Line.CreateBound(currentLocation, new XYZ(currentLocation.X, currentLocation.Y, currentLocation.Z + 1)), GetAngle(Instance, lastOrientation));
                    Instance.Document.Regenerate();
                    Instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(double.Parse(values[9]));

                }
                catch (Exception e)
                {
                    PrintError(e);
                }
            }
        }
        public void Update()
        {
            if (Wall == null) { return; }
            List<Intersection> intersections = new List<Intersection>();
            foreach (ExtensibleSubElement subElement in SubElements)
            {
                Solid s = IntersectionTools.SolidIntersection(Wall.Solid, subElement.Solid);
                if (s.Volume > 0.000001)
                {
                    intersections.Add(new Intersection(subElement.Element, s));
                }
            }
            PlaceParameters placeParameters = new PlaceParameters(Wall, intersections, Instance.Document);
            try
            {
                Instance.LookupParameter(Variables.parameter_height).Set(placeParameters.Height);
                Instance.LookupParameter(Variables.parameter_thickness).Set(placeParameters.Thickness);
                Instance.LookupParameter(Variables.parameter_width).Set(placeParameters.Width);
                Instance.LookupParameter(Variables.parameter_offset_down).Set(placeParameters.OffsetDown);
                Instance.LookupParameter(Variables.parameter_offset_up).Set(placeParameters.OffsetUp);
                Instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(placeParameters.Level.Id);
                XYZ currentLocation = (Instance.Location as LocationPoint).Point;
                XYZ currentOrientation = Instance.FacingOrientation;
                XYZ lastLocation = placeParameters.Position;
                double currentAngle = (180 / Math.PI) * Math.Atan2(currentOrientation.X, currentOrientation.Y);
                double trueAngle = currentAngle - placeParameters.GetAngle(Instance);
                double angle = (180 / Math.PI) * trueAngle;
                ElementTransformUtils.RotateElement(Instance.Document, Instance.Id, Line.CreateBound(currentLocation, new XYZ(currentLocation.X, currentLocation.Y, currentLocation.Z + 1)), placeParameters.GetAngle(Instance));
                ElementTransformUtils.MoveElement(Instance.Document, Instance.Id, new XYZ(lastLocation.X - currentLocation.X, lastLocation.Y - currentLocation.Y, lastLocation.Z - currentLocation.Z));
                Instance.Document.Regenerate();
                Instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(placeParameters.Elevation);
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }
        public bool IsAbleToUpdate()
        {
            if (Wall != null && !HasUnfoundSubElements() && WallStatus != WallStatus.NotFound)
            {
                List<Intersection> intersections = new List<Intersection>();
                foreach (ExtensibleSubElement subElement in SubElements)
                {
                    Intersection i = new Intersection(subElement.Element, IntersectionTools.GetIntersectionSolid(Wall.Solid, subElement.Solid));
                    if (!i.IsValid)
                    {
                        return false;
                    }
                    intersections.Add(i);
                }
                PlaceParameters parameters = new PlaceParameters(Wall, intersections, Instance.Document);
                if (parameters.Position.Equals((Instance.Location as LocationPoint).Point) ||
                    Math.Round(parameters.GetAngle(Instance), 5) != 0 ||
                    Math.Round(parameters.Elevation, 5) != Math.Round(Instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble(), 5) ||
                    Math.Round(parameters.Width, 5) != Math.Round(Instance.LookupParameter(Variables.parameter_width).AsDouble(), 5) ||
                    Math.Round(parameters.Height, 5) != Math.Round(Instance.LookupParameter(Variables.parameter_height).AsDouble(), 5) ||
                    parameters.Level.Id.IntegerValue != Instance.LevelId.IntegerValue ||
                    Math.Round(parameters.Thickness, 5) != Math.Round(Instance.LookupParameter(Variables.parameter_thickness).AsDouble(), 5))
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasUncommitedSubElements()
        {
            foreach (ExtensibleSubElement subElement in SubElements)
            { 
                if (subElement.Status == SubStatus.Changed)
                { return true; }
            }
            return false;
        }
        public bool HasUnfoundSubElements()
        {
            foreach (ExtensibleSubElement subElement in SubElements)
            {
                if (subElement.Status == SubStatus.NotFound)
                { return true; }
            }
            return false;
        }
        public void Apply()
        {
            ExtensibleTools.SetStatus(Instance, Collections.Status.Applied);
            Status = Status.Applied;
            Approve();
        }
        public void Approve()
        {
            ExtensibleTools.ApplyInstance(this);
            SavedData = this.ToString();
        }
        public void Reject()
        {
            ExtensibleTools.SetStatus(Instance, Collections.Status.Rejected);
            Status = Status.Rejected;
        }
        public void AddSubElement(ExtensibleSubElement element)
        {
            SubElements.Add(element);
            element.SetParent(this);
            ExtensibleTools.AddSubElement(Instance, element);
        }
        public void RemoveSubElement(ExtensibleSubElement element)
        {
            SubElements.Remove(element);
            ExtensibleTools.RemoveSubElement(Instance, element);
        }
        public void SetWall(SE_LinkedWall wall)
        {
            Wall = wall;
            ExtensibleTools.SetWall(Instance, Wall);
        }
        public void ApplySubElements()
        {
            ExtensibleTools.ApplySubElements(this);
        }
        public void ApplyWall()
        {
            ExtensibleController.Write(this.Instance, Collections.ExtensibleParameter.Wall, string.Join(Variables.separator_sub_element, new string[] { Wall.Wall.Id.ToString(), WallLinkId.ToString(), Wall.ToString()}));
        }
        public void SwapType()
        {
            try
            {
                double height = Instance.LookupParameter(Variables.parameter_height).AsDouble();
                double width = Instance.LookupParameter(Variables.parameter_width).AsDouble();
                double thickness = Instance.LookupParameter(Variables.parameter_thickness).AsDouble();
                double offset = Instance.LookupParameter(Variables.parameter_offset_bounds).AsDouble();
                double offset_down = Instance.LookupParameter(Variables.parameter_offset_down).AsDouble();
                double offset_up = Instance.LookupParameter(Variables.parameter_offset_up).AsDouble();
                double elevation = Instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                Instance.Symbol = FamilyTools.GetSwapType(Instance);
                Instance.Document.Regenerate();
                Instance.LookupParameter(Variables.parameter_height).Set(height);
                Instance.LookupParameter(Variables.parameter_offset_bounds).Set(offset);
                Instance.LookupParameter(Variables.parameter_offset_down).Set(offset_down);
                Instance.LookupParameter(Variables.parameter_offset_up).Set(offset_up);
                Instance.LookupParameter(Variables.parameter_thickness).Set(thickness);
                Instance.LookupParameter(Variables.parameter_width).Set(width);
                Instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(elevation);
            }
            catch (Exception e)
            {
                PrintError(e);
            }
        }
        public static ExtensibleElement GetExtensibleElementByInstance(FamilyInstance instance)
        {
            return new ExtensibleElement(instance);
        }
        public override string ToString()
        {
            try
            {
                return string.Join(Variables.separator_element, new string[]
                    { Instance.Id.ToString(),
                    Status.ToString(),
                    ExtensibleConverter.ConvertLocation(Instance.Location),
                    ExtensibleConverter.ConvertPoint(Instance.FacingOrientation),
                    Instance.Symbol.Id.ToString(),
                    ExtensibleConverter.ConvertDouble(Instance.LookupParameter(Variables.parameter_height).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Instance.LookupParameter(Variables.parameter_offset_bounds).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Instance.LookupParameter(Variables.parameter_thickness).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Instance.LookupParameter(Variables.parameter_width).AsDouble()),
                    ExtensibleConverter.ConvertDouble(Instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble()),
                    Instance.LevelId.ToString()});
            }
            catch (Exception)
            {
                return Variables.empty;
            }
        }
        private ExtensibleElement(FamilyInstance instance)
        {
            Id = instance.Id.IntegerValue;
            Instance = instance;
            Status = Status.Null;
            Wall = null;
            SubElements = new List<ExtensibleSubElement>();
            Comments = new List<ExtensibleComment>();
            SavedData = Variables.empty;
            foreach (ViewDetailLevel dl in new ViewDetailLevel[] { ViewDetailLevel.Coarse, ViewDetailLevel.Fine, ViewDetailLevel.Medium })
            {
                try
                {
                    foreach (GeometryObject geometry in instance.get_Geometry(new Options() { DetailLevel = ViewDetailLevel.Fine, IncludeNonVisibleObjects = true }))
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
            try
            {
                if (ExtensibleController.Read(Instance, ExtensibleParameter.Instance) != "" && ExtensibleController.Read(Instance, ExtensibleParameter.Instance) != Variables.empty)
                {
                    string[] values = ExtensibleController.Read(Instance, ExtensibleParameter.Instance).Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Count() == 11)
                    {
                        if (values[0] == Instance.Id.ToString())
                        {
                            Status status;
                            Enum.TryParse(values[1], out status); 
                            if (status != Status.Null) { Status = status; }
                            Wall = SE_LinkedWall.TryParse(Instance.Document, ExtensibleController.Read(Instance, ExtensibleParameter.Wall));
                            SavedData = ExtensibleController.Read(Instance, ExtensibleParameter.Instance);
                            Comments = ExtensibleComment.TryParseCollection(ExtensibleController.Read(Instance, ExtensibleParameter.CommentsCollection), this);
                            SubElements = ExtensibleSubElement.TryParseCollection(this, ExtensibleController.Read(Instance, ExtensibleParameter.SubElementsCollection));
                            if (this.ToString() != ExtensibleController.Read(Instance, ExtensibleParameter.Instance))
                            {
                                status = Status.Rejected;
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
