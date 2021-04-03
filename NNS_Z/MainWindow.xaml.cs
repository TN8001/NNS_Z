using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace NNS_Z
{
    public partial class MainWindow : Window
    {
        public static Duration Duration { get; } = TimeSpan.FromMinutes(3);

        public SettingsModel Settings { get; }

        private readonly DispatcherTimer timer = new();
        private readonly Storyboard storyboard;
        private string Css => _Css ??= ReadCss();
        private string _Css;


        public MainWindow()
        {
            InitializeComponent();
            Settings = SettingsHelper.LoadOrDefault<SettingsModel>(GetConfigPath());
            DataContext = Settings;

            storyboard = Resources["MyStoryboard"] as Storyboard;

            var url = $"https://live.nicovideo.jp/search?keyword={Settings.SearchWord}";
            webView.Source = new Uri(url);

            timer.Interval = Duration.TimeSpan;
            timer.Tick += Timer_Tick;

            _ = InitializeAsync();
        }


        private async Task InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
        }

        private async Task InsertCssAsync()
        {
            var js = $"var st=document.createElement('style');document.body.appendChild(st);st.innerHTML=`{Css}`;";
            await webView.CoreWebView2.ExecuteScriptAsync(js);
        }

        private async Task<string> GetSearchWordAsync()
        {
            var js = $@"document.getElementsByClassName('search-form-textbox')[0].value;";
            var r = await webView.CoreWebView2.ExecuteScriptAsync(js);
            return r.Trim('\"');
        }

        private async Task SubmitAsync()
        {
            var js = @"document.getElementsByClassName('search-form-submit-button')[0].click();";
            await webView.ExecuteScriptAsync(js);
        }


        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.WriteLine("WebView_NavigationStarting");

            if (e.Uri.StartsWith(@"https://live.nicovideo.jp/search"))
            {
                timer.Stop();
                storyboard.Stop();
            }
            else
            {
                var psi = new ProcessStartInfo(e.Uri) { UseShellExecute = true, };
                Process.Start(psi);
                e.Cancel = true;
            }
        }

        private void WebView_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
            => Debug.WriteLine("WebView_ContentLoading");

        private async void CoreWebView2_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            Debug.WriteLine("CoreWebView2_DOMContentLoaded");
            await InsertCssAsync();

            Settings.SearchWord = await GetSearchWordAsync();
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("WebView_NavigationCompleted");

            if (!e.IsSuccess)
                Debug.WriteLine($"NavigationFailed: Status[{e.WebErrorStatus}]");

            timer.Start();
            storyboard.Begin();
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Timer_Tick");

            _ = SubmitAsync();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("Closing");
            SettingsHelper.Save(Settings, GetConfigPath());
        }


        private static string GetConfigPath()
        {
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exeDir, ProductInfo.Name + ".config");
        }

        private static string ReadCss()
        {
            try
            {
                var exeDir = AppDomain.CurrentDomain.BaseDirectory;
                return File.ReadAllText(Path.Combine(exeDir, "NicoNama.css"))?.Replace("\r\n", "");
            }
            catch
            {
                return "";
            }
        }
    }
}
