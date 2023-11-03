using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AIS_lab_5
{

    public class GroupsRoot
    {
        public GroupsInfo response { get; set; }
    }
    public class GroupsInfo
    {
        public List<Group> items { get; set; }
    }

    public class Contacts
    {
        public int user_id { get; set; }
        public string desc { get; set; }
    }

    public class Group
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Contacts> contacts { get; set; }
    }

    public class UsersRoot
    {
        public List<User> response { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set;}
    }

    /// <summary>
    /// Логика взаимодействия для RequestWindow.xaml
    /// </summary>
    public partial class RequestWindow : Window
    {
        private MainWindow mw;
        public bool IsEnabledButton;
        List<Group> groups { get; set; }
        public RequestWindow(MainWindow MW)
        {
            InitializeComponent();
            mw = MW;
            IsEnabledButton = false;
        }

        private string GroupInfoStrBuilder(Group group)
        {
            string info = group.name + ":\nID группы: " + group.id + "\n" + "Количество указанных контактов: " + group.contacts.Count();
            return info;
        }

        private void Button_Click_Groups(object sender, RoutedEventArgs e)
        {
            string requestStringBlank = "https://api.vk.com/method/{0}?access_token={1}&v=5.154&extended=1&filter=moder&fields=contacts&user_id=" + mw.UserID;
            string requestAnswer = mw.GET(requestStringBlank, "groups.get", mw.Access_token);
            groups = JsonSerializer.Deserialize<GroupsRoot>(requestAnswer).response.items;

            List<string> listToPrint = new();
            foreach (Group group in groups) { listToPrint.Add(GroupInfoStrBuilder(group)); }
            RequestInfoTextBox.Text = string.Join("\n", listToPrint);

            IsEnabledButton = true;
        }

        private void Button_Click_Contacts(object sender, RoutedEventArgs e)
        {
            List<string> UsersIDs = GetAllIDs();
            string requestStringBlank = "https://api.vk.com/method/{0}?access_token={1}&v=5.154&user_ids=" + string.Join(",", UsersIDs);
            string method = "users.get";
            string requestAnswer = mw.GET(requestStringBlank, method, mw.Access_token);
            List<User> users = JsonSerializer.Deserialize<UsersRoot>(requestAnswer).response;

            RequestInfoTextBox.Text = UsersStringBuilder(users);
        }

        private string UsersStringBuilder(List<User> users)
        {
            string answ = "-----Контакты в группах-----\n";
            foreach (Group group in groups)
            {
                answ += group.name + ":\n";
                User temp_user;
                foreach(Contacts contact in group.contacts)
                {
                    temp_user = users.Select(x => x).Where(x => contact.user_id == x.id).FirstOrDefault();
                    answ += "   " + temp_user.first_name + " " + temp_user.last_name + "\n";
                }
                
            }
            return answ;
        }

        private List<string> GetAllIDs()
        {
            var list = new List<string>();
            foreach (Group group in groups)
            {
                foreach (Contacts user in group.contacts)
                {
                    list.Add(user.user_id.ToString());
                }
                
            }
            return list;
        }
    }
}
