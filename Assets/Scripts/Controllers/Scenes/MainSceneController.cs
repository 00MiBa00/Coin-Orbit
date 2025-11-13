using Models.Scenes;
using Types;
using UnityEngine;
using Utility;
using Views.General;
using Views.Main;

namespace Controllers.Scenes
{
    public class MainSceneController : AbstractSceneController
    {
        [SerializeField] private MainPanel _mainPanel;
        
        private MainSceneModel _model;
        
        protected override void OnSceneEnable()
        {
            base.StartSpinning();
            
            LoadCoins();
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
            
            _model = new MainSceneModel();
        }

        protected override void Subscribe()
        {
            
        }

        protected override void Unsubscribe()
        {
            
        }

        private void LoadCoins()
        {
            _model.LoadTopCoins(models =>
            {
                base.StopSpinning();
                _mainPanel.SetInfo(models, RegistrationUtility.CanShowRegistration);
                OpenMainPanel();
            });
        }

        private void OpenMainPanel()
        {
            _mainPanel.PressBtnAction += OnReceiveAnswerPanel;
            _mainPanel.Open();
        }

        private void OnReceiveAnswerPanel(PanelView view, int answer)
        {
            _mainPanel.PressBtnAction -= OnReceiveAnswerPanel;

            switch (answer)
            {
                case 0:
                    SceneLoaderUtility.LoadScene(SceneType.PriceTrackerScene, this);
                    break;
                case 1:
                    SceneLoaderUtility.LoadScene(SceneType.ConverterScene, this);
                    break;
                case 2:
                    SceneLoaderUtility.LoadScene(SceneType.PortfolioScene, this);
                    break;
                default:
                    SceneLoaderUtility.LoadScene(SceneType.PrivacyPolicyScene, this);
                    break;
            }
        }
    }
}