using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using View.General;

namespace Views.PriceTracker
{
    public class PriceTrackerCoinItemView : CoinItemView, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] 
        private Image _graphImage;
        [SerializeField] 
        private List<Sprite> _graphSprites;
        [SerializeField] 
        private Text _profitCount;
        
        private string _coinId;
        private float _pressStartTime;
        private bool _isPressing = false;
        private const float LongPressDuration = 1f;

        public event Action<PriceTrackerCoinItemView> OnStartPressAction;
        public event Action<PriceTrackerCoinItemView, bool> OnEndPressAction;
        
        void Update()
        {
            if (_isPressing && Time.time - _pressStartTime > LongPressDuration)
            {
                _isPressing = false;
                OnEndPressAction?.Invoke(this, true);
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnStartPressAction?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressing = false;
            
            OnEndPressAction?.Invoke(this, false);
        }

        public void SetCanCheckPressTime()
        {
            _pressStartTime = Time.time;
            _isPressing = true;
        }

        public void SetGraphSprite(bool isProfit)
        {
            int index = isProfit ? 0 : 1;

            _graphImage.sprite = _graphSprites[index];
            
            _graphImage.SetNativeSize();
            
            RectTransform rect = _graphImage.rectTransform;
            
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width/4);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.height/4);
        }

        public void SetProfit(float percent)
        {
            _profitCount.text = $"{percent:+0.00;-0.00}%";
            _profitCount.color = percent >= 0 ? Color.green : Color.red;
        }
    }
}