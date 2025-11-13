using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using View.General;
using Models.General;
using Utility;

namespace Views.General
{
    public class ChooseCurrencyPanel : PanelView
    {
        [SerializeField] 
        private GameObject _coinItemPrefab;
        [SerializeField] 
        private Transform _container;
        [SerializeField] 
        private InputField _inputField;

        private List<ItemCoinModel> _currentModels = new List<ItemCoinModel>();

        public event Action<string> OnSelectedCoinAction;

        private void Awake()
        {
            _inputField.onValueChanged.AddListener(OnInputChanged);
            _inputField.onEndEdit.AddListener(OnInputConfirmed);
        }
        
        private bool _ready = false;

        public void SetReady(bool value)
        {
            _ready = value;
        }
        
        public void ResetInput()
        {
            _inputField.text = string.Empty;
            _inputField.DeactivateInputField();
        }

        public void SetDefaultCoins(List<ItemCoinModel> models)
        {
            _currentModels = models;
            ShowCoins(models);
        }

        private void ShowCoins(List<ItemCoinModel> models)
        {
            ClearList();

            foreach (var coin in models)
            {
                GameObject go = Instantiate(_coinItemPrefab, _container);
                CoinItemView itemView = go.GetComponent<CoinItemView>();

                itemView.OnPressBtnAction += OnPressCoinItemBtn;

                itemView.SetInfo(coin);
            }
        }

        private void ClearList()
        {
            foreach (Transform child in _container)
            {
                Destroy(child.gameObject);
            }
        }
        
        private void OnInputChanged(string input)
        {
            if (!_ready) return;

            if (string.IsNullOrWhiteSpace(input))
            {
                ShowCoins(_currentModels);
            }
            else
            {
                ClearList();
            }
        }

        private void OnInputConfirmed(string input)
        {
            if (!_ready) return;

            if (string.IsNullOrWhiteSpace(input)) return;

            SearchCoinsFromCache(input);
        }

        private void SearchCoinsFromCache(string query)
        {
            var results = CoinAPIUtility.SearchCoinsFromCache(query);
            if (results.Count == 0)
            {
                Debug.Log($"🔍 Coin '{query}' not found in cache.");
                ClearList();
                return;
            }

            List<ItemCoinModel> limited = results.GetRange(0, Mathf.Min(10, results.Count));
            ShowCoins(limited);
        }

        private void OnPressCoinItemBtn(ItemCoinModel model)
        {
            OnSelectedCoinAction?.Invoke(model.Id);
        }
    }
}