/*
    Copyright 2012, Grant Hess

    This file is part of S3ToolKit.Utils.

    S3ToolKit.Utils is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with CC Magic.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace S3ToolKit.Utils.Registry
{
    /// <summary>
    /// Encapsulates the registry information for a single game entity 
    /// (i.e. Base Game, Expansion Pack, Stuff Pack, or tool)
    /// </summary>
    // Information stored (from Registry):
    // Country  REG_SZ              (BG,   ,WA,HEL,         )
    // DisplayName  REG_SZ          (BG,CAW,WA,HEL,         )
    // ExePath      REG_SZ          (  ,   ,WA,HEL,         )
    // Install Dir  REG_SZ          (BG,CAW,WA,HEL,         )
    // InstallStart REG_DWORD       (BG,CAW,WA,HEL,         )
    // Locale       REG_SZ          (BG,CAW,WA,HEL,         )
    // ProductID    REG_DWORD       (  ,   ,WA,HEL,         )
    // SKU          REG_DWORD       (BG,   ,WA,HEL,         )
    // Telemetery   REG_DWORD       (BG,   ,WA,HEL,         )
    //
    // Looks like SKU is 7 for The Sims 3???
    // EP,SP determined by ProductID???
    public class InstalledGameEntry
    {
        private static readonly Logging.ILog log = Logging.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        public string RegistryName { get; private set; }
        public string Country { get; private set; }
        public string DisplayName { get; private set; }
        public string ExePath { get; private set; }
        public string InstallDir { get; private set; }
        public Int32 InstallStart { get; private set; }
        public string Locale { get; private set; }
        public Int32 ProductID { get; private set; }
        public Int32 SKU { get; private set; }
        public Int32 Telemetery { get; private set; }

        public bool IsTool { get { return GetIsTool(); } }
        public bool IsGame { get { return GetIsGame(); } }
        public bool IsBaseGame { get { return GetIsBaseGame(); } }

        public InstalledGameEntry()
        {
            this.RegistryName = "The Sims 3";

            this.Country = "US";
            this.DisplayName = "The Sims 3";
            this.ExePath = @"C:\Program Files (x86)\Origin Games\The Sims 3\Game\Bin\TS3.exe";
            this.InstallDir = @"C:\Program Files (x86)\Origin Games\The Sims 3";
            this.Locale = "en-US";

            this.InstallStart = 0;
            this.ProductID = 0x3e9;
            this.SKU = 7;
            this.Telemetery = 0;
        }

        public InstalledGameEntry(RegistryKey Key, string KeyName)
        {
            log.Debug("InstalledGameEntry .ctor");
            RegistryKey SubKey = Key.OpenSubKey(KeyName);
            try
            {
                this.RegistryName = KeyName;

                this.Country = (string)SubKey.GetValue("Country", "");
                this.DisplayName = (string)SubKey.GetValue("DisplayName", "");
                this.ExePath = (string)SubKey.GetValue("ExePath", "");
                this.InstallDir = (string)SubKey.GetValue("Install Dir");
                this.Locale = (string)SubKey.GetValue("Locale", "");

                this.InstallStart = (Int32)(SubKey.GetValue("InstallStart", 0));
                this.ProductID = (Int32)(SubKey.GetValue("ProductID", 0));
                this.SKU = (Int32)(SubKey.GetValue("SKU", 0));
                this.Telemetery = (Int32)(SubKey.GetValue("Telemetry", 0));

                if (!this.RegistryName.StartsWith("The Sims 3 Create a"))
                {
                    if ((this.ProductID == 0) | (this.ProductID == 0x3e8) | (this.ProductID == 0x3e9))
                    {
                        this.ProductID = 1;
                    }
                }
            }
            finally
            {
                SubKey.Close();
            }
            log.Info(string.Format("Loaded Game Pack {0}", this.DisplayName));
        }

        internal bool GetIsTool()
        {
            return (SKU == 0);
        }

        internal bool GetIsGame()
        {
            log.Info("GetIsGame()"); 
            if (RegistryName.StartsWith("The Sims 3 Create a"))
                return false;

            //if (SKU != 7)
            //    log.Warn(string.Format("Unknown SKU entry: {0}", SKU));
            return (RegistryName.StartsWith("The Sims 3"));
        }

        internal bool GetIsBaseGame()
        {
            return (IsGame & ((ProductID == 0) | (ProductID == 1) | (ProductID == 1000) | (ProductID == 1001)));
        }
    }
}
