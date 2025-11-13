using System.Collections.Generic;
using Models.General;
using UnityEngine;
using Views.General;
using Views.PriceTracker;

namespace Views.Main
{
    public class MainPanel : PanelView
    {
        [SerializeField] private GameObject _mainTopCoinItemPrefab;
        [SerializeField] private GameObject _priceTrackerCoinItemPrefab;
        [SerializeField] private Transform _mainTopCoinContainer;
        [SerializeField] private Transform _priceTrackerCoinItemContainer;

        public void SetInfo(List<ItemCoinModel> models, bool canShowReg)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject go = Instantiate(_mainTopCoinItemPrefab, _mainTopCoinContainer);
                MainTopCoinItemView view = go.GetComponent<MainTopCoinItemView>();
                
                view.SetInfo(models[i]);
                view.SetProfit(models[i].PriceChangePercent24h);
            }

            if (canShowReg)
            {
                return;
            }
            
            for (int i = 3; i < models.Count; i++)
            {
                GameObject go = Instantiate(_priceTrackerCoinItemPrefab, _priceTrackerCoinItemContainer);
                PriceTrackerCoinItemView view = go.GetComponent<PriceTrackerCoinItemView>();
                
                bool isProfit = models[i].PriceChangePercent24h >= 0f;
                view.SetGraphSprite(isProfit);
                view.SetProfit(models[i].PriceChangePercent24h);
                view.SetInfo(models[i]);
            }
        }
    }
}