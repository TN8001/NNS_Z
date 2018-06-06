using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
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
        public static Duration Duration { get; } = TimeSpan.FromMinutes(1);

        public SettingsModel Settings { get; }

        private DispatcherTimer timer = new DispatcherTimer();
        private Storyboard storyboard;
        private string css => _css ?? (_css = ReadCss());
        private string _css;

        public MainWindow()
        {
            InitializeComponent();

            storyboard = Resources["MyStoryboard"] as Storyboard;
            Settings = SettingsHelper.LoadOrDefault<SettingsModel>(GetConfigPath());
            DataContext = this; //ViewModel省略

            var url = $"http://live.nicovideo.jp/search?keyword={Settings.SearchWord}";
            WebView.Navigate(url);

            timer.Interval = Duration.TimeSpan;
            timer.Tick += Timer_TickAsync;
        }

        private string GetConfigPath()
        {
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exeDir, ProductInfo.Name + ".config");
        }
        private string ReadCss()
        {
            try
            {
                var exeDir = AppDomain.CurrentDomain.BaseDirectory;
                return File.ReadAllText(Path.Combine(exeDir, "NicoNama.css"))?.Replace("\r\n", "");
            }
            catch { return ""; }
        }

        private async void Timer_TickAsync(object sender, EventArgs e)
        {
            Debug.WriteLine("Timer_Tick");

            await SubmitAsync();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine("Closing");

            SettingsHelper.Save(Settings, GetConfigPath());
        }

        private void WebView_NavigationStarting(object sender, WebViewControlNavigationStartingEventArgs e)
        {
            Debug.WriteLine("NavigationStarting");

            if(e.Uri.AbsolutePath == @"/search")
            {
                timer.Stop();
                storyboard.Stop();
            }
            else
            {
                Process.Start(e.Uri.ToString());
                e.Cancel = true;
            }
        }
        private void WebView_ContentLoading(object sender, WebViewControlContentLoadingEventArgs e)
        {
            Debug.WriteLine("ContentLoading");
        }
        private async void WebView_DOMContentLoaded(object sender, WebViewControlDOMContentLoadedEventArgs e)
        {
            Debug.WriteLine("DOMContentLoaded");

            await InsertCssAsync();
            await SortSelecterMoveAsync();
            Settings.SearchWord = await GetSearchWordAsync();
        }
        private void WebView_NavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
        {
            if(e.IsSuccess)
                Debug.WriteLine($"NavigationCompleted: {e.Uri.ToString()}");
            else
                Debug.WriteLine($"NavigationFailed: {e.Uri.ToString()} Status: {e.WebErrorStatus.ToString()}");

            timer.Start();
            storyboard.Begin();
        }

        private async Task SubmitAsync()
        {
            var js = @"document.getElementsByClassName('search-form-submit-button')[0].click();";
            await WebView.InvokeScriptAsync("eval", new string[] { js });
        }
        private async Task InsertCssAsync()
        {
            var js = $@"(function() {{
                            var node = document.createElement('style');
                            document.body.appendChild(node);
                            window.addStyleString = function(str) {{ node.innerHTML = str; }}
                        }}());
                        addStyleString('{css}');";

            await WebView.InvokeScriptAsync("eval", new string[] { js });
        }
        private async Task SortSelecterMoveAsync()
        {
            var js = $@"var target = document.getElementById('sortselect');
                        var container = document.getElementsByClassName('setting-option-aret')[0];
                        container.appendChild(target); ";
            await WebView.InvokeScriptAsync("eval", new string[] { js });
        }
        private async Task<string> GetSearchWordAsync()
        {
            var js = $@"document.getElementsByClassName('search-form-textbox')[0].value;";
            return await WebView.InvokeScriptAsync("eval", new string[] { js });
        }
        private async Task SetSearchWordAsync()
        {
            var js = $@"document.getElementsByClassName('search-form-textbox')[0].value = '{Settings.SearchWord}';";
            await WebView.InvokeScriptAsync("eval", new string[] { js });
        }
    }
}
