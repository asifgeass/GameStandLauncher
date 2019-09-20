using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace GetWebShortCut
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            UpdateWebPage("www.codeproject.com/");          
            
        }

        private void UpdateWebPage(string url)
        {
            Uri uri = new Uri("http://" + url);

            webBrowser1.Url = uri;

            System.Threading.Thread.Sleep(500);

            webBrowser1.Update();

            textBoxURL.Text = uri.Host;
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string iconPath = "";

            Image img = null;
            Stream myStream = null;

            WebProxy proxy = new WebProxy("http://proxy:80/", true);
            proxy.Credentials = new NetworkCredential("userId", "password", "Domain");


            WebRequest requestImg = WebRequest.Create("http://" + e.Url.Host + "/favicon.ico");

            requestImg.Proxy = proxy;
            requestImg.Timeout = 10000;

            WebResponse response = requestImg.GetResponse();

            if (response.ContentLength > 0)
            {
                myStream = response.GetResponseStream();                
            }
            else
            {
                HtmlDocument doc = webBrowser1.Document;
                HtmlElementCollection collect = doc.GetElementsByTagName("link");

                foreach (HtmlElement element in collect)
                {
                    if (element.GetAttribute("rel") == "SHORTCUT ICON")
                        iconPath = element.GetAttribute("href");
                }

                this.Text = doc.Title;
               
                requestImg = WebRequest.Create("http://" + e.Url.Host + iconPath);
                
                response = requestImg.GetResponse();

                myStream = response.GetResponseStream();
            }

            img = Image.FromStream(myStream);
            this.pictureBox1.Image = img;

            webBrowser2.Url = requestImg.RequestUri;
            webBrowser2.Update();
        }

        private void buttonGetIcon_Click(object sender, EventArgs e)
        {
            UpdateWebPage(textBoxURL.Text);
        }
    }
}