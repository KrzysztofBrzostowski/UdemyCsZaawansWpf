using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UdemyWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string DownloadedString { get; set; }

        public event Action<string> StringDownloaded = (x) => { };
        public event Action<string, string> FileNameProvided = (x, y) => { };

        public MainWindow()
        {
            InitializeComponent();

            StringDownloaded += (x) => SetControlsStateAfterDownload();
            StringDownloaded += (x) => DownloadedString = x;
            StringDownloaded += (x) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DisplayText.Text = "Podaj nazwę pliku";

                    MessageBox.Show("Podaj nazwę pliku");
                });
            };

            FileNameProvided += SaveToFile;
            FileNameProvided += (x,y) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DisplayText.Text = "Zapisano plik";

                    MessageBox.Show("Podaj nazwę pliku");
                });

            };

            FileName.Visibility = Visibility.Hidden;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadedString != null)
            {
                FileNameProvided.Invoke(FileName.Text,  DownloadedString);
                return;
            }

            var currentUrl = WebsiteUrl.Text;

            await Task.Run(async () =>
            {

                var webClient = new WebClient();

                //var downloadedString = webClient.DownloadString(currentUrl);
                var downloadedString = await webClient.DownloadStringTaskAsync(currentUrl);

                StringDownloaded.Invoke(downloadedString);

                //w innych aplikacjach niz wpf:
                //SynchronizationContext.Current.Post();
            });




            //var doSomeWorkTask = DoSomework();
            //doSomeWorkTask.Wait();
            //var resultInt = doSomeWorkTask.Result;
        }

        //private async Task<int> DoSomework()
        //{
        //    await Task.Delay(4000);

        //    return 2;
        //}

        private void SetControlsStateAfterDownload()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WebsiteUrl.Visibility = Visibility.Hidden;
                FileName.Visibility = Visibility.Visible;

                SubmitButton.Content = "Click to save";
            });
        }

        private void SaveToFile(string fileName, string downloadedString)
        {

            File.WriteAllText(fileName, downloadedString);
        }

    }
}
