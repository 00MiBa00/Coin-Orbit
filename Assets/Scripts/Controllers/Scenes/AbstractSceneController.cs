using DG.Tweening;
using UnityEngine;

namespace Controllers.Scenes
{
    public abstract class AbstractSceneController : MonoBehaviour
    {
        [SerializeField] private RectTransform _spinner;

        private Tween _spinTween;

        private void OnEnable()
        {
            Initialize();
            Subscribe();
            OnSceneEnable();
        }

        private void Start()
        {
            OnSceneStart();
        }

        private void OnDisable()
        {
            Unsubscribe();
            OnSceneDisable();
        }

        protected abstract void OnSceneEnable();
        protected abstract void OnSceneStart();
        protected abstract void OnSceneDisable();
        protected abstract void Initialize();
        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        
        protected void StartSpinning()
        {
            if (_spinner == null) return;
            
            Debug.Log("Start");
            
            _spinTween = _spinner
                .DORotate(new Vector3(0, 0, -360), 1.0f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        protected void StopSpinning()
        {
            if (_spinTween != null && _spinTween.IsActive())
            {
                _spinTween.Kill();
                _spinner.rotation = Quaternion.identity;
                _spinner.gameObject.SetActive(false);
            }
        }
    }
}