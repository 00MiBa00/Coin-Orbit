using UnityEngine;
using UnityEngine.UI;

namespace Views.Converter
{
    public class ToCoinTextUpdater : MonoBehaviour
    {
        [SerializeField] private Text _countText;
        [SerializeField] private Text _costText;

        public void SetInfo(float count, float cost)
        {
            _countText.text = FormatNumber(count);
            _costText.text = FormatCurrency(cost);
        }
        
        string FormatNumber(float value)
        {
            return value.ToString("#,0.######", new System.Globalization.CultureInfo("ru-RU"));
        }

        string FormatCurrency(float value)
        {
            return "$" + value.ToString("#,0.00", new System.Globalization.CultureInfo("ru-RU"));
        }
    }
}