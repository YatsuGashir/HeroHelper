using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using GlobalSpace;
using View.UI; // Для UISlideMover

public class GameStartAnimation : MonoBehaviour
{
    public bool SkipStartScreen = false;
    
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup startButtonGroup;
    [SerializeField] private SpriteRenderer mainMenuSprite;
    [SerializeField] private Canvas startScreenCanvas;
    
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraTargetPosition;
    [SerializeField] private float cameraZoomDuration = 1f;
    [SerializeField] private float targetSize = 3f;
    [SerializeField] private Ease cameraEase = Ease.OutQuad;
    
    [Header("Slide Animations")]
    [SerializeField] private List<UISlideMover> slideElements = new List<UISlideMover>();
    [SerializeField] private float slideDelayBetween = 0.15f;
    [SerializeField] private float startDelay = 0.3f;
    
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    
    private bool _isAnimating = false;

    public async UniTask PlayStartSequence()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        await G.GameManager.StartNewRun();
        AudioManager.Instance.PlaySFX("Scope", 2f);
        if (SkipStartScreen) return;

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
        
        startScreenCanvas.gameObject.SetActive(false);
        if(!G.TutorialSeq.tutorialComplete)return;
        
        for (int i = 0; i < slideElements.Count; i++)
        {
            if (slideElements[i] != null)
            {
                if (i > 0)
                    await UniTask.Delay((int)(slideDelayBetween * 1000));
                
                slideElements[i].SlideIn();
            }
        }

        await UniTask.Delay((int)((slideElements.Count * slideDelayBetween + 0.5f) * 1000));
        
        _isAnimating = false;
        startScreenCanvas.gameObject.SetActive(false);
        //await G.GameManager.StartNewRun();
    }

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
            mainCamera.transform.position = cameraTargetPosition;
            
        foreach (var slider in slideElements)
        {
            if (slider != null)
                slider.SetPosition(false);
        }
        
        _isAnimating = false;
    }
    
    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}