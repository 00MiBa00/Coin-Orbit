using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEngine.UI;

using Views.General;
using Models.General;

using Actions;
using Utility;

namespace Views.Portfolio
{
    public class AddCoinPanel : PanelView
    {
        [SerializeField] 
        private InputField _coinInputField;
        [SerializeField] 
        private InputField _amountInputField;
        [SerializeField] 
        private ResultBodyView _resultBodyView;
        [SerializeField] 
        private ResultCoinItemView _selectedResultView;
        [SerializeField] 
        private PriceUpdater _priceUpdater;
        
        private void Awake()
        {
            _coinInputField.onValueChanged.AddListener(OnCoinInputChanged);
            _coinInputField.onEndEdit.AddListener(OnCoinInputConfirmed);
            
            _amountInputField.onValueChanged.AddListener(OnAmountInputChanged);
            _amountInputField.onEndEdit.AddListener(OnAmountInputEnded);

            _selectedResultView.OnPressBtnAction += OnTryChangeCoin;
        }

        private void OnDestroy()
        {
            _coinInputField.onValueChanged.RemoveAllListeners();
            _coinInputField.onEndEdit.RemoveAllListeners();
            
            _amountInputField.onValueChanged.RemoveAllListeners();
            _amountInputField.onEndEdit.RemoveAllListeners();
            
            _selectedResultView.OnPressBtnAction -= OnTryChangeCoin;
        }

        public void SetState(bool isCanSearch)
        {
            if (isCanSearch)
            {
                Clear();
            }

            _coinInputField.gameObject.SetActive(isCanSearch);
            _selectedResultView.gameObject.SetActive(!isCanSearch);
            
            _resultBodyView.gameObject.SetActive(false);
            _amountInputField.interactable = !isCanSearch;
            
            base.SetInteractableBtn(1, false);
        }

        public void SetInfoSelectedResult(Sprite sprite, string name)
        {
            _selectedResultView.SetInfo(sprite, name);
        }

        public void SetSumPrice(float value)
        {
            _priceUpdater.UpdateText(value);
            base.SetInteractableBtn(1, false);
        }

        private void OnCoinInputConfirmed(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            SearchCoinsFromCache(input);
        }

        private void OnCoinInputChanged(string input)
        {
            _resultBodyView.ClearList();
            _resultBodyView.gameObject.SetActive(false);
        }

        private void OnAmountInputChanged(string input)
        {
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float amount))
            {
                PortfolioActions.ChangedAmountCoin(amount);
                base.SetInteractableBtn(1, false);
            }
            else
            {
                Debug.LogWarning("⚠️ Invalid amount input: " + input);
            }
        }

        private void OnAmountInputEnded(string input)
        {
            base.SetInteractableBtn(1, true);
        }

        private void SearchCoinsFromCache(string query)
        {
            var results = CoinAPIUtility.SearchCoinsFromCache(query);
            if (results.Count == 0)
            {
                Debug.Log($"🔍 Coin '{query}' not found in cache.");
                _resultBodyView.ClearList();
                return;
            }

            List<ItemCoinModel> limited = results.GetRange(0, Mathf.Min(3, results.Count));
            _resultBodyView.gameObject.SetActive(true);
            _resultBodyView.ShowCoins(limited);
        }

        private void OnTryChangeCoin(ResultCoinItemView view)
        {
            SetState(true);
        }

        private void Clear()
        {
            _coinInputField.text = "";
            _amountInputField.text = "";
            _priceUpdater.UpdateText(0);
        }
    }
}