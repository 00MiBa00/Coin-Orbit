using System;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Portfolio
{
    public class ResultCoinItemView : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;

        public event Action<ResultCoinItemView> OnPressBtnAction;

        private void OnEnable()
        {
            _btn.onClick.AddListener(OnPressBtn);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveAllListeners();
        }

        public void SetInfo(Sprite sprite, string text)
        {
            _icon.sprite = sprite;
            _name.text = text;
        }

        private void OnPressBtn()
        {
            OnPressBtnAction?.Invoke(this);
        }
    }
}