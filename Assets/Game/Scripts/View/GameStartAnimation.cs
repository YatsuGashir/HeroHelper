using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using GlobalSpace;
using View.UI; // Для UISlideMover

public class GameStartAnimation : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup startButtonGroup;    // Кнопка старта (для fade)
    [SerializeField] private SpriteRenderer mainMenuSprite; // Спрайт главного меню
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraTargetPosition;    // Куда приблизить камеру
    [SerializeField] private float cameraZoomDuration = 1f;
    [SerializeField] private float targetSize = 3f;
    [SerializeField] private Ease cameraEase = Ease.OutQuad;
    
    [Header("Slide Animations")]
    [SerializeField] private List<UISlideMover> slideElements = new List<UISlideMover>();
    [SerializeField] private float slideDelayBetween = 0.15f;  // Задержка между появлениями
    [SerializeField] private float startDelay = 0.3f;          // Задержка перед началом слайдов
    
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    
    private bool _isAnimating = false;

    public async UniTask PlayStartSequence()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        await G.GameManager.StartNewRun();
        

        if (startButtonGroup != null)
        {
            await startButtonGroup.DOFade(0f, fadeDuration)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
            
            startButtonGroup.interactable = false;
        }
        
        UniTask cameraTask = UniTask.CompletedTask;
        if (mainCamera != null)
        {
            cameraTask = mainCamera.transform.DOMove(cameraTargetPosition, cameraZoomDuration)
                .SetEase(cameraEase)
                .AsyncWaitForCompletion();
            mainCamera.DOOrthoSize(targetSize, cameraZoomDuration)
                .SetEase(cameraEase);
        }
            
        UniTask menuFadeTask = UniTask.CompletedTask;
        if (mainMenuSprite != null)
        {
            menuFadeTask = mainMenuSprite.DOFade(0f, fadeDuration)
                .SetEase(Ease.InOutQuad)
                .AsyncWaitForCompletion();
        }
        
        await UniTask.WhenAll(cameraTask, menuFadeTask);


        await UniTask.Delay((int)(startDelay * 1000));
        
        for (int i = 0; i < slideElements.Count; i++)
        {
            if (slideElements[i] != null)
            {
                if (i > 0)
                    await UniTask.Delay((int)(slideDelayBetween * 1000));
                
                slideElements[i].SlideIn();
            }
        }
        
        // === ЭТАП 4: Ждём завершения всех слайдов (опционально) ===
        // Если нужно ждать завершения анимаций перед продолжением:
        await UniTask.Delay((int)((slideElements.Count * slideDelayBetween + 0.5f) * 1000));
        
        _isAnimating = false;

        //await G.GameManager.StartNewRun();
    }
    
    /// <summary>
    /// Быстрый вызов для кнопки (через UnityEvent)
    /// </summary>
    public async void OnStartButtonClick()
    {
        await PlayStartSequence();
    }
    

    public void ResetSequence()
    {
        DOTween.KillAll();
        
        if (startButtonGroup != null)
        {
            startButtonGroup.alpha = 1f;
            startButtonGroup.interactable = true;
        }
        

            
        if (mainCamera != null)
            mainCamera.transform.position = cameraTargetPosition; // Или сохраните стартовую позицию отдельно
            
        foreach (var slider in slideElements)
        {
            if (slider != null)
                slider.SetPosition(false); // Скрыть в начальную позицию
        }
        
        _isAnimating = false;
    }
    
    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}