using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoaderView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _fadeDuration = 0.5f; // скорость моргания
    [SerializeField] private float _totalTime = 20f;     // всего моргает 20 сек
    [SerializeField] private float _targetAlpha = 0f;    // до какой альфы исчезает

    private float _startAlpha;
    private Tween _tween;

    private void OnEnable()
    {
        _startAlpha = _image.color.a;
        StartBlink();
    }

    private void OnDisable()
    {
        _tween?.Kill();
    }

    private void StartBlink()
    {
        float cycleDuration = _fadeDuration * 2f;
        
        int loops = Mathf.FloorToInt(_totalTime / cycleDuration);

        _tween = _image
            .DOFade(_targetAlpha, _fadeDuration)
            .SetLoops(loops * 2, LoopType.Yoyo);
    }
}
