﻿/*
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
    along with S3ToolKit.Utils.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using S3Launcher; 
using System.IO;

namespace S3ToolKit.Utils.Test
{
    [TestFixture]
    public class FNVTests
    {
        [Test]
        public void TestFNV64()
        {
            UInt64 ExpectedResult = 0xFB1D3BAE83872E94;
                                    
            string TestPhrase = "Test Phrase";
            UInt64 result = FNV.FNV64(TestPhrase);
            Assert.AreEqual(ExpectedResult , result);
        }
    }
}
