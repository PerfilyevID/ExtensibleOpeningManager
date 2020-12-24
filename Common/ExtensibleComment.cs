using KPLN_Loader.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static ExtensibleOpeningManager.Common.Collections;

namespace ExtensibleOpeningManager.Common
{
    public class ExtensibleComment : ExtensibleMessage
    {
        private ExtensibleElement Parent { get; }
        public string Message { get; }
        public string User { get; }
        public override DateTime Time { get; }
        public Department Department { get; }
        public ExtensibleComment(string message, ExtensibleElement parent)
        {
            Message = message;
            User = KPLN_Loader.Preferences.User.SystemName;
            Time = DateTime.Now;
            Department = UserPreferences.Department;
            Parent = parent;
        }
        private ExtensibleComment(string message, string user, DateTime time, Department department, ExtensibleElement parent)
        {
            Message = message;
            User = user;
            Time = time;
            Department = department;
            Parent = parent;
        }
        private ExtensibleComment(string message, string user, DateTime time, Department department)
        {
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
            SolidColorBrush bgColor = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            SolidColorBrush fgColor = new SolidColorBrush(Color.FromArgb(255, 115, 115, 115));
            switch (Department)
            {
                case Department.AR:
                    departmentText = "АР";
                    bgColor = new SolidColorBrush(Color.FromArgb(255, 185, 230, 255));
                    fgColor = new SolidColorBrush(Color.FromArgb(255, 0, 0, 200));
                    break;
                case Department.KR:
                    departmentText = "КР";
                    bgColor = new SolidColorBrush(Color.FromArgb(255, 255, 150, 150));
                    fgColor = new SolidColorBrush(Color.FromArgb(255, 200, 0, 0));
                    break;
                case Department.MEP:
                    departmentText = "ИС";
                    bgColor = new SolidColorBrush(Color.FromArgb(255, 160, 255, 180));
                    fgColor = new SolidColorBrush(Color.FromArgb(255, 0, 200, 0));
                    break;
            }
            Grid grid = new Grid();
            if (user.SystemName == KPLN_Loader.Preferences.User.SystemName && UserPreferences.Department == Department)
            {
                grid.Margin = new Thickness() { Left = 0, Top = 0, Right = 35, Bottom = 5 };
            }
            else
            {
                grid.Margin = new Thickness() { Left = 35, Top = 0, Right = 0, Bottom = 5 };
            }
            Rectangle rectangle = new Rectangle() { Fill = bgColor, RadiusX = 5, RadiusY = 5 };
            string visibleMessage = Message;
            if (Message == Variables.msg_created)
            { visibleMessage = "<Создан>"; }
            if (Message == Variables.msg_approved)
            { visibleMessage = "<Одобрен>"; }
            if (Message == Variables.msg_rejected)
            { visibleMessage = "<Отклонен>"; }
            if (Message == Variables.msg_autoJoined)
            { visibleMessage = "<Автопривязка>"; }
            StackPanel spControlls = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            StackPanel spBody = new StackPanel() { Orientation = Orientation.Vertical };
            StackPanel spUser = new StackPanel() { Orientation = Orientation.Horizontal };
            Button btnClose = new Button() { Content = "❌", Width = 14, Height = 14, VerticalAlignment = VerticalAlignment.Top, Background = null, FontSize = 6, BorderBrush = fgColor, Foreground = fgColor, Margin = new Thickness() { Left = 2, Top = 2, Right = 2, Bottom = 2 } };
            TextBlock tbDate = new TextBlock() { Text = Time.ToString("G"), Margin = new Thickness() { Left = 5, Top = 5, Right = 5, Bottom = 5 }, HorizontalAlignment=HorizontalAlignment.Left };
            TextBlock tbFrom = new TextBlock() { Text = "от:", VerticalAlignment = VerticalAlignment.Bottom, FontSize = 10, Margin = new Thickness() { Left = 5, Top = 0, Right = 1, Bottom = 0 } };
            TextBlock tbUser = new TextBlock() { Text = GetUserNameBySystemName(User) };
            TextBlock tbDepartment = new TextBlock() { Text = departmentText, HorizontalAlignment = HorizontalAlignment.Right, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Top, FontSize = 8, Foreground = fgColor, Margin = new Thickness() { Left = 2, Top = 0, Right = 0, Bottom = 0 } };
            TextBlock tbMessage = new TextBlock() { Text = visibleMessage, TextWrapping = TextWrapping.Wrap, Margin = new Thickness() { Left = 5, Top = 0, Right = 5, Bottom = 5 } };
            Separator sp = new Separator() { Background = Brushes.White, Foreground = Brushes.White, Margin = new Thickness() { Left = 0, Top = 5, Right = 0, Bottom = 2 } };
            spUser.Children.Add(tbFrom);
            spUser.Children.Add(tbUser);
            spUser.Children.Add(tbDepartment);
            spBody.Children.Add(tbDate);
            spBody.Children.Add(spUser);
            spBody.Children.Add(sp);
            spBody.Children.Add(tbMessage);
            if (UserPreferences.Department == Department && Parent != null && user.SystemName == KPLN_Loader.Preferences.User.SystemName && Message != Variables.msg_created && Message != Variables.msg_approved && Message != Variables.msg_rejected && Message != Variables.msg_autoJoined)
            { spControlls.Children.Add(btnClose); }
            grid.Children.Add(rectangle);
            grid.Children.Add(spControlls);
            grid.Children.Add(spBody);
            btnClose.Click += OnClick;
            return grid;
        }
        private void OnClick(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                Parent.RemoveComment(this);
            }
        }
        public override string ToString()
        {
            return string.Join(Variables.separator_sub_element, new string[] { Message, User, Time.ToString(), Department.ToString() });
        }
        public static List<ExtensibleMessage> TryParseCollection(string value, ExtensibleElement parent)
        {
            List<ExtensibleMessage> comments = new List<ExtensibleMessage>();
            foreach (string commentString in value.Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = commentString.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 4)
                {
                    DateTime time = DateTime.Parse(parts[2]);
                    Department department;
                    Enum.TryParse(parts[3], out department);
                    comments.Add(new ExtensibleComment(parts[0], parts[1], time, department, parent));
                }
            }
            return comments;
        }
        public static List<ExtensibleComment> TryParseCollection(string value)
        {
            List<ExtensibleComment> comments = new List<ExtensibleComment>();
            foreach (string commentString in value.Split(new string[] { Variables.separator_element }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = commentString.Split(new string[] { Variables.separator_sub_element }, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length == 4)
                {
                    DateTime time = DateTime.Parse(parts[2]);
                    Department department;
                    Enum.TryParse(parts[3], out department);
                    comments.Add(new ExtensibleComment(parts[0], parts[1], time, department));
                }
            }
            return comments;
        }
    }
}
