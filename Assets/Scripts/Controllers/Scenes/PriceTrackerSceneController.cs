using UnityEngine;
using Views.General;
using Views.PriceTracker;

using Utility;
using Models.Scenes;
using Types;

namespace Controllers.Scenes
{
    public class PriceTrackerSceneController : AbstractSceneController
    {
        [SerializeField] private ChooseCurrencyPanel _chooseCurrencyPanel;
        [SerializeField] private MainPanel _mainPanel;
        [SerializeField] private PanelView _confirmPanel;

        private PriceTrackerSceneModel _model;

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

            _model = new PriceTrackerSceneModel();
        }

        protected override void Subscribe()
        {
            _mainPanel.OnCanDeleteItemAction += OnCanDeleteItem;
        }

        protected override void Unsubscribe()
        {
            _mainPanel.OnCanDeleteItemAction -= OnCanDeleteItem;
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
            if (_model.canAddPriceTrackerCoinsItems)
            {
                _mainPanel.SetCoinsInfo(_model.LoadSavedCoinModels());
            }

            OpenPanel(_mainPanel, true);
        }

        private void OpenPanel(PanelView view, bool isMain)
        {
            view.PressBtnAction += (panelView, i) => OnReceiveAnswerPanel(panelView, i, isMain);
            view.Open();
        }

        private void ClosePanel(PanelView view)
        {
            view.Close();
        }

        private void OnReceiveAnswerPanel(PanelView view, int answer, bool isMainPanel)
        {
            view.PressBtnAction -= (panelView, i) => OnReceiveAnswerPanel(panelView, i, isMainPanel);

            if (isMainPanel)
            {
                switch (answer)
                {
                    case 0:
                        ClosePanel(_mainPanel);
                        _chooseCurrencyPanel.OnSelectedCoinAction += OnSelectedCoin;
                        OpenPanel(_chooseCurrencyPanel, false);
                        break;
                    case 1:
                        SceneLoaderUtility.LoadScene(SceneType.MainScene, this);
                        break;
                    case 2:
                        SceneLoaderUtility.LoadScene(SceneType.PortfolioScene, this);
                        break;
                    case 3:
                        SceneLoaderUtility.LoadScene(SceneType.ConverterScene, this);
                        break;
                    default:
                        SceneLoaderUtility.LoadScene(SceneType.PrivacyPolicyScene, this);
                        break;
                }
            }
            else
            {
                ClosePanel(_chooseCurrencyPanel);
                OpenMainPanel();
            }
        }

        private void OnSelectedCoin(string id)
        {
            _chooseCurrencyPanel.OnSelectedCoinAction -= OnSelectedCoin;
            
            _model.SaveCoins(id);
            
            ClosePanel(_chooseCurrencyPanel);
            OpenMainPanel();
        }

        private void OnCanDeleteItem(int index)
        {
            OpenConfirmPanel(index);
        }

        private void OpenConfirmPanel(int index)
        {
            _confirmPanel.PressBtnAction += (view, answer) => OnReceiveAnswerConfirmPanel(view,answer, index);
            _confirmPanel.gameObject.SetActive(true);
        }

        private void OnReceiveAnswerConfirmPanel(PanelView view,int answer, int itemIndex)
        {
            _confirmPanel.PressBtnAction = null;
            _confirmPanel.gameObject.SetActive(false);

            if (answer == 0)
            {
                _model.DeleteActiveItem(itemIndex);
                _mainPanel.DestroyItem(itemIndex);
            }
        }
    }
}
