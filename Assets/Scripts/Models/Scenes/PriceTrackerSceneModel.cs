using System;
using System.Collections.Generic;
using Models.General;
using Utility;
using UnityEngine;

namespace Models.Scenes
{
    public class PriceTrackerSceneModel
    {
        public bool canAddPriceTrackerCoinsItems => LoadIdCoins() != null;
        
        private const string SaveCoinKey = "PriceTrackerSceneModel.SaveCoin";
        private const string SaveCoinsCountKey = "PriceTrackerSceneModel.SaveCoinsCount";
        
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

        public void DeleteActiveItem(int index)
        {
            List<string> idCoins = LoadIdCoins();
            
            idCoins.RemoveAt(index);
            
            ClearSavedCoins();
            
            for (int i = 0; i < idCoins.Count; i++)
            {
                PlayerPrefs.SetString($"{SaveCoinKey}{i}", idCoins[i]);
            }
            
            PlayerPrefs.SetInt(SaveCoinsCountKey, idCoins.Count);
        }

        public void SaveCoins(string idCoin)
        {
            List<string> idCoins = LoadIdCoins();

            if (idCoins == null)
            {
                idCoins = new List<string>();
            }

            if (idCoins.Contains(idCoin))
            {
                return;
            }

            idCoins.Add(idCoin);
            
            ClearSavedCoins();
            
            for (int i = 0; i < idCoins.Count; i++)
            {
                PlayerPrefs.SetString($"{SaveCoinKey}{i}", idCoins[i]);
            }
            
            PlayerPrefs.SetInt(SaveCoinsCountKey, idCoins.Count);
        }
        
        public List<ItemCoinModel> LoadSavedCoinModels()
        {
            var ids = LoadIdCoins();
            if (ids == null || ids.Count == 0) return new List<ItemCoinModel>();

            List<ItemCoinModel> models = new List<ItemCoinModel>();

            foreach (string id in ids)
            {
                var model = CoinAPIUtility.GetCoinModelFromCache(id);
                if (model != null)
                    models.Add(model);
            }

            return models;
        }
        
        private List<string> LoadIdCoins()
        {
            int count = PlayerPrefs.GetInt(SaveCoinsCountKey, 0);

            if (count == 0)
            {
                return null;
            }

            List<string> idCoins = new List<string>();

            for (int i = 0; i < count; i++)
            {
                idCoins.Add(PlayerPrefs.GetString($"{SaveCoinKey}{i}"));
            }

            return idCoins;
        }
        
        private void ClearSavedCoins()
        {
            int count = PlayerPrefs.GetInt(SaveCoinsCountKey, 0);

            for (int i = 0; i < count; i++)
            {
                PlayerPrefs.DeleteKey($"{SaveCoinKey}{i}");
            }

            PlayerPrefs.DeleteKey(SaveCoinsCountKey);
        }
    }
}