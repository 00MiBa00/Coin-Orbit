using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Converter
{
    public class ConverterCoinBtn : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _shortName;

        public event Action OnPressBtnAction;

        private void Awake()
        {
            _btn.onClick.AddListener(OnPressBtn);
        }

        private void OnDestroy()
        {
            _btn.onClick.RemoveAllListeners();
        }

        public void SetInfo(Sprite sprite, string shortName)
        {
            _icon.sprite = sprite;
            _shortName.text = shortName;
        }

        private void OnPressBtn()
        {
            OnPressBtnAction?.Invoke();
        }
    }
}