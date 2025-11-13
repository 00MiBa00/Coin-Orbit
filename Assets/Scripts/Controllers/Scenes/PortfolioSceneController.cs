using UnityEngine;
using Actions;
using Models.General;
using Models.Scenes;
using Types;
using Utility;
using Views.General;
using Views.Portfolio;

namespace Controllers.Scenes
{
    public class PortfolioSceneController : AbstractSceneController
    {
        [SerializeField] private AddCoinPanel _addCoinPanel;
        [SerializeField] private PortfolioMainPanel _mainPanel;
        [SerializeField] private PanelView _confirmPanel;
        
        private PortfolioSceneModel _model;
        
        protected override void OnSceneEnable()
        {
            
        }

        protected override void OnSceneStart()
        {
            
        }

        protected override void OnSceneDisable()
        {
            
        }

        protected override void Initialize()
        {
            base.StartSpinning();
            
            CoinAPIUtility.Initialize(this);
            
            _model = new PortfolioSceneModel();
            
            CoinAPIUtility.EnsureCacheFresh(() =>
            {
                base.StopSpinning();
                OpenMainPanel();
            });
        }

        protected override void Subscribe()
        {
            PortfolioActions.OnSelectedCoinAction += OnSelectedCoin;
            PortfolioActions.OnAmountValueChangedAction += OnAmountCoinChanged;
            _mainPanel.OnCanDeleteItemAction += OnCanDeleteItem;
        }

        protected override void Unsubscribe()
        {
            PortfolioActions.OnSelectedCoinAction -= OnSelectedCoin;
            PortfolioActions.OnAmountValueChangedAction -= OnAmountCoinChanged;
            _mainPanel.OnCanDeleteItemAction -= OnCanDeleteItem;
        }

        private void OnSelectedCoin(ItemCoinModel model)
        {
            _model.AddSaveCoinId(model.Id, model.FullName, model.ShortName);
            _model.AddSaveCoinCost(model.Cost);
            
            _addCoinPanel.SetState(false);
            _addCoinPanel.SetInfoSelectedResult(model.Icon, model.FullName);
        }

        private void OnAmountCoinChanged(float amount)
        {
            _model.AddSaveCoinDataCount(amount);

            _addCoinPanel.SetSumPrice(_model.GetSumPrice());
        }
        
        private void OnReceiveAnswerAddCoinPanel(PanelView view, int answer)
        {
            _addCoinPanel.PressBtnAction -= OnReceiveAnswerAddCoinPanel;

            if (answer == 1)
            {
                _model.SaveCoin();
            }

            OpenMainPanel();
            ClosePanel(_addCoinPanel);
            _addCoinPanel.SetState(true);
        }

        private void OnReceiveAnswerMainPanel(PanelView view, int answer)
        {
            _mainPanel.PressBtnAction -= OnReceiveAnswerMainPanel;
            
            switch (answer)
            {
                case 0:
                    ClosePanel(_mainPanel);
                    OpenAddCoinPanel();
                    break;
                case 1:
                    SceneLoaderUtility.LoadScene(SceneType.MainScene, this);
                    break;
                case 2:
                    SceneLoaderUtility.LoadScene(SceneType.PriceTrackerScene, this);
                    break;
                case 3:
                    SceneLoaderUtility.LoadScene(SceneType.ConverterScene, this);
                    break;
                default:
                    SceneLoaderUtility.LoadScene(SceneType.PrivacyPolicyScene, this);
                    break;
            }
        }

        private void OpenAddCoinPanel()
        {
            _addCoinPanel.PressBtnAction += OnReceiveAnswerAddCoinPanel;
            OpenPanel(_addCoinPanel);
        }

        private void OpenMainPanel()
        {
            _mainPanel.SetCoinsInfo(_model.LoadCoins());
            _mainPanel.SetProfit(_model.GetProfit());
            _mainPanel.UpdateBalance(_model.GetBalance());
            _mainPanel.PressBtnAction += OnReceiveAnswerMainPanel;
            OpenPanel(_mainPanel);
        }

        private void OpenPanel(PanelView view)
        {
            view.Open();
        }

        private void ClosePanel(PanelView view)
        {
            view.Close();
        }
        
        private void OnCanDeleteItem(int index)
        {
            OpenConfirmPanel(index);
        }
        
        private void OpenConfirmPanel(int index)
        {
            _confirmPanel.PressBtnAction += (view, answer) => OnReceiveAnswerConfirmPanel(view, answer, index);
            OpenPanel(_confirmPanel);
        }
        
        private void OnReceiveAnswerConfirmPanel(PanelView view,int answer, int itemIndex)
        {
            _confirmPanel.PressBtnAction = null;
           ClosePanel(_confirmPanel);

            if (answer == 0)
            {
                _model.DeleteCoin(itemIndex);
                _mainPanel.DestroyItem(itemIndex);
                _mainPanel.UpdateBalance(_model.GetBalance());
                _mainPanel.SetProfit(_model.GetProfit());
            }
        }
    }
}