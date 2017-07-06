using Microsoft.Win32;
using mshtml;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace NNS_Z
{
    public partial class MainWindow : Window
    {
        public SettingsModel Setting { get; }

        private DispatcherTimer timer = new DispatcherTimer();
        private Storyboard storyboard;
        private string cssString;

        public MainWindow()
        {
            InitializeComponent();

            var serializer = new SerializeHelper<SettingsModel>();
            try { Setting = serializer.Load(); }
            catch(SerializationException) { Application.Current.MainWindow.Close(); }

            DataContext = this; //ViewModel省略

            Closing += (s, e) =>
            {
                DeleteRegistryKey();
                Setting.SearchWord = GetSearchWord();
                serializer.Save(Setting);
            };

            SetRegistryKey(); // IEをEdgeモードに
            JSErrorSuppression(); // IEのスクリプトエラーを抑制
            cssString = ReadCssFile("NicoNama.css");

            //var url = $"http://live.nicovideo.jp/search?keyword=一般&filter=+:official:"; // スクショ取る用
            var url = $"http://live.nicovideo.jp/search?keyword={Setting.SearchWord}";
            browser.Navigate(url);

            timer.Interval = TimeSpan.FromMinutes(5); //xaml側AnimationのDurationも一緒に変えること
            timer.Tick += Timer_Tick;
            timer.Start();

            storyboard = Resources["MyStoryboard"] as Storyboard;
            storyboard.Begin();
        }

        private string GetSearchWord()
        {
            var doc = browser.Document as IHTMLDocument3;
            var element = doc.getElementById("search_form_word");
            return element?.getAttribute("value") as string ?? "";
        }

        private void JSErrorSuppression()
        {
            // http://qiita.com/hbsnow/items/3b92775c75b8a6dc171f
            var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            var comObj = axIWebBrowser2.GetValue(browser, null);
            comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new object[] { true });
            comObj.GetType().InvokeMember("RegisterAsDropTarget", BindingFlags.SetProperty, null, comObj, new object[] { false, });
        }

        private string ReadCssFile(string name)
        {
            try
            {
                var exeDir = AppDomain.CurrentDomain.BaseDirectory;
                using(var reader = new StreamReader(Path.Combine(exeDir, name)))
                    return reader.ReadToEnd();
            }
            catch(FileNotFoundException) { }
            catch(IOException) { }
            return null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Timer_Tick");

            var doc = browser.Document as IHTMLDocument3;
            doc.getElementById("search_form_submit")?.click();
        }

        private void Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("Navigating:" + e.Uri);

            // 放送のリンクは規定ブラウザに飛ばす
            if(e.Uri.ToString().StartsWith("http://live.nicovideo.jp/searchresult"))
            {
                Process.Start(e.Uri.ToString());
                e.Cancel = true;
            }
        }

        private void Navigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("Navigated");

            //JSErrorSuppression();
            CssInjection();

            timer.Stop();
            timer.Start();
            storyboard.Seek(TimeSpan.Zero);
        }
        private void CssInjection()
        {
            var doc = browser.Document as HTMLDocument;
            var css = doc.createStyleSheet();
            css.cssText = cssString;
            // css.cssText = GetCss();
        }

        private void LoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("LoadCompleted");
        }


        private const string KEY = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
        private string processName = Process.GetCurrentProcess().ProcessName + ".exe";

        private void SetRegistryKey()
        {
            // http://blog.livedoor.jp/tkarasuma/archives/1036522520.html
            var key = Registry.CurrentUser.CreateSubKey(KEY);
            key.SetValue(processName, 11001, RegistryValueKind.DWord);
            key.Close();
        }
        private void DeleteRegistryKey()
        {
            try
            {
                var key = Registry.CurrentUser.CreateSubKey(KEY);
                key.DeleteValue(processName);
                key.Close();
            }
            catch(Exception) { }
        }
    }
}
