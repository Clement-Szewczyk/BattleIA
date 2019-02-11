﻿using System;

namespace BattleIA
{
    public class Bot
    {
        // Fix data
        public Guid GUID;
        public string Name;

        // variables data
        public UInt16 Energy;
        public UInt16 ShieldLevel;
        public UInt16 CloackLevel;

        // dynamics data
        public DateTime StartTime;
        public int ShortID;

        public byte X;
        public byte Y;
    }
}
