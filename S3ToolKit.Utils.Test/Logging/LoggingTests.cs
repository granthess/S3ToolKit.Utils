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
using NUnit.Framework;
using NSubstitute.Core;

using S3ToolKit.Utils.Logging;

namespace S3ToolKit.Utils.Test.Logging
{
    [TestFixture]
    class LoggingTests
    {
        private string LogFileName;
        
        [TestFixtureSetUp]
        public void Setup()
        {
            LogFileName = Path.GetTempFileName();
            File.Delete(LogFileName);  // GetTempFileName creates a file, we don't want it to exist until LogManager creates it
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
            LogManager.IsEnabled = false;
            File.Delete(LogFileName);  // Remove file after test
        }

        [Test]
        public void Ensure_Initial_Entiries_Are_Stored()
        {
            string Value = DateTime.Now.ToLongTimeString();
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());            
            
            LogManager.IsEnabled = true;
            LogManager.Level = LogManager.LogLevel.Debug;
            log.Info (string.Format("Test --{0}--", Value));
            LogManager.SetFilename (LogFileName);
            LogManager.IsEnabled = false;

            if (!File.Exists(LogFileName))
            {
                Assert.Fail("Log file not created");
            }

            string Result = File.ReadAllText(LogFileName);            
            Assert.GreaterOrEqual(Result.IndexOf(Value), 0, "Log message not found in log file");            
        }       
    }
}
