﻿using System;

namespace NeoForge.Utilities.Movement
{
    [Serializable]
    public class FloatRange
    {
        public float Min;
        public float Max;
        public bool IsInRange(float value) => value >= Min && value <= Max;
        public bool IsInRange(int value) => value >= Min && value <= Max;
    }
}