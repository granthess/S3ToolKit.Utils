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
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace S3ToolKit.Utils
{
    public static class FileTools
    {
        public static bool SwapBackupChain(string FileName)
        {
            string OldName = Path.ChangeExtension(FileName, ".bak");

            bool result = false;

            if (File.Exists(OldName))
            {
                result = true;
                File.Delete(OldName);
            }

            if (File.Exists(FileName))
            {
                File.Move(FileName, OldName);
            }

            return result;
        }

        public static bool MoveOrRecycleDuplicate(string SourceName, string DestName, bool CheckHash)
        {
            string DestFolderName = Path.GetDirectoryName(DestName);
            string DestFileName = Path.GetFileNameWithoutExtension(DestName);
            string DestFileExtension = Path.GetExtension(DestName);

            bool Recycle = false;
            string NewDestName = DestName;

            if (CheckHash)
            {
                if (File.Exists(DestName))
                {
                    string SourceHash = GenerateFileHash(SourceName);

                    // Generate a full list of Name/Hash Pairs for matching patterns
                    var List = GenerateHashForPattern(DestFolderName, DestFileName, DestFileExtension);


                    if (List.ContainsKey(SourceHash))
                    {
                        Recycle = true;
                    }
                    else
                    {
                        bool found = false;                        
                        int i = 0;
                        do
                        {
                            NewDestName = Path.Combine(DestFolderName, ExpandBaseToPattern(DestFileName, DestFileExtension, i++));
                            found = (from item in List.Values
                                     where item.ToLower() == NewDestName.ToLower()
                                     select item).Count() != 0;
                            //foreach (string item in List.Values)
                            //{
                            //    if (item.ToLower() == NewDestName.ToLower())
                            //    {
                            //        found = true; 
                            //        break;
                            //    }

                            //}

                        }
                        while (found);


                    }
                }               
            }
            else
            {
                Recycle = true;              
            }

            if (Recycle)
            {                
                RecycleBin.SendSilent(SourceName);
            }
            else
            {
                File.Move(SourceName, NewDestName);
            }

            return !Recycle;
        }

        private static Dictionary<string, string> GenerateHashForPattern(string FolderName, string BaseName, string Extension)
        {
            string SearchPart = string.Format ("{0}[*{1}",BaseName,Extension).ToLower();
            var FileList = Directory.GetFiles (FolderName, SearchPart);

            Dictionary<string, string> Result = new Dictionary<string, string>();
                        
            foreach (string FileName in FileList)
            {
                try
                {
                    Result.Add(GenerateFileHash(FileName), FileName);
                }
                catch (ArgumentException)
                {

                }
            }

            string OriginalName = Path.ChangeExtension(Path.Combine(FolderName, BaseName), Extension);
            try
            {
                Result.Add(GenerateFileHash(OriginalName), OriginalName);
            }
            catch (ArgumentException)
            {

            }

            return Result;
        }

        private static string ExpandBaseToPattern(string BaseName, string Extension, int Number)
        {
            return string.Format("{0}[{1:X4}]{2}", BaseName, Number, Extension);
        }

        public static string GenerateFileHash(string FileName)
        {
            string result = "";
            if (!File.Exists(FileName))
                return "";
            Stream Source = File.OpenRead(FileName);
            try
            {
                result = GenerateFileHash(Source);
            }
            finally
            {
                Source.Close();
            }

            return result;
        }

        public static string GenerateFileHash(Stream Source)
        {
            SHA1 shar = new SHA1CryptoServiceProvider();

            byte[] result = shar.ComputeHash(Source);
            shar.Dispose();

            StringBuilder output = new StringBuilder();

            foreach (byte x in result)
            {
                output.Append(string.Format("{0:x2}", x));
            }

            return output.ToString();
        }

        public static string FormatFileSize(long FileSize)
        {
            if (FileSize < 1024)
                return string.Format("{0} bytes", FileSize);

            if (FileSize < 1024 * 1024)
                return string.Format("{0} KB", FileSize / 1024);

            return string.Format("{0} MB", FileSize / (1024 * 1024));
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        private static extern long StrFormatByteSize( long fileSize, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, int bufferSize );

        public static string FormatFileSizeAPI(long FileSize)
        {
            StringBuilder buffer = new StringBuilder(128);
            StrFormatByteSize (FileSize,buffer,128);
            return buffer.ToString();
        }
    }
}
