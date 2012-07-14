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
using System.Linq;
using System.Text;

namespace S3ToolKit.Utils.Logging
{
    public class Log : ILog
    {
        public LogManager logManager { get; private set; }
        public string ModuleName { get; private set; }

        public Log(LogManager logManager, string ModuleName)
        {
            this.logManager = logManager;
            this.ModuleName = ModuleName;
        }

        public void Debug(string Message)         
        {
            PostLog(LogManager.LogLevel.Debug, Message);
        }

        public void Info(string Message) 
        {
            PostLog(LogManager.LogLevel.Info, Message);
        }

        public void Warn(string Message) 
        {
            PostLog(LogManager.LogLevel.Warn, Message);
        }

        public void Error(string Message) 
        {
            PostLog(LogManager.LogLevel.Error, Message);
        }

        public void Fatal(string Message)
        {
            PostLog(LogManager.LogLevel.Fatal, Message);
        }

        private void PostLog(LogManager.LogLevel Level, string Message)
        {
            logManager.PostEntry(Level, ModuleName, Message);
        }
    }
}
