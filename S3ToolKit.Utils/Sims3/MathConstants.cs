﻿using System;
namespace Sims3.SimIFace
{
    internal class MathConstants
    {
        public static readonly Matrix44 gIdentMatrix44 = new Matrix44(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f));
        private MathConstants()
        {
        }
    }
}
