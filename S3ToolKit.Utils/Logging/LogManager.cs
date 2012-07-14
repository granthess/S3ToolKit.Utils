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
using System.Linq;
using System.Text;
using System.Reflection;

namespace S3ToolKit.Utils.Logging
{
    public sealed class LogManager
    {
        public enum LogLevel
        {
            Off = 0,
            Fatal = 1,
            Error = 2,
            Warn = 3,
            Info = 4,
            Debug = 5
        }

        #region Singleton Support
        private static readonly LogManager instance = new LogManager();

        public LogManager Singleton { get { return instance; } }
        #endregion

        #region Constructor

        private LogManager()
        {
            _LogFilename = "";
            _Buffer = new List<string>();
            // _LogFilename = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".log");
            LogModules = new Dictionary<string, ILog>();
        }

        #endregion

        #region Fields
        private LogLevel _Level;
        private bool _Enabled;
        private string _LogFilename;
        private StreamWriter _LogWriter;
        private Dictionary<string, ILog> LogModules;
        private List<string> _Buffer;
        #endregion

        #region Properties
        public static LogLevel Level { get { return instance._Level; } set { instance._Level = value; } }
        public static bool IsEnabled { get { return instance._Enabled; } set { Set_Enabled(value); } }


        public static string LogFilename { get { return instance._LogFilename; } }
        #endregion

        public static void SetLevel (LogLevel level)
        {
            instance._Level = level;
        }

        public static void Enable()
        {
            IsEnabled = true;
        }

        public static void Disable()
        {
            IsEnabled = false;
        }

        public static ILog GetLogger(string ModuleName)
        {
            return instance.GetLogModule(ModuleName);
        }

        private ILog GetLogModule(string ModuleName)
        {
            // Check to see if a module of that name already exissts
            if (LogModules.ContainsKey(ModuleName))
            {
                return LogModules[ModuleName];
            }
            else
            {
                ILog temp = new Log(this, ModuleName);
                LogModules.Add(ModuleName, temp);
                return temp;
            }
        }

        private static void Set_Enabled(bool value)
        {
            instance.SetEnabled(value);
        }

        public static void SetFilename(string Filename)
        {
            instance.Set_Filename(Filename);
        }


        private string NumberedLogFile(string Filename, int Number)
        {            
            return Path.ChangeExtension(Filename, string.Format(".{0}.log", Number));
        }

        private void RotateLogFiles(string Filename)
        {
            // Resolved bug #13
            // We will keep 5 old logs plus the current one

            if (File.Exists(NumberedLogFile(Filename, 5)))
            {
                File.Delete(NumberedLogFile(Filename, 5));
            }

            for (int i = 4; i > 0; i--)
            {
                if (File.Exists (NumberedLogFile(Filename,i)))
                {
                    File.Move(NumberedLogFile(Filename, i), NumberedLogFile(Filename, i + 1));
                }
            }

            if (File.Exists (Filename))
            {
                File.Move (Filename,NumberedLogFile(Filename,1));
            }
        }

        private void Set_Filename(string Filename)
        {
            if (instance._LogFilename != "")
            {
                throw new ArgumentException("Cannot set Filename twice");
            }

            _LogFilename = Filename;

            // ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_LogFilename));

            // Remove old file if exists
            // Resolved bug #13
            RotateLogFiles(_LogFilename);

            // Create the file to write
            _LogWriter = File.CreateText(_LogFilename);

            // now dump anything in the Buffer back to the file

            foreach (string Line in _Buffer)
            {
                _LogWriter.WriteLine(Line);
            }

            _Buffer.Clear();
            _LogWriter.Flush();
        }

        private void SetEnabled(bool value)
        {
            if (value)
            {
                if (!_Enabled)
                {
                    _Enabled = true;
                }
                else return;  // do nothing, changing Enabled from True to True
            }
            else
            {
                if (_Enabled)
                {
                    if (_LogFilename != "")
                    {
                        // Just close out the file
                        _LogWriter.Close();
                        _Enabled = false;
                        _LogFilename = "";
                    }
                }
                else return;  // do nothing, changing Enabled from False to False
            }
        }

        internal void PostEntry(LogLevel Level, string LogModule, string Message)
        {
            if (_Enabled)
            {
                if (Level <= _Level)
                {
                    if ((_LogFilename != "") && (_LogWriter.BaseStream.CanWrite ))
                    {
                        _LogWriter.WriteLine(string.Format("{0}| {1} | [{2}] -- {3}", DateTime.Now.ToString("u"), Level.ToString(), LogModule, Message));
                        _LogWriter.Flush();
                    }
                    else
                    {
                        _Buffer.Add(string.Format("{0}| {1} | [{2}] -- {3}", DateTime.Now.ToString("u"), Level.ToString(), LogModule, Message));
                    }                        
                }
            }
        }
    }
}


