using UnityEngine;

namespace Models.Scenes
{
    public class InitSceneModel
    {
        private const string RegistartionKey = "InitSceneModel.Registartion";

        public bool CanCheckAf => !PlayerPrefs.HasKey(RegistartionKey);

        public string Registration
        {
            get => PlayerPrefs.GetString(RegistartionKey, "");
            set => PlayerPrefs.SetString(RegistartionKey, value);
        }
    }
}