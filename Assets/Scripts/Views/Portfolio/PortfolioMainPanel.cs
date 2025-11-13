using System;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using UnityEngine.UI;

using Models.General;
using Models.Support;

using Utility;
using Views.General;

namespace Views.Portfolio
{
    public class PortfolioMainPanel : PanelView
    {
        [SerializeField] private GameObject _portfolioCoinItemPrefab;
        [SerializeField] private Transform _conteiner;
        [SerializeField] private Text _balanceText;
        [SerializeField] private Text _profitText;

        private PortfolioCoinItemView _selectedView = null;
        private List<GameObject> _activeObjects;
        private bool _canCheckPressTime;
        
        public event Action<int> OnCanDeleteItemAction;
        
        public void SetCoinsInfo(List<SavedCoinData> datas)
        {
            if (datas == null || datas.Count == 0)
            {
                return;
            }
            
            _canCheckPressTime = true;
            
            Clear();

            foreach (var data in datas)
            {
                var coin = CoinAPIUtility.GetCoinMarketModelById(data.ID);

                if (coin == null)
                {
                    Debug.LogWarning($"⚠️ Coin {data.ID} not found in cache.");
                    continue;
                }

                CoinAPIUtility.LoadOrDownloadIcon(coin.id, coin.image, sprite =>
                {
                    var model = new ItemCoinModel(
                        coin.id,
                        coin.name,
                        coin.symbol.ToUpper(),
                        data.Count * data.LastCost,
                        sprite,
                        false,
                        coin.price_change_percentage_24h
                    );

                    GameObject go = Instantiate(_portfolioCoinItemPrefab, _conteiner);
                    go.transform.SetSiblingIndex(0);
                    PortfolioCoinItemView view = go.GetComponent<PortfolioCoinItemView>();
                    
                    view.OnStartPressAction += OnStartPressItem;
                    view.OnEndPressAction += OnEndPressItem;
                    
                    _activeObjects.Add(go);

                    view.SetInfo(model);
                    view.SetCount(data.Count);
                });
            }
        }

        public void UpdateBalance(float value)
        {
            string formatted = $"${value:N2}";
            
            _balanceText.text = $"${value:N2}";
        }
        
        public void SetProfit(float profitValue)
        {
            string sign = profitValue > 0 ? "+" : profitValue < 0 ? "-" : "";
            string formatted = Mathf.Abs(profitValue).ToString("#,0.00", new CultureInfo("ru-RU"));
    
            _profitText.text = $"{sign}${formatted}";
    
            if (profitValue > 0)
                _profitText.color = Color.green;
            else if (profitValue < 0)
                _profitText.color = Color.red;
            else
                _profitText.color = Color.gray;
        }
        
        public void DestroyItem(int index)
        {
            _canCheckPressTime = true;
            _selectedView = null;
            
            Destroy(_activeObjects[index]);
            _activeObjects.RemoveAt(index);
        }

        private void Clear()
        {
            _activeObjects ??= new List<GameObject>();

            if (_activeObjects.Count == 0)
            {
                return;
            }

            foreach (var go in _activeObjects)
            {
                Destroy(go);
            }
        }
        
        private void OnStartPressItem(PortfolioCoinItemView view)
        {
            if (!_canCheckPressTime)
            {
                return;
            }

            _canCheckPressTime = false;
            _selectedView = view;
            
            view.SetCanCheckPressTime();
        }
        
        private void OnEndPressItem(PortfolioCoinItemView view, bool isSuccess)
        {
            if (_selectedView != view)
            {
                return;
            }

            if (!isSuccess)
            {
                _canCheckPressTime = true;
                return;
            }
            
            view.OnEndPressAction -= OnEndPressItem;

            int index = _activeObjects.IndexOf(view.gameObject);
            
            Debug.Log($"index {index} Count {_activeObjects.Count}");
            
            OnCanDeleteItemAction?.Invoke(index);
        }
    }
}