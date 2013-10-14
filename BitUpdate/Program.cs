using System;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace BitUpdate
{
    class Program
    {
        //You can add multiple URLS, split them with |

        private const string VERSION = "https://home.ayra.ch:4433/bitsign/bitmessage.txt|http://home.ayra.ch/bitsign/bitmessage.txt|https://bitmessage.org/w/index.php?title=Template:Bitmessage_Current_Version_Number&action=raw";
        private const string BITMSG = "https://home.ayra.ch:4433/bitsign/bitmessage.exe|http://home.ayra.ch/bitsign/bitmessage.exe|https://bitmessage.org/download/windows/Bitmessage.exe";

        private const string F_VERSION = "version.txt";
        private const string F_BITMESSAGE = "bitmessage.exe";

        static void Main(string[] args)
        {
            //if bitmessage does not exists, we download anyways,
            //no matter of hasUpdate result
            //hasUpdate is still called to write the version file.
            if (hasUpdate() || !File.Exists(F_BITMESSAGE))
            {
                DownloadUpdate();
            }
            Process.Start("bitmessage.exe");
        }

        static bool hasUpdate()
        {
            //If version file is not found, force update
            foreach (string s in VERSION.Split('|'))
            {
                Console.WriteLine("Downloading version info...");
                WebClient WC = new WebClient();
                try
                {
                    string v=WC.DownloadString(s).Trim();
                    string o = File.Exists(F_VERSION)?File.ReadAllText(F_VERSION).Trim():string.Empty;
                    WC.Dispose();

                    //Replace Version file
                    if (File.Exists(F_VERSION))
                    {
                        File.Delete(F_VERSION);
                    }
                    File.WriteAllText(F_VERSION, v);

                    //check if versions differ
                    Console.WriteLine("Version check OK");
                    return v != o;
                }
                catch
                {
                    Console.WriteLine("Version check failed");
                    WC.Dispose();
                    //Next URL
                }
            }
            Console.WriteLine("Check your internet connection. All URLs are unavailable");
            return false;
        }

        static void DownloadUpdate()
        {
            foreach (string s in BITMSG.Split('|'))
            {
                WebClient WC = new WebClient();
                try
                {
                    if (File.Exists(F_BITMESSAGE))
                    {
                        File.Delete(F_BITMESSAGE);
                    }
                    WC.DownloadFile(s,F_BITMESSAGE);
                    WC.Dispose();
                    return;
                }
                catch
                {
                    Console.WriteLine("Cannot download and replace bitmessage.exe!");
                    WC.Dispose();
                    //Next URL
                }
            }
            Console.WriteLine("Check your internet connection. All URLs are unavailable");
        }
    }
}
