using System;
using Models.General;

namespace Actions
{
    public static class PortfolioActions
    {
        public static event Action<ItemCoinModel> OnSelectedCoinAction;
        public static event Action<float> OnAmountValueChangedAction; 
        
        public static void RaiseSelectedCoin(ItemCoinModel model)
        {
            OnSelectedCoinAction?.Invoke(model);
        }

        public static void ChangedAmountCoin(float value)
        {
            OnAmountValueChangedAction?.Invoke(value);
        }
    }
}