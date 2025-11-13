using System;
using System.Collections.Generic;
using UnityEngine;

using Models.General;
using Views.General;

namespace Views.PriceTracker
{
    public class MainPanel : PanelView
    {
        [SerializeField] 
        private GameObject _priceTrackerCoinItemPrefab;
        [SerializeField] 
        private Transform _container;
        
        private List<GameObject> _spawnedItems = new List<GameObject>();
        private bool _canCheckPressTime;
        private PriceTrackerCoinItemView _selectedView = null;

        public event Action<int> OnCanDeleteItemAction;

        public void SetCoinsInfo(List<ItemCoinModel> models)
        {
            _canCheckPressTime = true;
            
            ClearItems();

            foreach (var model in models)
            {
                GameObject go = Instantiate(_priceTrackerCoinItemPrefab, _container);
                go.transform.SetSiblingIndex(1);
                var view = go.GetComponent<PriceTrackerCoinItemView>();

                view.SetInfo(model);

                view.OnStartPressAction += OnStartPressItem;
                view.OnEndPressAction += OnEndPressItem;

                bool isProfit = model.PriceChangePercent24h >= 0f;
                view.SetGraphSprite(isProfit);
                view.SetProfit(model.PriceChangePercent24h);

                _spawnedItems.Add(go);
            }
        }
        
        public void DestroyItem(int index)
        {
            Destroy(_spawnedItems[index]);
            _spawnedItems.RemoveAt(index);
            
            _canCheckPressTime = true;
            _selectedView = null;
        }
        
        private void ClearItems()
        {
            foreach (var item in _spawnedItems)
            {
                Destroy(item);
            }

            _spawnedItems.Clear();
        }

        private void OnStartPressItem(PriceTrackerCoinItemView view)
        {
            if (!_canCheckPressTime)
            {
                return;
            }

            _canCheckPressTime = false;

            _selectedView = view;
            
            view.SetCanCheckPressTime();
        }

        private void OnEndPressItem(PriceTrackerCoinItemView view, bool isSuccess)
        {
            if (_selectedView != view)
            {
                return;
            }

            if (!isSuccess)
            {
                _canCheckPressTime = true;
                _selectedView = null;
                return;
            }
            
            view.OnEndPressAction -= OnEndPressItem;

            int index = _spawnedItems.IndexOf(view.gameObject);
            
            OnCanDeleteItemAction?.Invoke(index);
        }
    }
}