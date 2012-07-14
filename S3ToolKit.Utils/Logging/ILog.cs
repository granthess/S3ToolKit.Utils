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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S3ToolKit.Utils.Logging
{
    public interface ILog
    {

        // Implements the following message levels
                
        //* DEBUG    5
        //* INFO     4
        //* WARN     3
        //* ERROR    2
        //* FATAL    1
        //* OFF      0

        void Debug(string Message);
        void Info(string Message);
        void Warn(string Message);
        void Error(string Message);
        void Fatal(string Message);

    }
}
