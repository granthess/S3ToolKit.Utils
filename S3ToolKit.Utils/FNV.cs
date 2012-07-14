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

namespace S3Launcher
{
    public static class FNV
    {

        // Bits Prime 	            Offset
        // 32 	0x01000193 	        0x811C9DC5
        // 64 	0x00000100000001B3 	0xCBF29CE484222325

        static UInt64 Prime64 = 0x00000100000001B3;
        static UInt64 Offset64 = 0xCBF29CE484222325;

        static UInt32 Prime32 = 0x01000193;
        static UInt32 Offset32 = 0x811c9dc5;

        public static UInt32 FNV32(string Value)
        {
            UInt32 hash = Offset32;

            byte[] text = Encoding.ASCII.GetBytes(Value.ToLower());

            foreach (byte b in text)
            {
                hash *= Prime32;
                hash ^= b;
            }

            return hash;
        }

        public static UInt64 FNV64(string Value)
        {
            UInt64 hash = Offset64;

            byte[] text = Encoding.ASCII.GetBytes(Value.ToLower());

            foreach (byte b in text)
            {
                hash *= Prime64;
                hash ^= b;
            }

            return hash;
        }

        //FNV(byte[] data)
        //{
        //    hash = offset;
        //    foreach (byte b in data)
        //    {
        //        hash *= prime;
        //        hash ^= b;
        //    }
        //    return hash;
        //}
    }
}

