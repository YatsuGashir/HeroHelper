using DG.Tweening;
using UnityEngine;

/// <summary>
/// Анимация дыхания для UI-элементов (использует anchoredPosition и localScale)
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class BreathAnimation : MonoBehaviour
{
    [Header("=== Scale Settings ===")]
    [Min(1f)]
    [SerializeField] private float breathScale = 1.08f;
    [Min(0.1f)]
    [SerializeField] private float scaleDuration = 1.5f;
    [SerializeField] private Ease scaleEase = Ease.InOutSine;

    [Header("=== Position Settings (опционально) ===")]
    [Tooltip("Амплитуда 'парения' по Y в пикселях")]
    [SerializeField] private float floatAmplitude = 5f;
    [Min(0.1f)]
    [SerializeField] private float floatDuration = 2f;
    [SerializeField] private Ease floatEase = Ease.InOutSine;

    [Header("=== Control ===")]
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool loop = true;
    [Min(0f)]
    [SerializeField] private float startDelay = 0f;

    private RectTransform _rectTransform;
    private Vector2 _originalAnchoredPosition;
    private Vector3 _originalScale;
    
    private Sequence _breathSequence;
    private bool _isPlaying = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
        _originalAnchoredPosition = _rectTransform.anchoredPosition;
        
        if (playOnAwake)
            Play();
    }

    public void Play()
    {
        if (_isPlaying) return;
        
        _breathSequence?.Kill();
        _rectTransform.localScale = _originalScale;
        _rectTransform.anchoredPosition = _originalAnchoredPosition;
        
        _breathSequence = DOTween.Sequence();
        
        // === Дыхание: масштаб ===
        _breathSequence.Append(
            _rectTransform.DOScale(_originalScale * breathScale, scaleDuration * 0.5f)
                .SetEase(scaleEase)
        );
        _breathSequence.Append(
            _rectTransform.DOScale(_originalScale, scaleDuration * 0.5f)
                .SetEase(scaleEase)
        );
        
        // === Парение: позиция (опционально) ===
        if (floatAmplitude > 0f)
        {
            _breathSequence.Join(
                _rectTransform.DOAnchorPosY(
                    _originalAnchoredPosition.y + floatAmplitude,
                    floatDuration * 0.5f
                ).SetEase(floatEase).SetLoops(-1, LoopType.Yoyo)
            );
        }
        
        _breathSequence.SetLoops(loop ? -1 : 1, LoopType.Restart);
        _breathSequence.SetDelay(startDelay);
        
        _isPlaying = true;
    }

    public void Pause() => _breathSequence?.Pause();
    public void Resume() => _breathSequence?.Play();
    
    public void Stop()
    {
        _breathSequence?.Kill();
        _rectTransform.localScale = _originalScale;
        _rectTransform.anchoredPosition = _originalAnchoredPosition;
        _isPlaying = false;
    }

    public void SetScaleProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        float scale = Mathf.Lerp(1f, breathScale, Mathf.PingPong(progress * 2f, 1f));
        _rectTransform.localScale = _originalScale * scale;
    }

    private void OnDestroy() => _breathSequence?.Kill();

#if UNITY_EDITOR
    private void OnValidate()
    {
        breathScale = Mathf.Max(1f, breathScale);
        scaleDuration = Mathf.Max(0.1f, scaleDuration);
    }
#endif
}