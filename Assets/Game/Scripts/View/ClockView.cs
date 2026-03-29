using DG.Tweening;
using UnityEngine;

public class ClockView : MonoBehaviour
{
[Header("References")]
    [SerializeField] private Transform _hourHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _secondHand; // Опционально

    [Header("End Turn Settings")]
    [SerializeField] private float _duration = 1f;           // Длительность анимации конца хода
    [SerializeField] private int _rotations = 3;             // Сколько полных оборотов сделать
    [SerializeField] private Ease _ease = Ease.OutBack;      // Плавность (OutBack даёт эффект "протяжки")
    [SerializeField] private bool _shakeOnComplete = true;   // Лёгкая вибрация в конце для акцента
    
    [Header("Shake Settings (if enabled)")]
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeStrength = 2f;
    [SerializeField] private int _shakeVibrato = 10;

    private Sequence _endTurnSequence;
    private bool _isAnimating;

    /// <summary>
    /// Запустить анимацию конца хода: стрелки быстро вращаются несколько раз
    /// </summary>
    /// <param name="onComplete">Коллбэк после завершения анимации</param>
    public void PlayEndTurnAnimation(System.Action onComplete = null)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        StopAnimations();

        float totalAngle = 360f * _rotations;
        
        // Создаём последовательность
        _endTurnSequence = DOTween.Sequence();

        // Минутная стрелка: вращается быстро (основной визуальный акцент)
        if (_minuteHand != null)
        {
            _endTurnSequence.Join(
                _minuteHand.DOLocalRotate(new Vector3(0, 0, -totalAngle), _duration, RotateMode.FastBeyond360)
                    .SetEase(_ease)
            );
        }

        // Часовая стрелка: вращается медленнее (визуальный контраст)
        if (_hourHand != null)
        {
            float hourAngle = totalAngle * 0.3f; // Часовая делает ~30% оборотов минутной
            _endTurnSequence.Join(
                _hourHand.DOLocalRotate(new Vector3(0, 0, -hourAngle), _duration, RotateMode.FastBeyond360)
                    .SetEase(_ease)
            );
        }

        // Секундная стрелка: вращается ещё быстрее для динамики
        if (_secondHand != null)
        {
            _secondHand.transform.position = new Vector3(-135, 39, 0);
            float secondAngle = totalAngle * 2f; // Секундная делает в 2 раза больше оборотов
            _endTurnSequence.Join(
                _secondHand.DOMove(new Vector3(177, 39, 0), 2.5f)
                    .SetEase(Ease.Linear) // Линейно — как настоящий механизм
            );
        }

        // Эффект завершения
        _endTurnSequence.AppendCallback(() =>
        {
            if (_shakeOnComplete)
                TriggerShakeEffect();
            
            _isAnimating = false;
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// Лёгкая вибрация часов в конце анимации для акцента
    /// </summary>
    private void TriggerShakeEffect()
    {
        if (_shakeDuration <= 0f) return;
        
        // Вибрация всего объекта часов
        transform.DOShakePosition(_shakeDuration, _shakeStrength, _shakeVibrato, 90f, false, true)
            .SetEase(Ease.OutQuad);
        
        // Или вибрация только стрелок (альтернатива):
        // if (_minuteHand != null)
        //     _minuteHand.DOShakeRotation(_shakeDuration, new Vector3(0, 0, _shakeStrength), _shakeVibrato)
        //         .SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Мгновенно сбросить стрелки в исходное положение (0°)
    /// </summary>
    public void ResetHands()
    {
        StopAnimations();
        
        if (_hourHand != null) _hourHand.localRotation = Quaternion.identity;
        if (_minuteHand != null) _minuteHand.localRotation = Quaternion.identity;
        if (_secondHand != null) _secondHand.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Остановить все текущие анимации
    /// </summary>
    public void StopAnimations()
    {
        _endTurnSequence?.Kill();
        DOTween.Kill(_hourHand);
        DOTween.Kill(_minuteHand);
        DOTween.Kill(_secondHand);
        DOTween.Kill(transform); // Для shake
        _isAnimating = false;
    }

    /// <summary>
    /// Проверка: идёт ли сейчас анимация
    /// </summary>
    public bool IsAnimating => _isAnimating;

    private void OnDestroy()
    {
        StopAnimations();
    }


}