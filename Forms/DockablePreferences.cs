using DockableDialog.Forms;
using System;

namespace ExtensibleOpeningManager.Forms
{
    public static class DockablePreferences
    {
        public static DockableManager Page = new DockableManager();
        public static Guid PageGuid
        {
            get
            {
                switch (UserPreferences.Department)
                {
                    case Common.Collections.Department.AR:
                        return new Guid("42246bf5-7ea2-4ce9-94ef-61e87d352a4c");
                    case Common.Collections.Department.KR:
                        return new Guid("14c813d0-d736-4845-9ef9-9fa1ab80bafc");
                    case Common.Collections.Department.MEP:
                        return new Guid("f842f855-4e9d-4564-a4c3-987d8f2cfbd7");
                    default:
                        return new Guid("98ff5304-2f35-4bb0-a25e-4967aecd794d");
                }
            }
        }
    }
}
