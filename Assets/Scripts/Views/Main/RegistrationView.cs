using UnityEngine;
using Utility;

namespace Views.Main
{
    public class RegistrationView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        
        private UniWebView _webView;
        
        public void SetRegistration()
        {
            _webView = gameObject.AddComponent<UniWebView>();
            _webView.EmbeddedToolbar.Hide();
            _webView.ReferenceRectTransform = _container;
            _webView.SetSupportMultipleWindows(false, true);
            _webView.SetAllowFileAccess(true);
            _webView.SetCalloutEnabled(true);
            _webView.SetBackButtonEnabled(true);
            _webView.OnShouldClose += webView => false;
            _webView.OnPageFinished += (view, code, url) => OnLoaded();
            _webView.SetAllowBackForwardNavigationGestures(true);
            
            _webView.Load(RegistrationUtility.RegistrationData);
        }

        private void OnLoaded()
        {
            _webView.Show();
        }
    }
}