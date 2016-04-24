using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace iFlagUpdater
{
    public partial class mainForm : Form
    {
        string appFilename = "iFlag.exe";
        int[] windowLocation = { 0, 0 };
        string version = "";
        string updateVersion = "";

        string updateRepository = "https://github.com/simracer-cz/iFlag";
        string updateRepositoryFormat = "{0}/blob/{1}/software/iFlag/{2}?raw=true";
        string updateDownloadFormat = "{0}.update";
        string[] updateFiles = {
          "iFlag.exe",
          "iFlag.exe.config",
          "iFlag.pdb",
          "device/firmware.hex",
          // "iRSDKSharp.dll",
        };
        int updatedCount;

        public mainForm(string[] args)
        {
            if (args.Length != 4)
            {
                Application.Exit();
                return;
            }

            version = args[0];
            updateVersion = args[1];
            windowLocation[0] = Convert.ToInt32(args[2]);
            windowLocation[1] = Convert.ToInt32(args[3]);

            InitializeComponent();
            performUpdate();
        }

                                                  // When the window opens,
                                                  // set its location the same as caller's app
        private void mainForm_Load(object sender, EventArgs e)
        {
            System.Drawing.Point place = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Location;
            place.X += windowLocation[0];
            place.Y += windowLocation[1];
            this.Location = place;

            beforeLabel.Text = version;
            afterLabel.Text = updateVersion;
        }

                                                  // Gets called repeatedly for each update files list entry.
                                                  // It downloads new versions of files and stores then
                                                  // as *.update files while keeping count.
                                                  // When all downloading is complete, the intermittent
                                                  // *.update files overwrite the main app ones.
        private void performUpdate(){
            if (updatedCount < updateFiles.Length)
            {
                try
                {
                    string filename = updateFiles[updatedCount];
                    updatedCount++;
                    string url = string.Format(updateRepositoryFormat, updateRepository, updateVersion, filename);
                    string file = string.Format(updateDownloadFormat, filename);

                    WebClient webClient = new WebClient();
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadComplete);
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(progressChanged);
                    webClient.DownloadFileAsync(new Uri(url), @file);
                }
                catch (Exception)
                {
                    MessageBox.Show("Sorry, update download failed.");
                    returnToAppNow();
                }
            }
            else
            {
                try
                {
                    int progressAt = progressBar.Value;
                    int progressStep = (100 - progressAt) / updateFiles.Length;
                    for (int i = 0; i < updateFiles.Length; i++)
                    {
                        string filename = updateFiles[i];
                        FileInfo file = new FileInfo(string.Format(updateDownloadFormat, filename));
                        file.CopyTo(filename, true);
                        file.Delete();
                        progressBar.Value = progressAt + (i + 1) * progressStep;
                    }
                    progressBar.Value = 100;
                    returnToApp();
                }
                catch (Exception)
                {
                    MessageBox.Show("Sorry, update of local files failed.");
                    returnToAppNow();
                }
            }
        }

        private void downloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            performUpdate();
        }

        private void progressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int steps = updateFiles.Length + 1;
            int segment = 100 / steps;
            int percentage = updatedCount * segment + e.ProgressPercentage / steps;

            progressBar.Value = percentage;
        }

        private void returnToApp()
        {
            finishTimer.Start();
        }

        private void returnToAppNow()
        {
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = appFilename;
            process.StartInfo = info;
            process.Start();
            Application.Exit();
        }

        private void finishTimer_Tick(object sender, EventArgs e)
        {
            returnToAppNow();
        }
    }
}
