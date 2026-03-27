using System;
using DG.Tweening;
using UnityEngine;

public class NameAnimation : MonoBehaviour
{
 [Header("Animation Settings")]
    [SerializeField] private float rotationAngle = 5f;      // Амплитуда покачивания в градусах
    [SerializeField] private float duration = 1f;           // Время одного покачивания (туда-обратно)
    [SerializeField] private Ease easeType = Ease.InOutSine; // Плавность анимации
    [SerializeField] private bool startAutomatically = true; // Запускать ли анимацию при старте
    [SerializeField] private bool loop = true;              // Зациклить анимацию
    
    private RectTransform _rectTransform;
    private Sequence  _slideSequence;
    private bool _isAnimating = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (startAutomatically)
        {
            StartAnimation();
        }
    }

    /// <summary>
    /// Запускает анимацию покачивания вокруг центра
    /// </summary>
    public void StartAnimation()
    {
        if (_isAnimating || _rectTransform == null) return;
        
        _isAnimating = true;
        
        // Создаём последовательность: поворот вправо → влево → возврат
        
        _slideSequence?.Kill();
        _slideSequence = DOTween.Sequence();
    
        float segmentDuration = duration / 3f; // Делим общее время на 3 равных части
    
        _slideSequence
            .Append(_rectTransform.DORotate(new Vector3(0, 0, rotationAngle), segmentDuration)
                .SetEase(easeType))
            .Append(_rectTransform.DORotate(new Vector3(0, 0, -rotationAngle), segmentDuration)
                .SetEase(easeType))
            .Append(_rectTransform.DORotate(Vector3.zero, segmentDuration)
                .SetEase(easeType));
    
        if (loop)
            _slideSequence.SetLoops(-1, LoopType.Restart);
    
        _slideSequence.Play();
    }
    
    private void CreateSequence()
    {
        _slideSequence?.Kill();
        _slideSequence = DOTween.Sequence();
    
        float segmentDuration = duration / 3f; // Делим общее время на 3 равных части
    
        _slideSequence
            .Append(_rectTransform.DORotate(new Vector3(0, 0, rotationAngle), segmentDuration)
                .SetEase(easeType))
            .Append(_rectTransform.DORotate(new Vector3(0, 0, -rotationAngle), segmentDuration)
                .SetEase(easeType))
            .Append(_rectTransform.DORotate(Vector3.zero, segmentDuration)
                .SetEase(easeType));
    
        if (loop)
            _slideSequence.SetLoops(-1, LoopType.Restart);
    
        _slideSequence.Play();
    }

    /// <summary>
    /// Останавливает анимацию и возвращает объект в исходное положение
    /// </summary>
    public void StopAnimation()
    {
        _slideSequence?.Kill();
        _isAnimating = false;
        
        // Мгновенный сброс вращения
        if (_rectTransform != null)
        {
            _rectTransform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Переключает состояние анимации
    /// </summary>
    public void ToggleAnimation()
    {
        if (_isAnimating)
            StopAnimation();
        else
            StartAnimation();
    }

    /// <summary>
    /// Плавная остановка с возвратом в ноль
    /// </summary>
    public void SoftStop()
    {
        _slideSequence?.Kill();
        _isAnimating = false;
        
        // Плавный возврат в исходное положение
        if (_rectTransform != null)
        {
            _rectTransform.DORotate(Vector3.zero, duration / 4)
                .SetEase(Ease.OutQuad);
        }
    }

    private void OnDestroy()
    {
        // Очистка твинов при уничтожении объекта
        _slideSequence?.Kill();
        DOTween.Kill(_rectTransform);
    }

#if UNITY_EDITOR
    // Контекстные меню для быстрого тестирования в редакторе
    [ContextMenu("Start Animation")]
    private void Editor_StartAnimation() => StartAnimation();
    
    [ContextMenu("Stop Animation")]
    private void Editor_StopAnimation() => StopAnimation();
    
    [ContextMenu("Toggle Animation")]
    private void Editor_ToggleAnimation() => ToggleAnimation();
#endif
}
