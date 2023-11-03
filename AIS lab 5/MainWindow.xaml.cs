using System.Web;
using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;

namespace AIS_lab_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Access_token { get; set; }
        public string UserID { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void Auth_Click(object sender, RoutedEventArgs e) //открытие созданного приложения в VK
        {
            {
                string appId = new ConfigurationBuilder().AddUserSecrets<App>().Build().GetSection("appID").Value;
                var uriStr = @"https://oauth.vk.com/authorize?client_id=" + appId +
                    @"&scope=offline,friends&redirect_uri=https://oauth.vk.com/blank.html&display=page&v=5.6&response_type=token";
                Browser.AddressChanged += BrowserOnNavigated;
                Browser.Load(uriStr);
            }
        }
        private void BrowserOnNavigated(object sender, DependencyPropertyChangedEventArgs e) // получение токена и ID пользователя
        {
            var uri = new Uri((string)e.NewValue);
            if (uri.AbsoluteUri.Contains(@"oauth.vk.com/blank.html#"))
            {
                string url = uri.Fragment;
                url = url.Trim('#');
                Access_token = HttpUtility.ParseQueryString(url).Get("access_token");
                UserID = HttpUtility.ParseQueryString(url).Get("user_id");
                RequestWindow newWindow = new RequestWindow(this);
                newWindow.Show();
            }
        }
        public string GET(string Url, string Method, string Token) // httpclient
        {
            WebRequest req = WebRequest.Create(string.Format(Url, Method, Token));
            WebResponse resp = req.GetResponse();
            Stream stream = resp.GetResponseStream();
            StreamReader sr = new StreamReader(stream);
            string Out = sr.ReadToEnd();
            return Out;
        }
    }
}
