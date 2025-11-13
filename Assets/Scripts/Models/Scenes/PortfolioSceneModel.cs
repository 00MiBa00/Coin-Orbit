using UnityEngine;
using System.Collections.Generic;
using Models.Support;
using Utility;

namespace Models.Scenes
{
    public class PortfolioSceneModel
    {
        private const string SaveKey = "SavedCoins";
        
        private List<SavedCoinData> _savedCoins = new List<SavedCoinData>();
        private SavedCoinData _currentCoin;

        public void AddSaveCoinId(string id, string fullName, string shortName)
        {
            _currentCoin = new SavedCoinData 
                { 
                    ID = id, 
                    FullName = fullName,
                    ShortName = shortName
                };
        }

        public void AddSaveCoinCost(float cost)
        {
            if (_currentCoin != null)
                _currentCoin.LastCost = cost;
        }

        public void AddSaveCoinDataCount(float count)
        {
            if (_currentCoin == null) return;

            _currentCoin.Count = count;
        }

        public float GetSumPrice()
        {
            return _currentCoin != null ? _currentCoin.Count * _currentCoin.LastCost : 0f;
        }

        public float GetBalance()
        {
            List<SavedCoinData> savedCoins = new List<SavedCoinData>(LoadCoins() ?? new List<SavedCoinData>());
            
            float totalValue = 0f;

            foreach (var coin in savedCoins)
            {
                float currentPrice = CoinAPIUtility.GetCoinModelFromCache(coin.ID)?.Cost ?? 0f;
                totalValue += coin.Count * currentPrice;
            }

            return totalValue;
        }

        public float GetProfit()
        {
            List<SavedCoinData> savedCoins = new List<SavedCoinData>(LoadCoins() ?? new List<SavedCoinData>());
            
            float totalCostBasis = 0f;

            foreach (var coin in savedCoins)
            {
                totalCostBasis += coin.Count * coin.LastCost;
            }
            
            float profit = GetBalance() - totalCostBasis;

            return profit;
        }

        public void DeleteCoin(int index)
        {
            List<SavedCoinData> coins = new List<SavedCoinData>(LoadCoins() ?? new List<SavedCoinData>());

            if (index >= coins.Count)
            {
                return;
            }
            
            coins.RemoveAt(index);
            
            var wrapper = new SavedCoinsWrapper { Coins = coins };
            string json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
            
            _savedCoins = coins;
        }

        private void FinalizeCoin()
        {
            _savedCoins = new List<SavedCoinData>(LoadCoins() ?? new List<SavedCoinData>());
            
            var existing = _savedCoins.Find(c => c.ID == _currentCoin.ID);

            if (existing != null)
            {
                existing.Count += _currentCoin.Count;
                existing.LastCost = _currentCoin.LastCost;
            }
            else
            {
                _savedCoins.Add(_currentCoin);
            }

            Debug.Log(_savedCoins.Count);
            
            SaveCoins();
            _currentCoin = null;
        }

        public void SaveCoin()
        {
            FinalizeCoin();
        }

        private void SaveCoins()
        {
            var wrapper = new SavedCoinsWrapper { Coins = _savedCoins };
            string json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public List<SavedCoinData> LoadCoins()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return null;

            string json = PlayerPrefs.GetString(SaveKey);
            var wrapper = JsonUtility.FromJson<SavedCoinsWrapper>(json);
            _savedCoins = wrapper.Coins ?? new List<SavedCoinData>();
            
            return _savedCoins;
        }
    }
}