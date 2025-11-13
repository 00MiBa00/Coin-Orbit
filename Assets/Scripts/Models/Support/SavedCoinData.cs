using System;
using System.Collections.Generic;

namespace Models.Support
{
    [Serializable]
    public class SavedCoinData
    {
        public string ID;
        public float LastCost;
        public string FullName;
        public string ShortName;
        public float Count;
    }

    [Serializable]
    public class SavedCoinsWrapper
    {
        public List<SavedCoinData> Coins = new List<SavedCoinData>();
    }
}