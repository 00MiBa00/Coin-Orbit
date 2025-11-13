using System;
using UnityEngine;
using UnityEngine.UI;
using Views.General;

namespace Views.Converter
{
    public class ConverterMainPanel : PanelView
    {
        [SerializeField] private ConverterCoinBtn _converterFromCoinBtn;
        [SerializeField] private ConverterCoinBtn _converterToCoinBtn;
        [SerializeField] private InputField _fromCoinInputFiled;
        [SerializeField] private ToCoinTextUpdater _toCoinTextUpdater;

        public event Action<string> OnInputValueChangedAction;
        public event Action<bool> OnPressCoinBtnAction;

        private void Awake()
        {
            _fromCoinInputFiled.onValueChanged.AddListener(OnInputFieldValueChanged);
            _converterFromCoinBtn.OnPressBtnAction += OnPressFromCoinBtn;
            _converterToCoinBtn.OnPressBtnAction += OnPressToCoinBtn;
        }

        private void OnDestroy()
        {
            _fromCoinInputFiled.onValueChanged.RemoveListener(OnInputFieldValueChanged);
            _converterFromCoinBtn.OnPressBtnAction -= OnPressFromCoinBtn;
            _converterToCoinBtn.OnPressBtnAction -= OnPressToCoinBtn;
        }

        public void SetFromCoinInfo(Sprite sprite, string shortName)
        {
            _converterFromCoinBtn.SetInfo(sprite, shortName);
        }
        
        public void SetToCoinInfo(Sprite sprite, string shortName)
        {
            _converterToCoinBtn.SetInfo(sprite, shortName);
        }

        public void SetToCoinInfo(float count, float cost)
        {
            _toCoinTextUpdater.SetInfo(count, cost);
        }

        public void Reset()
        {
            _fromCoinInputFiled.text = "";
            _toCoinTextUpdater.SetInfo(0, 0);
        }

        private void OnInputFieldValueChanged(string input)
        {
            OnInputValueChangedAction?.Invoke(input);
        }

        private void OnPressFromCoinBtn()
        {
            OnPressCoinBtnAction?.Invoke(true);
        }

        private void OnPressToCoinBtn()
        {
            OnPressCoinBtnAction?.Invoke(false);
        }
    }
}