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
using s3pi.Interfaces;

namespace S3ToolKit.Utils
{
    public class ResourceKey : AResourceKey
    {
        public ResourceKey(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
        }

        public ResourceKey(int APIversion, EventHandler handler, IResourceKey basis)
            : this(APIversion, handler)
        {
            this.instance = basis.Instance;
            this.ResourceGroup = basis.ResourceGroup;
            this.ResourceType = basis.ResourceType;
        }

        public ResourceKey(int APIversion, EventHandler handler, uint resourceType,
            uint resourceGroup, ulong instance)
            : this(APIversion, handler)
        {
            this.Instance = instance;
            this.ResourceGroup = resourceGroup;
            this.ResourceType = resourceType;
        }

        public override AHandlerElement Clone(EventHandler handler)
        {
            return null;
        }

        public override List<string> ContentFields
        {
            get { return null; }
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        private UInt32 ToHex8(string Hex)
        {
            if (Hex.StartsWith("0x"))
            {
                Hex = Hex.Substring(3);
            }

            return UInt32.Parse(Hex, System.Globalization.NumberStyles.HexNumber);
        }

        private UInt64 ToHex16(string Hex)
        {
            if (Hex.StartsWith("0x"))
            {
                Hex = Hex.Substring(3);
            }

            return UInt64.Parse(Hex, System.Globalization.NumberStyles.HexNumber);
        }

        public void SetTGI(string ResourceType, string ResourceGroup, string Instance)
        {
            SetTGI(ToHex8(ResourceType), ToHex8(ResourceGroup), ToHex16(Instance));
        }

        public void SetTGI(UInt32 ResourceType, UInt32 ResourceGroup, UInt64 Instance)
        {
            this.ResourceType = ResourceType;
            this.ResourceGroup = ResourceGroup;
            this.Instance = Instance;
        }
    }
}
