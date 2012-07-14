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
using S3ToolKit.Utils;

namespace S3ToolKit.Utils
{
    public class IniFile
    {
        public string FileName { get; private set; }

        public Dictionary<string, Dictionary<string, string>> Entries { get; private set; }

        public IniFile(string FileName)
        {
            this.FileName = FileName;
            Entries = new Dictionary<string, Dictionary<string, string>>();
            Load();
        }

        public void Load()
        {
            var Lines = File.ReadAllLines(FileName);

            string CurrentSection = "";
            Dictionary<string, string> Current = null;
            string Key;
            string Value;

            foreach (string LineItem in Lines)
            {
                string Line = LineItem.Trim();
                if (Line == "")
                {
                    // skip blank lines since EA seems to want to doublespace the file
                }
                else if (Line.StartsWith("["))
                {
                    CurrentSection = Line.Trim("[]".ToCharArray());
                    if (!Entries.ContainsKey(CurrentSection))
                    {
                        Entries.Add(CurrentSection, new Dictionary<string, string>());
                    }

                    Current = Entries[CurrentSection];
                }
                else
                {
                    Key = Line.Substring(0, Line.IndexOf('=') - 1).Trim();
                    Value = Line.Substring(Line.IndexOf('=') + 1).Trim();

                    if (Current != null)
                    {
                        Current.Add(Key, Value);
                    }
                }
            }


        }

        public void Save()
        {
            List<string> Lines = new List<string>();

            foreach (var Section in Entries)
            {
                Lines.Add(string.Format("[{0}]", Section.Key));

                foreach (var Item in Section.Value)
                {
                    Lines.Add(string.Format("{0} = {1}", Item.Key, Item.Value));
                }
            }

            FileTools.SwapBackupChain(FileName);
            File.WriteAllLines(FileName,Lines);
        }


        public string GetValue(string Section, string Key)
        {
            if (!Entries.ContainsKey(Section))
            {
                return null;
            }

            if (!Entries[Section].ContainsKey(Key))
            {
                return null;
            }

            return Entries[Section][Key];
        }

        public void SetValue(string Section, string Key, string Value)
        {
            Dictionary<string, string> CurrentSection = null;

            if (!Entries.ContainsKey(Section))
            {
                CurrentSection = new Dictionary<string, string>();
                Entries.Add(Section, CurrentSection);
            }
            else
            {
                CurrentSection = Entries[Section];
            }

            if (!CurrentSection.ContainsKey(Key))
            {
                CurrentSection.Add(Key, Value);
            }
            else
            {
                CurrentSection[Key] = Value;
            }
        }
    }
}
