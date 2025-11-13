using UnityEngine;

namespace Utility
{
    public static class RegistrationUtility
    {
        private const string RegistrationKey = "RegistrationUtility.Registration";
        private const string RegistrationDataKey = "RegistrationUtility.RegistrationData";
        
        public static bool CanShowRegistration
        {
            get => PlayerPrefs.GetInt(RegistrationKey, 1) == 0;
            set => PlayerPrefs.SetInt(RegistrationKey, value ? 0 : 1);
        }
        
        public static string RegistrationData
        {
            get => PlayerPrefs.GetString(RegistrationDataKey, "");
            set => PlayerPrefs.SetString(RegistrationDataKey, value);
        }
    }
}