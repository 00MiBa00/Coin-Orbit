using System.Collections.Generic;
using UnityEngine;

using Actions;
using Models.General;

namespace Views.Portfolio
{
    public class ResultBodyView : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _resultItemPrefab;

        private List<ItemCoinModel> _activeModels;
        private List<ResultCoinItemView> _activeResultCoinItems;

        public void ShowCoins(List<ItemCoinModel> models)
        {
            _activeModels = new List<ItemCoinModel>(models);
            _activeResultCoinItems ??= new List<ResultCoinItemView>();
            
            foreach (var coin in models)
            {
                GameObject go = Instantiate(_resultItemPrefab, transform);
                ResultCoinItemView itemView = go.GetComponent<ResultCoinItemView>();

                itemView.OnPressBtnAction += OnPressResultCoinItem;

                itemView.SetInfo(coin.Icon, coin.FullName);
                
                _activeResultCoinItems.Add(itemView);
            }
        }
        
        public void ClearList()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            _activeResultCoinItems?.Clear();
        }

        private void OnPressResultCoinItem(ResultCoinItemView view)
        {
            int index = _activeResultCoinItems.IndexOf(view);
            
            PortfolioActions.RaiseSelectedCoin(_activeModels[index]);
        }
    }
}