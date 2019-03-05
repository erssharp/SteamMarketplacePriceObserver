using SteamMarketplacePriceObserver.Core;
using SteamMarketplacePriceObserver.Core.Steam;
using System;
using System.Windows;
using System.Media;
using Tulpep.NotificationWindow;
using System.Windows.Media;

namespace SteamMarketplacePriceObserver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParserWorker<string> parser;
        string previous = null;

        public MainWindow()
        {
            InitializeComponent();
            CreateBacground();
            ID.Text = "TARGET%20PRACTICE";
            AppID.Text = "578080";
            Dif.Text = "1";
            parser = new ParserWorker<string>(new SteamParser(), ID.Text);
            parser.OnCompleted += Parser_OnCompleted;
            parser.OnNewData += Parser_OnNewData;
        }

        private void CreateBacground()
        {
            LinearGradientBrush myLinearGradientBrush =  new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 0);
            myLinearGradientBrush.EndPoint = new Point(1, 1);
            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Black, 0.0));
            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.White, 1.0));
            Background = myLinearGradientBrush;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SteamSettings ss = new SteamSettings();
            ss.Prefix = ss.Prefix.Replace("{AppID}", AppID.Text);
            ss.Prefix = ss.Prefix.Replace("{ID}", ID.Text);
            parser.Settings = ss;
            parser.ID = ID.Text;
            parser.Start();
            start.IsEnabled = false;
        }

        private void Parser_OnNewData(object a1, string a2)
        {
            if (a2 == null)
                return;

            var aa = a2.ToCharArray();
            string res = null;
            for (int i = 32; i < aa.Length; i++)
            {
                if (aa[i] == 'p')
                    break;
                res += aa[i];
            }

            if (previous == null)
                previous = res;

            if ((float.Parse(res) - float.Parse(previous)) != 0)
            {
                Differ.Items.Add((float.Parse(res) - float.Parse(previous)).ToString());
                Differ.ScrollIntoView((float.Parse(res) - float.Parse(previous)).ToString());
            }

            if (Math.Abs(float.Parse(res) - float.Parse(previous)) > float.Parse(Dif.Text))
            {
                SystemSounds.Asterisk.Play();
                PopupNotifier popup = new PopupNotifier();
                popup.Image = Properties.Resources.icon;
                popup.BodyColor = System.Drawing.Color.Black;
                popup.ContentColor = System.Drawing.Color.White;
                popup.TitleColor = System.Drawing.Color.White;
                popup.TitleText = "Steam Marketplace Price Observer";
                popup.ContentText = "\n\nPrice changed: " + (float.Parse(res) - float.Parse(previous)).ToString();
                popup.Popup();
                previous = res;
            }
            Res.Items.Add(res);
            Res.ScrollIntoView(res);
        }

        private void Parser_OnCompleted(object obj)
        {
            MessageBox.Show("Done!");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            parser.Abort();
            start.IsEnabled = true;
        }
    }
}
