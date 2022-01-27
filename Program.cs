using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.IO.Compression;

namespace UNTONE.Microstaller
{
    public static class Info
    {
        public static string Company = "untone";
        public static string CompanyName = "UNTONE";
        public static string App = "appname";
        public static string AppName = "appname";
        public static string Endpoint = "https://yourendpoint.untone/" + App + "/app.ep"; // should be a plaintext file with a URL to download
        public static string InstallationLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + Company + "/" + App + "/";
        public static bool StartOnFinish = true;
        public static string StartApp = InstallationLocation + "app.exe";
    }

    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
           

        }

        static void Main(string[] args)
        {
            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                var handle = GetConsoleWindow();

                string source = "";

                ShowWindow(handle, SW_HIDE);

                try
                {
                    WebRequest req = HttpWebRequest.Create(Info.Endpoint);
                    req.Method = "GET";

                    using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                    {
                        source = reader.ReadToEnd();
                    }
                }
                catch
                {

                }

                if(source.Contains("https://"))
                {
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                        string location = "temp_" + Info.App + ".zip";
                        wc.DownloadFile(
                            new System.Uri(source),
                            location
                        );
                        if(System.IO.Directory.Exists(Info.InstallationLocation))
                        {
                            Directory.CreateDirectory(Info.InstallationLocation);
                        }
                        ZipFile.ExtractToDirectory(location, Info.InstallationLocation);
                        System.IO.File.Delete(location);
                        //MessageBox(handle, "Done!", "Microstaller by UNTONE [installing " + Info.App + "]", 0);
                        if (Info.StartOnFinish == true)
                        {
                            System.Diagnostics.Process.Start(Info.StartApp);
                        }
                    }
                }
                else
                {
                    MessageBox(handle, "Could not contact installation server.", "Microstaller by UNTONE [installing " + Info.App + "]", 0);
                }
            }
        }
    }
}
