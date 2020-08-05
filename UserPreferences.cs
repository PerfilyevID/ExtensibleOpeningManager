﻿using System;
using System.Collections.Generic;
using System.IO;
using static KPLN_Loader.Common.SQLDepartmentInfo;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager
{
    public static class UserPreferences
    {
        public static Department Department
        {
            get
            {
                if(KPLN_Loader.Preferences.User.Department.Code == "АР")
                {
                    return Department.AR;
                }
                if (KPLN_Loader.Preferences.User.Department.Code == "КР")
                {
                    return Department.KR;
                }
                if (KPLN_Loader.Preferences.User.Department.Code == "MEP")
                {
                    return Department.MEP;
                }
                return Department.MEP;
            }
        }
        public static string SubDepartment = "ОВ";
        public static bool PlaceOnArchitecturalWalls = true;
        public static bool PlaceOnStructuralWalls = true;
        public static double MinInstanceWidth = 10;
        public static double MinInstanceHeight = 10;
        public static double MinWallWidth = 80;
        public static double DefaultOffset = 25;
        public static void TryLoadParameters()
        {
            try
            {
                DirectoryInfo moduleLocation = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Local\KPLN_Loader"));
                if (Directory.Exists(moduleLocation.FullName))
                {
                    string path = Path.Combine(moduleLocation.FullName, "eomuser.txt");
                    if (File.Exists(path))
                    {
                        using (StreamReader sr = File.OpenText(path))
                        {
                            string value = sr.ReadToEnd();
                            string[] parts = value.Split(new string[] { Variables.separator_element }, StringSplitOptions.None);
                            SubDepartment = parts[0];
                            PlaceOnArchitecturalWalls = bool.Parse(parts[1]);
                            PlaceOnStructuralWalls = bool.Parse(parts[2]);
                            MinInstanceWidth = double.Parse(parts[3], System.Globalization.NumberStyles.Float);
                            MinInstanceHeight = double.Parse(parts[4], System.Globalization.NumberStyles.Float);
                            MinWallWidth = double.Parse(parts[5], System.Globalization.NumberStyles.Float);
                            DefaultOffset = double.Parse(parts[6], System.Globalization.NumberStyles.Float);
                        }

                    }
                }
            }
            catch (Exception)
            {
                SubDepartment = "ОВ";
                PlaceOnArchitecturalWalls = true;
                PlaceOnStructuralWalls = true;
                MinInstanceWidth = 10;
                MinInstanceHeight = 10;
                MinWallWidth = 80;
                DefaultOffset = 25;
            }

        }
        public static void TrySaveParameters()
        {
            try
            {
                List<string> parts = new List<string>();
                parts.Add(SubDepartment);
                parts.Add(PlaceOnArchitecturalWalls.ToString());
                parts.Add(PlaceOnStructuralWalls.ToString());
                parts.Add(MinInstanceWidth.ToString());
                parts.Add(MinInstanceHeight.ToString());
                parts.Add(MinWallWidth.ToString());
                parts.Add(DefaultOffset.ToString());
                string value = string.Join(Variables.separator_element, parts);
                DirectoryInfo moduleLocation = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @"AppData\Local\KPLN_Loader"));
                if (Directory.Exists(moduleLocation.FullName))
                {
                    string path = Path.Combine(moduleLocation.FullName, "eomuser.txt");
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.Write(value);
                        sw.Close();
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
