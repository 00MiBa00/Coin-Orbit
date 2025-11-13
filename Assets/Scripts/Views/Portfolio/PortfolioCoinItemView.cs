using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using View.General;

namespace Views.Portfolio
{
    public class PortfolioCoinItemView : CoinItemView, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] 
        private Text _count;
        
        private string _coinId;
        private float _pressStartTime;
        private bool _isPressing = false;
        private const float LongPressDuration = 1f;
        
        public event Action<PortfolioCoinItemView> OnStartPressAction;
        public event Action<PortfolioCoinItemView, bool> OnEndPressAction;
        
        void Update()
        {
            if (_isPressing && Time.time - _pressStartTime > LongPressDuration)
            {
                _isPressing = false;
                OnEndPressAction?.Invoke(this, true);
            }
        }

        public void SetCount(float value)
        {
            _count.text = ((decimal)value).ToString("0.################");
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
    }
}