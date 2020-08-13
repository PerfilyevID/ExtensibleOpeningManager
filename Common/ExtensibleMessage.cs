using KPLN_Loader.Common;
using System;
using System.Windows;

namespace ExtensibleOpeningManager.Common
{
    public abstract class ExtensibleMessage
    {
        public virtual DateTime Time { get; }
        public virtual UIElement GetUiElement()
        { return null; }
        protected string GetUserNameBySystemName(string systemName)
        {
            foreach (SQLUserInfo user in KPLN_Loader.Preferences.Users)
            {
                if (user.SystemName == systemName && user.Surname != "")
                {
                    return string.Format("{0} {1} {2}.", user.Family, user.Name, user.Surname[0]);
                }
                if (user.SystemName == systemName && user.Surname == "")
                {
                    return string.Format("{0} {1}", user.Family, user.Name);
                }
            }
            return systemName;
        }
    }
}
