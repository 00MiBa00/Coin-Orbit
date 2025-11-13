using System;
using System.Globalization;
using Models.General;
using UnityEngine;
using UnityEngine.UI;

namespace View.General
{
    public class CoinItemView : MonoBehaviour
    {
        [SerializeField] 
        private Image _image;
        [SerializeField] 
        private Text _fullName;
        [SerializeField]
        private Text _shortName;
        [SerializeField] 
        private Text _cost;
        [SerializeField]
        private Button _btn;

        private ItemCoinModel _model;
            
        public event Action<ItemCoinModel> OnPressBtnAction;

        private void OnEnable()
        {
            _btn?.onClick.AddListener(OnPressBtn);
        }

        private void OnDisable()
        {
            _btn?.onClick.RemoveAllListeners();
        }

        public void SetInfo(ItemCoinModel model)
        {
            _model = model;
            
            SetSprite(model.Icon);
            SetFullName(model.FullName);
            SetShortName(model.ShortName);
            SetCost(model.Cost, model.IsChooseCoin);
        }

        private void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        private void SetFullName(string value)
        {
            _fullName.text = value;
        }
        
        private void SetShortName(string value)
        {
            _shortName.text = value;
        }

        private void SetCost(float value, bool isChooseCoin)
        {
            string formatted = value.ToString("N2", new CultureInfo("ru-RU"));
            string text = !isChooseCoin ? $"${formatted}" : formatted;

            _cost.text = text;
        }

        private void OnPressBtn()
        {
            OnPressBtnAction?.Invoke(_model);
        }
    }
}