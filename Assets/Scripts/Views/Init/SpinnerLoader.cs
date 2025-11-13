using DG.Tweening;
using UnityEngine;

namespace Views.Init
{
    public class SpinnerLoader : MonoBehaviour
    {
        [SerializeField] private RectTransform _spinner;

        private Tween _spinTween;

        private void Start()
        {
            StartSpinning();
        }

        private void OnDestroy()
        {
            StopSpinning();
        }

        private void StartSpinning()
        {
            if (_spinner == null) return;
            
            _spinTween = _spinner
                .DORotate(new Vector3(0, 0, -360), 1.0f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        private void StopSpinning()
        {
            if (_spinTween != null && _spinTween.IsActive())
            {
                _spinTween.Kill();
                _spinner.rotation = Quaternion.identity;
            }
        }
    }
}