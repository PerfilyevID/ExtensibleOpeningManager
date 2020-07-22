using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Extensible
{
    public static class ExtensibleController
    {
        public static string Read(FamilyInstance instance, ExtensibleParameter parameter)
        {
            try
            {
                Entity entity = instance.GetEntity(GetCurrentSchema());
                return entity.Get<string>(ExtensibleParameter_String[(int)parameter]);
            }
            catch (Exception)
            {
                return Variables.empty;
            }
            
        }
        public static void Write(FamilyInstance instance, ExtensibleParameter parameter, string value)
        {
                try
                {
                    Entity entity = instance.GetEntity(GetCurrentSchema());
                    entity.Set<string>(ExtensibleParameter_String[(int)parameter], value);
                    instance.SetEntity(entity);
                }
                catch (Exception)
                {
                    Entity entity = new Entity(GetCurrentSchema());
                    entity.Set<string>(ExtensibleParameter_String[(int)parameter], value);
                    instance.SetEntity(entity);
                }
        }
        public static Schema CreateNewSchema()
        {
            SchemaBuilder schemaBuilder = new SchemaBuilder(Variables.schema);
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBuilder.AddSimpleField(ExtensibleParameter_String[(int)ExtensibleParameter.Document], typeof(string));
            schemaBuilder.AddSimpleField(ExtensibleParameter_String[(int)ExtensibleParameter.Instance], typeof(string));
            schemaBuilder.AddSimpleField(ExtensibleParameter_String[(int)ExtensibleParameter.Status], typeof(string));
            schemaBuilder.AddSimpleField(ExtensibleParameter_String[(int)ExtensibleParameter.Wall], typeof(string));
            schemaBuilder.AddSimpleField(ExtensibleParameter_String[(int)ExtensibleParameter.SubElementsCollection], typeof(string));
            schemaBuilder.AddSimpleField(ExtensibleParameter_String[(int)ExtensibleParameter.CommentsCollection], typeof(string));
            schemaBuilder.SetSchemaName("eom_kpln");
            Schema schema = schemaBuilder.Finish();
            return schema;
        }
        private static Schema CurrentSchemaValue { get; set; }
        public static string ModuleName { get; private set; }

        public static Schema GetCurrentSchema()
        {
            if (CurrentSchemaValue == null)
            {
                Schema schema = Schema.Lookup(Variables.schema);
                if (schema == null)
                { CurrentSchemaValue = CreateNewSchema(); }
                else
                { CurrentSchemaValue = schema; }
                return CurrentSchemaValue;
            }
            else
            { return CurrentSchemaValue; }
        }
    }
}
