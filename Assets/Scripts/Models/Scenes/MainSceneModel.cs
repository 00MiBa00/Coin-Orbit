using System;
using System.Collections.Generic;

using Models.General;
using UnityEngine;
using Utility;

namespace Models.Scenes
{
    public class MainSceneModel
    {
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
    }
}