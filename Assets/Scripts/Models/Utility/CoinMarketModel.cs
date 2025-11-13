using System;

namespace Models.Utility
{
    [Serializable]
    public class CoinMarketModel
    {
        public string id;
        public string symbol;
        public string name;
        public float current_price;
        public string image;
        public float price_change_percentage_24h;
    }
}