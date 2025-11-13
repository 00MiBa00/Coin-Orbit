using System.Globalization;
using Models.General;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Main
{
    public class MainTopCoinItemView : MonoBehaviour
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
        private Text _profitCount;
        
        public void SetInfo(ItemCoinModel model)
        {
            SetSprite(model.Icon);
            SetFullName(model.FullName);
            SetShortName(model.ShortName);
            SetCost(model.Cost, model.IsChooseCoin);
        }
        
        public void SetProfit(float percent)
        {
            _profitCount.text = $"{percent:+0.00;-0.00}%";
            _profitCount.color = percent >= 0 ? Color.green : Color.red;
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
    }
}