using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO.Compression;
using SCREW_INSTALLER.Installer;
using System.Diagnostics;
using SCREW.Installer;

namespace SCREW_INSTALLER
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = $"{App.appName}";
            ShowLoader();
            this.Loaded += OnWindowLoaded;
        }

        public async void ShowLoader()
        {
            MainPanel.Visibility = Visibility.Hidden;
            LoaderPanel.Visibility = Visibility.Visible;
            await Task.Delay(500);
            StartMainLogoAnim();
            StartScrewLogoAnim();
            StartInstallerLogoAnim();
        }

        public async void StartMainLogoAnim()
        {
            await AnimController.FadeIn(LoaderMainLogo);
            await Task.Delay(2600);
            await AnimController.FadeOut(LoaderMainLogo);
            await Task.Delay(200);
            ShowMain();
        }

        public async void StartScrewLogoAnim()
        {
            await AnimController.FadeIn(ScrewLogo);
            await Task.Delay(800);
            await AnimController.FadeOut(ScrewLogo);
        }

        public async void StartInstallerLogoAnim()
        {
            await Task.Delay(1800);
            await AnimController.FadeIn(InstallerLogo);
            await Task.Delay(800);
            await AnimController.FadeOut(InstallerLogo);
        }

        public void ShowMain()
        {
            LoaderPanel.Visibility = Visibility.Hidden;
            MainPanel.Visibility = Visibility.Visible;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            ShowInstall();
            PublisherLabel.Content = $"Publisher: {App.companyName}";
            VersionLabel.Content = $"Version: {App.installer.GetVersion()}";
            await LoadImageAsync();
        }

        public void ShowOpen()
        {
            ProgressSelection.Visibility = Visibility.Hidden;
            InstallSelection.Visibility = Visibility.Hidden;
            OpenSelection.Visibility = Visibility.Visible;
            InstallLabel.Content = $"{App.appName} installed!";
        }

        public void ShowInstall()
        {
            ProgressSelection.Visibility = Visibility.Hidden;
            OpenSelection.Visibility = Visibility.Hidden;
            InstallSelection.Visibility = Visibility.Visible;
            InstallLabel.Content = $"Install {App.appName}?";
        }

        public void ShowProgress()
        {
            OpenSelection.Visibility = Visibility.Hidden;
            InstallSelection.Visibility = Visibility.Hidden;
            ProgressSelection.Visibility = Visibility.Visible;
            InstallLabel.Content = $"Installing {App.appName}...";
        }

        private Task LoadImageAsync()
        {
            var fullFilePath = App.appLogo;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            bitmap.DownloadCompleted += (sender, args) =>
            {
                tcs.SetResult(true);
            };

            AppLogo.Source = bitmap;

            return tcs.Task;
        }

        private void OnInstallClicked(object sender, RoutedEventArgs e)
        {
            Download();
        }

        private async Task Download()
        {
            ShowProgress();

            string directoryPath = System.IO.Path.Combine(App.installPath, App.workingDerictory);
            string zipName = GenerateRandomName();
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            else
            {
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(App.installer.GetUrl(), HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;
                    long totalRead = 0;

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream($"{zipName}.zip", FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[8192];
                        int readBytes;

                        while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, readBytes);

                            totalRead += readBytes;
                            if (totalBytes.HasValue)
                            {
                                double percentage = (double)totalRead / totalBytes.Value * 100;
                                Console.WriteLine($"Download Progress: {percentage:F2}%");
                                DownloadProgress.Value = percentage;
                            }
                        }
                    }

                    using (FileStream zipFile = new FileStream($"{zipName}.zip", FileMode.Open, FileAccess.Read))
                    {
                        await ExtractZipAsync(zipFile, directoryPath);
                    }

                    File.Delete($"{zipName}.zip");
                }
            }

            ShortcutCreator.CreateShortcut(System.IO.Path.Combine(directoryPath, App.runName));
            ShowOpen();
        }

        private void OnOpenAppClick(object sender, RoutedEventArgs e)
        {
            string directoryPath = System.IO.Path.Combine(App.installPath, App.workingDerictory);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = System.IO.Path.Combine(directoryPath, App.runName),
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            System.Environment.Exit(0);
        }

        private async Task ExtractZipAsync(Stream zipStream, string destinationDirectory)
        {
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
            {
                foreach (var entry in archive.Entries)
                {
                    string entryPath = System.IO.Path.Combine(destinationDirectory, entry.FullName);

                    if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                    {
                        Directory.CreateDirectory(entryPath);
                        continue;
                    }

                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(entryPath));

                    using (var entryStream = entry.Open())
                    using (var entryFile = File.Create(entryPath))
                    {
                        await entryStream.CopyToAsync(entryFile);
                    }
                }
            }
        }

        private void OnSelectFolderClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                App.installPath = folderBrowserDialog.SelectedPath;
            }
        }

        public string GenerateRandomName()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 12; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            string randomString = stringBuilder.ToString();
            return randomString;
        }
    }
}
