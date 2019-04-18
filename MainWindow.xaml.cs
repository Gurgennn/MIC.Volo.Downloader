using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Mic_Volo_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource src;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new WebClient())
            {
                TextOutput.Text = "";
                Download_Button.IsEnabled = false;
                TextOutput.Text = string.Empty;
                Uri adress;
                if (TextUrl.Text == string.Empty || !Uri.IsWellFormedUriString(TextUrl.Text, UriKind.RelativeOrAbsolute))
                {
                    MessageBox.Show("Enter valid url");
                    Download_Button.IsEnabled = true;
                    return;
                }
                try
                {
                    adress = new Uri(TextUrl.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Enter valid url");
                    Download_Button.IsEnabled = true;
                    return;
                }
                var dlg = new SaveFileDialog();
                dlg.FileName = adress.AbsoluteUri.Substring(TextUrl.Text.LastIndexOf('/') + 1);
                dlg.ShowDialog();

                src = new CancellationTokenSource();
                src.Token.Register(() =>
                {
                    client.CancelAsync();
                });
                try
                {
                    Cancel_Button.IsEnabled = true;
                    client.DownloadProgressChanged += (s, ev) =>
                    {
                        DownloadProgressBar.Value = ev.ProgressPercentage;
                    };
                    await client.DownloadFileTaskAsync(adress, dlg.FileName);
                }
                catch (WebException exception)
                {
                    TextOutput.Text = exception.Message;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    Download_Button.IsEnabled = true;
                    DownloadProgressBar.Value = 0;
                    if (src.IsCancellationRequested)
                    {
                        TextOutput.Text = "Download canceled";
                    }
                    else
                    {
                        TextOutput.Text = "Download completed!";
                    }
                    Cancel_Button.IsEnabled = false;
                }

            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
