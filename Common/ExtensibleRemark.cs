using Autodesk.Revit.DB;
using ExtensibleOpeningManager.Commands;
using ExtensibleOpeningManager.Common.ExtensibleSubElements;
using ExtensibleOpeningManager.Extensible;
using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ExtensibleOpeningManager.Common.Collections;
using static KPLN_Loader.Output.Output;

namespace ExtensibleOpeningManager.Common
{
    public class ExtensibleRemark : ExtensibleMessage
    {
        private ExtensibleElement Parent { get; }
        public string Header { get; }
        public string Message { get; }
        public string User { get; }
        public string _GUID_THIS_INSTANCE { get; }
        public string _GUID_REQUEST_INSTANCE { get; }
        public string _GUID_HOST { get; }
        public override DateTime Time { get; }
        public Department Department { get; }
        public RemarkType Type { get; }
        public ExtensibleRemark Request { get; set; }
        public ExtensibleRemark(string header, string message, ExtensibleElement parent, ExtensibleRemark hostRemark, RemarkType type)
        {
            _GUID_THIS_INSTANCE = Guid.NewGuid().ToString();
            _GUID_REQUEST_INSTANCE = hostRemark._GUID_THIS_INSTANCE;
            _GUID_HOST = hostRemark._GUID_HOST;
            Type = type;
            Header = header;
            Message = message;
            User = KPLN_Loader.Preferences.User.SystemName;
            Time = DateTime.Now;
            Department = UserPreferences.Department;
            Parent = parent;
        }
        public ExtensibleRemark(string header, string message, ExtensibleElement parent, SE_LinkedInstance hostElement)
        {
            _GUID_THIS_INSTANCE = Guid.NewGuid().ToString();
            _GUID_REQUEST_INSTANCE = "None";
            _GUID_HOST = ExtensibleController.Read(hostElement.Element as FamilyInstance, ExtensibleParameter.Document);
            Type = RemarkType.Request;
            Header = header;
            Message = message;
            User = KPLN_Loader.Preferences.User.SystemName;
            Time = DateTime.Now;
            Department = UserPreferences.Department;
            Parent = parent;
        }
        private ExtensibleRemark(string linkguid, string guid, string header, string message, string user, string host, DateTime time, Department department, RemarkType type, ExtensibleElement parent)
        {
            _GUID_THIS_INSTANCE = guid;
            _GUID_REQUEST_INSTANCE = linkguid;
            _GUID_HOST = host;
            Type = type;
            Header = header;
            Message = message;
            User = user;
            Time = time;
            Department = department;
            Parent = parent;
        }
        private ExtensibleRemark(string linkguid, string guid, string header, string message, string user, string host, DateTime time, Department department, RemarkType type)
        {
            _GUID_THIS_INSTANCE = guid;
            _GUID_REQUEST_INSTANCE = linkguid;
            _GUID_HOST = host;
            Type = type;
            Header = header;
            Message = message;
            User = user;
            Time = time;
            Department = department;
            Parent = null;
        }

        public override UIElement GetUiElement()
        {
            string departmentText = string.Empty;
            SQLUserInfo user = KPLN_Loader.Preferences.SQLiteDataBase.GetUser(User);
            SolidColorBrush bgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 200, 200, 200));
            SolidColorBrush fgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 115, 115, 115));
            switch (Department)
            {
                case Department.AR:
                    departmentText = "АР";
                    bgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 185, 230, 255));
                    fgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 200));
                    break;
                case Department.KR:
                    departmentText = "КР";
                    bgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 150, 150));
                    fgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 200, 0, 0));
                    break;
                case Department.MEP:
                    departmentText = "ИС";
                    bgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 160, 255, 180));
                    fgColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 200, 0));
                    break;
            }
            //Element
            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
            if (user.SystemName == KPLN_Loader.Preferences.User.SystemName && UserPreferences.Department == Department)
            {
                grid.Margin = new Thickness() { Left = 0, Top = 0, Right = 35, Bottom = 5 };
            }
            else
            {
                grid.Margin = new Thickness() { Left = 35, Top = 0, Right = 0, Bottom = 5 };
            }
            StackPanel sp_major = new StackPanel() { Orientation = Orientation.Vertical };
            //Background
            System.Windows.Shapes.Rectangle rec_bg = new System.Windows.Shapes.Rectangle() { RadiusX = 5, RadiusY = 5 };
            if (Type != RemarkType.Request) { rec_bg.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 194, 171)); }
            else { rec_bg.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 70, 0)); }
            grid.Children.Add(rec_bg);
            //Head
            System.Windows.Controls.Grid grid_head = new System.Windows.Controls.Grid();
            sp_major.Children.Add(grid_head);
            grid_head.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Pixel)});
            grid_head.ColumnDefinitions.Add(new ColumnDefinition());
            System.Windows.Shapes.Rectangle rec_icon = new System.Windows.Shapes.Rectangle() { Fill = Brushes.White, RadiusX = 3, RadiusY = 3, Margin = new Thickness() { Left = 3, Top = 3, Right = 3, Bottom = 3 } };
            System.Windows.Controls.Grid.SetColumn(rec_icon, 0);
            grid_head.Children.Add(rec_icon);
            TextBlock tb_icon = new TextBlock() { Text = "!?", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 25, Margin = new Thickness() { Left = 2, Top = 2, Right = 3, Bottom = 2 }, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 193, 0)) };
            if (Type == RemarkType.Answer_Ok)
            {
                tb_icon.Text = "✓";
                tb_icon.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 22, 140, 53));
            }
            if (Type == RemarkType.Answer_No)
            {
                tb_icon.Text = "✘";
                tb_icon.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 243, 30, 26));
            }
            System.Windows.Controls.Grid.SetColumn(tb_icon, 0);
            grid_head.Children.Add(tb_icon);
            StackPanel sp_head = new StackPanel() { Orientation = Orientation.Vertical };
            System.Windows.Controls.Grid.SetColumn(sp_head, 1);
            grid_head.Children.Add(sp_head);
            if (Request != null)
            {
                string reqDep = "";
                switch (Request.Department)
                {
                    case Department.AR:
                        reqDep = "АР";
                        break;
                    case Department.KR:
                        reqDep = "КР";
                        break;
                    case Department.MEP:
                        reqDep = "ИС";
                        break;
                }
                TextBlock tb_req_date = new TextBlock() { Foreground = Brushes.White, Text = Request.Time.ToString("G") };
                StackPanel sp_req = new StackPanel() { Orientation = Orientation.Horizontal };
                TextBlock tb_req_from = new TextBlock() { Foreground = Brushes.White, Text = "от:", VerticalAlignment = VerticalAlignment.Bottom, FontSize = 10, Margin = new Thickness() { Left = 0, Top = 0, Right = 1, Bottom = 0 } };
                TextBlock tb_req_user = new TextBlock() { Foreground = Brushes.White, Text = GetUserNameBySystemName(Request.User) };
                TextBlock tb_rew_department = new TextBlock() { Foreground = Brushes.White, Text = reqDep, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Top, FontSize = 8, Margin = new Thickness() { Left = 2, Top = 0, Right = 0, Bottom = 0 } };
                sp_req.Children.Add(tb_req_from);
                sp_req.Children.Add(tb_req_user);
                sp_req.Children.Add(tb_rew_department);
                Separator sep_head = new Separator() { Background = Brushes.White, Foreground = Brushes.White, Margin = new Thickness() { Left = 0, Top = 0, Right = 5, Bottom = 0 } };
                sp_head.Children.Add(tb_req_date);
                sp_head.Children.Add(sp_req);
                sp_head.Children.Add(sep_head);
            }
            TextBlock tb_local_date = new TextBlock() { Foreground = Brushes.White, Text = Time.ToString("G") };
            StackPanel sp_local = new StackPanel() { Orientation = Orientation.Horizontal };
            TextBlock tb_local_from = new TextBlock() { Foreground = Brushes.White, Text = "от:", VerticalAlignment = VerticalAlignment.Bottom, FontSize = 10, Margin = new Thickness() { Left = 0, Top = 0, Right = 1, Bottom = 0 } };
            TextBlock tb_local_user = new TextBlock() { Foreground = Brushes.White, Text = GetUserNameBySystemName(User) };
            TextBlock tb_local_department = new TextBlock() { Foreground = Brushes.White, Text = departmentText, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Top, FontSize = 8, Margin = new Thickness() { Left = 2, Top = 0, Right = 0, Bottom = 0 } };
            sp_local.Children.Add(tb_local_from);
            sp_local.Children.Add(tb_local_user);
            sp_local.Children.Add(tb_local_department);
            sp_head.Children.Add(tb_local_date);
            sp_head.Children.Add(sp_local);
            //Body
            sp_major.Children.Add(new Separator() { Background = Brushes.White, Foreground = Brushes.White, Margin = new Thickness() { Left = 0, Top = 2, Right = 0, Bottom = 2 } });
            string symbol = "";
            if (Type == RemarkType.Answer_Ok) { symbol = "✓ "; }
            if (Type == RemarkType.Answer_No) { symbol = "✘ "; }
            if (Type == RemarkType.Request) { symbol = "✉ "; }
            if (Request != null)
            {
                TextBlock tb_req_user = new TextBlock() { FontSize = 14, FontWeight = FontWeights.Bold, Foreground = Brushes.White, Text = string.Format("✉ {0}", Request.Header), TextWrapping = TextWrapping.Wrap, Margin = new Thickness() { Left = 5, Top = 0, Right = 5, Bottom = 0 } };
                TextBlock tb_req_message = new TextBlock() { Foreground = Brushes.White, Text = Request.Message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness() { Left = 5, Top = 0, Right = 5, Bottom = 0 } };
                sp_major.Children.Add(tb_req_user);
                sp_major.Children.Add(tb_req_message);
            }
            TextBlock tb_local_header = new TextBlock() { FontSize = 14, FontWeight = FontWeights.Bold, Foreground = Brushes.White, Text = string.Format("{0}{1}", symbol, Header), TextWrapping = TextWrapping.Wrap, Margin = new Thickness() { Left = 5, Top = 0, Right = 5, Bottom = 0 } };
            TextBlock tb_local_massage = new TextBlock() { Foreground = Brushes.White, Text = Message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness() { Left = 5, Top = 0, Right = 5, Bottom = 0 } };
            sp_major.Children.Add(tb_local_header);
            sp_major.Children.Add(tb_local_massage);
            //Buttons
            StackPanel sp_buttons = new StackPanel() { Orientation = Orientation.Horizontal };
            if (Type == RemarkType.Request && Parent != null)
            {
                if (Department == UserPreferences.Department)
                {
                    Button btn_remove = new Button() { Content = "❌", Width = 20, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness() { Left = 5, Top = 0, Right = 0, Bottom = 5 } };
                    sp_buttons.Children.Add(btn_remove);
                    btn_remove.Click += OnRemove;
                }
                else
                {
                    Button btn_approve = new Button() { Content = "Сделано", HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness() { Left = 5, Top = 0, Right = 0, Bottom = 5 } };
                    sp_buttons.Children.Add(btn_approve);
                    btn_approve.Click += OnApply;
                    Button btn_reject = new Button() { Content = "Отклонить", HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness() { Left = 5, Top = 0, Right = 0, Bottom = 5 } };
                    sp_buttons.Children.Add(btn_reject);
                    btn_reject.Click += OnReject;
                }
            }
            sp_major.Children.Add(sp_buttons);
            grid.Children.Add(sp_major);
            return grid;

        }
        private void OnApply(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                ModuleData.CommandQueue.Enqueue(new CommandApproveRemark(Parent, this));
            }
        }
        private void OnReject(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                ModuleData.CommandQueue.Enqueue(new CommandRejectRemark(Parent, this));
            }
        }
        private void OnRemove(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.RemoveRemark(this);
            }
        }
        public override string ToString()
        {
            return string.Join(Variables.separator_sub_element, new string[] { Type.ToString(), _GUID_HOST, Header, Message, User, Time.ToString(), Department.ToString(), _GUID_THIS_INSTANCE, _GUID_REQUEST_INSTANCE });
        }
        public static List<ExtensibleMessage> TryParseCollection(string value, ExtensibleElement parent)
        {
            List<ExtensibleMessage> comments = new List<ExtensibleMessage>();
            foreach (string commentString in value.Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string[] parts = commentString.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 9)
                    {
                        DateTime time = DateTime.Parse(parts[5]);
                        Department department;
                        Enum.TryParse(parts[6], out department);
                        RemarkType type;
                        Enum.TryParse(parts[0], out type);
                        comments.Add(new ExtensibleRemark(parts[8], parts[7], parts[2], parts[3], parts[4], parts[1], time, department, type, parent));
                    }
                }
                catch (Exception e)
                {
                    PrintError(e);
                }
            }
            return comments;
        }
        public static List<ExtensibleMessage> TryParseCollection(string value)
        {
            List<ExtensibleMessage> comments = new List<ExtensibleMessage>();
            foreach (string commentString in value.Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string[] parts = commentString.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 9)
                    {
                        DateTime time = DateTime.Parse(parts[5]);
                        Department department;
                        Enum.TryParse(parts[6], out department);
                        RemarkType type;
                        Enum.TryParse(parts[0], out type);
                        comments.Add(new ExtensibleRemark(parts[8], parts[7], parts[2], parts[3], parts[4], parts[1], time, department, type));
                    }
                }
                catch (Exception e)
                {
                    PrintError(e);
                }
            }
            return comments;
        }
    }
}
