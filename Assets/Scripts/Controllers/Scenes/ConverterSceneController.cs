using UnityEngine;
using Views.Converter;
using Views.General;

using Models.General;
using Models.Scenes;
using Types;
using Utility;


namespace Controllers.Scenes
{
    public class ConverterSceneController : AbstractSceneController
    {
        [SerializeField] private ChooseCurrencyPanel _chooseCurrencyPanel;
        [SerializeField] private ConverterMainPanel _mainPanel;

        private ConverterSceneModel _model;
        
        protected override void OnSceneEnable()
        {
            base.StartSpinning();
            
            SetCoinsChooseCurrencyPanel();
        }

        protected override void OnSceneStart()
        {
            
        }

        protected override void OnSceneDisable()
        {
            
        }

        protected override void Initialize()
        {
            CoinAPIUtility.Initialize(this);

            _model = new ConverterSceneModel();
        }

        protected override void Subscribe()
        {
            _mainPanel.OnPressCoinBtnAction += OnSelectedCoinToChange;
            _mainPanel.OnInputValueChangedAction += OnInputValueChanged;
            _chooseCurrencyPanel.OnSelectedCoinAction += OnSelectedNewCoin;
        }

        protected override void Unsubscribe()
        {
            _mainPanel.OnPressCoinBtnAction -= OnSelectedCoinToChange;
            _mainPanel.OnInputValueChangedAction -= OnInputValueChanged;
            _chooseCurrencyPanel.OnSelectedCoinAction -= OnSelectedNewCoin;
        }
        
        private void SetCoinsChooseCurrencyPanel()
        {
            _chooseCurrencyPanel.SetReady(false);

            _model.LoadTopCoins(models =>
            {
                base.StopSpinning();
                
                _chooseCurrencyPanel.SetDefaultCoins(models);
                _chooseCurrencyPanel.ResetInput();
                _chooseCurrencyPanel.SetReady(true);
                
                OpenMainPanel();
                ClosePanel(_chooseCurrencyPanel);
            });
        }

        private void OpenMainPanel()
        {
            _mainPanel.PressBtnAction += OnReceiveAnswerMainPanel;
            
            _mainPanel.Reset();
            
            ItemCoinModel fromModel = _model.GetFromCoinModel();
            ItemCoinModel toModel = _model.GetToCoinModel();
            
            _mainPanel.SetFromCoinInfo(fromModel.Icon, fromModel.ShortName);
            _mainPanel.SetToCoinInfo(toModel.Icon, toModel.ShortName);
            
            OpenPanel(_mainPanel);
        }

        private void OpenChooseCoinPanel()
        {
            _chooseCurrencyPanel.PressBtnAction += OnReceiveAnswerOpenChooseCoinPanel;
            
            OpenPanel(_chooseCurrencyPanel);
        }

        private void OnReceiveAnswerOpenChooseCoinPanel(PanelView view, int answer)
        {
            view.PressBtnAction -= OnReceiveAnswerOpenChooseCoinPanel;
            
            OpenPanel(_mainPanel);
            ClosePanel(_chooseCurrencyPanel);
        }

        private void OnReceiveAnswerMainPanel(PanelView view,int answer)
        {
            view.PressBtnAction -= OnReceiveAnswerMainPanel;
            
            switch (answer)
            {
                case 0:
                    _model.SwapCoins();
                    OpenMainPanel();
                    break;
                case 1:
                    SceneLoaderUtility.LoadScene(SceneType.MainScene, this);
                    break;
                case 2:
                    SceneLoaderUtility.LoadScene(SceneType.PriceTrackerScene, this);
                    break;
                case 3:
                    SceneLoaderUtility.LoadScene(SceneType.PortfolioScene, this);
                    break;
                default:
                    SceneLoaderUtility.LoadScene(SceneType.PrivacyPolicyScene, this);
                    break;
            }
        }

        private void OpenPanel(PanelView view)
        {
            view.Open();
        }

        private void ClosePanel(PanelView view)
        {
            view.Close();
        }

        private void OnInputValueChanged(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                _mainPanel.Reset();
                return;
            }

            if (!float.TryParse(input, System.Globalization.NumberStyles.Float, 
                    System.Globalization.CultureInfo.InvariantCulture, out float amount) || amount <= 0f)
            {
                Debug.LogWarning("⚠️ Некорректный ввод количества.");
                return;
            }

            var from = _model.GetFromCoinModel();
            var to = _model.GetToCoinModel();

            if (from == null || to == null)
            {
                Debug.LogWarning("⚠️ Монеты не загружены.");
                return;
            }

            float fromPriceUsd = from.Cost;
            float toPriceUsd = to.Cost;
            
            float toAmount = (amount * fromPriceUsd) / toPriceUsd;
            
            float totalUsd = amount * fromPriceUsd;
            
            _mainPanel.SetToCoinInfo(toAmount, totalUsd);
        }

        private void OnSelectedCoinToChange(bool isFromCoin)
        {
            _model.SetSelectedCoin(isFromCoin);
            
            ClosePanel(_mainPanel);
            OpenChooseCoinPanel();
        }

        private void OnSelectedNewCoin(string id)
        {
            _model.SetNewCoin(id);
            
            OpenMainPanel();
            ClosePanel(_chooseCurrencyPanel);
        }
    }
}