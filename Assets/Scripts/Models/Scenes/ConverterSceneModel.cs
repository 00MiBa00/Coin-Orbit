using System;
using System.Collections.Generic;
using Models.General;
using UnityEngine;
using Utility;

namespace Models.Scenes
{
    public class ConverterSceneModel
    {
        private const string FromCoinKey = "ConverterSceneModel.ConverterFromCoinId";
        private const string ToCoinKey = "ConverterSceneModel.ConverterToCoinId";

        private bool _isFromCoin;

        private string FromCoinId
        {
            get => PlayerPrefs.GetString(FromCoinKey, "bitcoin");
            set => PlayerPrefs.SetString(FromCoinKey, value);
        }
        
        private string ToCoinId
        {
            get => PlayerPrefs.GetString(ToCoinKey, "usd-coin");
            set => PlayerPrefs.SetString(ToCoinKey, value);
        }
        
        public ItemCoinModel GetFromCoinModel()
        {
            return CoinAPIUtility.GetCoinModelFromCache(FromCoinId);
        }

        public ItemCoinModel GetToCoinModel()
        {
            return CoinAPIUtility.GetCoinModelFromCache(ToCoinId);
        }

        public void SetNewCoin(string id)
        {
            if (_isFromCoin)
            {
                FromCoinId = id;
            }
            else
            {
                ToCoinId = id;
            }
        }

        public void LoadTopCoins(Action<List<ItemCoinModel>> onComplete)
        {
            Debug.Log("🌀 Loading top coins...");

            CoinAPIUtility.EnsureCacheFresh(() =>
            {
                var topCoins = CoinAPIUtility.GetTopCoinsFromCache(10);

                if (topCoins == null || topCoins.Count == 0)
                {
                    Debug.LogWarning("⚠️ No top coins found in cache after refresh.");
                }
                else
                {
                    Debug.Log($"✅ Loaded top {topCoins.Count} coins from cache.");
                }

                onComplete?.Invoke(topCoins);
            });
        }

        public void SetSelectedCoin(bool isFromCoin)
        {
            _isFromCoin = isFromCoin;
        }

        public void SwapCoins()
        {
            (FromCoinId, ToCoinId) = (ToCoinId, FromCoinId);
        }
    }
}