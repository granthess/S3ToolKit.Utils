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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using S3ToolKit.Utils.Logging;

namespace S3ToolKit.Utils.Registry
{

    /// <summary>
    /// Encapsulates the information about what game units are installed.  Include Base Game, Expansion Packs,
    /// Stuff Packs, and tools.
    /// </summary>
    public sealed class InstallationInfo
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        #region Singleton
        private static readonly Lazy<InstallationInfo> _instance = new Lazy<InstallationInfo>(() => new InstallationInfo());

        public static InstallationInfo Instance { get { return _instance.Value; } }
        #endregion

        public List<InstalledGameEntry> Packs { get; private set; }
        public InstalledGameEntry BaseGame { get; private set; }
        public InstalledGameEntry NewestGame { get; private set; }
        public string DocumentBaseDir { get; private set; }        
        public string Locale { get; private set; }
        public bool IsSteam { get; private set; }

        private InstallationInfo()
        {
            log.Debug("InstallationInfo .ctor");

            log.Info(System.Environment.OSVersion.ToString());

            if (System.Environment.Is64BitOperatingSystem)
            {
                log.Info("64 Windows");
            }
            else
            {
                log.Info("32 Windows");
            }

            if (System.Environment.Is64BitProcess)
            {
                log.Info("64 Process");
            }
            else
            {
                log.Info("32 Process");
            }


            Packs = new List<InstalledGameEntry>();

            // Gather Registry Information
            LoadRegistry();
   
            // Initialize Properties
            InitEntryProperties();

                       

            if (BaseGame == null)
            {
                Locale = "en-US";
            }
            else
            {
                Locale = BaseGame.Locale;
            }

            DocumentBaseDir = InitBaseDir(Locale);            

            if (!Directory.Exists(DocumentBaseDir))
            {
                DocumentBaseDir = InitBaseDir("en-US");
                log.Warn("Locale Base Directory Not Found!  Using default en-US");
            }
        }

        private string InitBaseDir(string Locale)
        {
            log.Debug(string.Format("InitBaseDir Locale:{0}",Locale));
            // Information about Documents directory provided by Sakura4
            string Result = "The Sims 3";
            switch (Locale)
            {
                case "en-US": Result = "The Sims 3"; break;
                case "cs-CZ": Result = "The Sims 3"; break;
                case "da-DK": Result = "The Sims 3"; break;
                case "nl-NL": Result = "De Sims 3"; break;
                case "fi-FI": Result = "The Sims 3"; break;
                case "fr-FR": Result = "Les Sims 3"; break;
                case "de-DE": Result = "Die Sims 3"; break;
                case "el-GR": Result = "The Sims 3"; break;
                case "hu-HU": Result = "The Sims 3"; break;
                case "it-IT": Result = "The Sims 3"; break;
                case "ja-JP": Result = "ザ・シムズ３"; break;
                case "ko-KR": Result = "심즈 3"; break;
                case "no-NO": Result = "The Sims 3"; break;
                case "pl-PL": Result = "The Sims 3"; break;
                case "pt-BR": Result = "The Sims 3"; break;
                case "pt-PT": Result = "Os Sims 3"; break;
                case "ru-RU": Result = "The Sims 3"; break;
                case "es-ES": Result = "Los Sims 3"; break;
                case "es-MX": Result = "The Sims 3"; break;
                case "sv-SE": Result = "The Sims 3"; break;
                case "th-TH": Result = "เดอะซิมส์ 3"; break;
                case "zh-CN": Result = "模拟人生3"; break;
                case "zh-TW": Result = "模擬市民3"; break;
                default: log.Warn(string.Format("Unknown Locale {0}", Locale)); break;
            }
            log.Info (string.Format("Result = {0}",Result));
            string FullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Electronic Arts", Result);
            log.Info(string.Format("Result[full] = {0}", FullPath));
            return FullPath;
        }

        internal void LoadRegistry()
        {
            log.Debug("LoadRegistry");
            RegistryKey BaseKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE");
            RegistryKey SubKey;

            try
            {
                SubKey = BaseKey.OpenSubKey("Sims");
                IsSteam = false;

                if (SubKey == null)
                {
                    // try the Steam version
                    SubKey = BaseKey.OpenSubKey("Sims(Steam)");
                    IsSteam = true;
                }

                if (SubKey != null)
                {

                    // Now since we know it does exist, open it                    
                    log.Debug("Registry Open");
                    try
                    {
                        var KeyList = SubKey.GetSubKeyNames();

                        log.Info(string.Format("LoadRegistry -- Found {0} Game Packs", KeyList.Length));

                        foreach (var entry in KeyList)
                        {
                            Packs.Add(new InstalledGameEntry(SubKey, entry));
                        }
                    }
                    finally
                    {
                        SubKey.Close();
                        log.Debug("Registry Close");
                    }
                }

            }
            finally
            {
                BaseKey.Close();
            }
        }

        internal void InitEntryProperties()
        {
            log.Debug("InitEntryProperties");
            if (Packs == null)
            {
                log.Debug("Packs == null");
            }
            else
            {
                log.Debug(string.Format("  {0} Packs found", Packs.Count));
            }
            foreach (var entry in Packs)
            {
                if (entry.IsBaseGame)
                    BaseGame = entry;

                if (NewestGame == null)
                {
                    NewestGame = entry;
                }
                else if ((entry.ProductID > NewestGame.ProductID) & (entry.ProductID < 100))
                {
                    NewestGame = entry;
                }
            }

            if (BaseGame == null)
            {
                log.Debug("BaseGame == null");
            }
            else
            {
                log.Info(string.Format("BaseGame = {0}", BaseGame.DisplayName));
            }

            if (NewestGame == null)
            {
                log.Debug("NewestGame == null");
            }
            else
            {
                log.Info(string.Format("NewestGame = {0}", NewestGame.DisplayName));
            }
        }
    }
}
