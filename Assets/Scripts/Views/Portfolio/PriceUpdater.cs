using UnityEngine;
using UnityEngine.UI;

namespace Views.Portfolio
{
    public class PriceUpdater : MonoBehaviour
    {
        [SerializeField] 
        private Text _text;

        public void UpdateText(float value)
        {
            _text.text = $"${value:F2}";
        }
    }
}