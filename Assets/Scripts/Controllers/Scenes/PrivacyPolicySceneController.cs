using Types;
using UnityEngine;
using Utility;
using Views.General;
using Views.PrivacyPolicy;

namespace Controllers.Scenes
{
    public class PrivacyPolicySceneController : AbstractSceneController
    {
        [SerializeField] private MainPanel _mainPanel;

        protected override void OnSceneEnable()
        {
            OpenMainPanel();
        }

        protected override void OnSceneStart()
        {

        }

        protected override void OnSceneDisable()
        {

        }

        protected override void Initialize()
        {

        }

        protected override void Subscribe()
        {

        }

        protected override void Unsubscribe()
        {

        }

        private void OpenMainPanel()
        {
            _mainPanel.PressBtnAction += OnReceiveAnswerMainPanel;

            OpenPanel(_mainPanel);
        }

        private void OpenPanel(PanelView view)
        {
            view.Open();
        }

        private void OnReceiveAnswerMainPanel(PanelView view, int answer)
        {
            _mainPanel.PressBtnAction -= OnReceiveAnswerMainPanel;

            switch (answer)
            {
                case 0:
                    SceneLoaderUtility.LoadScene(SceneType.MainScene, this);
                    break;
                case 1:
                    SceneLoaderUtility.LoadScene(SceneType.PriceTrackerScene, this);
                    break;
                case 2:
                    SceneLoaderUtility.LoadScene(SceneType.PortfolioScene, this);
                    break;
                case 3:
                    SceneLoaderUtility.LoadScene(SceneType.ConverterScene, this);
                    break;
            }
        }
    }
}