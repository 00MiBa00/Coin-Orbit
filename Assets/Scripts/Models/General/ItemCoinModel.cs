using UnityEngine;

namespace Models.General
{
    public class ItemCoinModel
    {
        public string Id { get; }
        public string FullName { get; }
        public string ShortName { get; }
        public float Cost { get; }
        public Sprite Icon { get; }
        public bool IsChooseCoin { get; }
        public float PriceChangePercent24h { get; }

        public ItemCoinModel(string id, string fullName, string shortName, float cost, Sprite icon, bool isChooseCoin, float priceChangePercent24h)
        {
            Id = id;
            FullName = fullName;
            ShortName = shortName;
            Cost = cost;
            Icon = icon;
            IsChooseCoin = isChooseCoin;
            PriceChangePercent24h = priceChangePercent24h;
        }
    }
}