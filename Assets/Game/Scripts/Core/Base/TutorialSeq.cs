using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GlobalSpace;
using TMPro;
using UnityEngine;
using View.UI;

public class TutorialSeq: MonoBehaviour
{
    [SerializeField] private GameStartAnimation gameStartAnimation;
    
    [SerializeField] private TextMeshProUGUI tutorialCenterText;
    
    [SerializeField] private List<UISlideMover> slideElements = new List<UISlideMover>();
    
    [HideInInspector] public bool placeFirstBuilding = false;
    [HideInInspector] public bool turnPushing = false;
    
    [HideInInspector] public bool tutorialComplete = false;

    private void Awake()
    {
        G.TutorialSeq = this;
        tutorialCenterText.gameObject.SetActive(false);
    }
    public async void nStartButtonClick()
    {
        await TryStartTutorial();
    }

    public async UniTask TryStartTutorial()
    {
        
        await gameStartAnimation.PlayStartSequence();
        slideElements[0].SlideIn();
        tutorialCenterText.gameObject.SetActive(true);
        await Say("Привет");
        await Say("Чё как");
        slideElements[1].SlideIn();
        await Say("Это здания");
        await Say("Разыграй их");
        tutorialCenterText.gameObject.SetActive(false);
        await UniTask.WaitUntil(() => placeFirstBuilding);
        tutorialCenterText.gameObject.SetActive(true);
        await Say("Отлично");
        slideElements[2].SlideIn();
        await Say("это твои ресурсы");
        await Say("каждый ход твои постройки дают ресурсы");
        slideElements[3].SlideIn();
        await Say("Заверши ход, чтобы получить их");
        tutorialCenterText.gameObject.SetActive(false);
        await UniTask.WaitUntil(() =>  turnPushing);
        tutorialCenterText.gameObject.SetActive(true);
        await Say("Жизнь коротка, нужно будет выбрать наследников");
        slideElements[4].SlideIn();
        await Say("Моя жизнь подходит к концу. выбери нового наследника");
        tutorialCenterText.gameObject.SetActive(false);


    }
    
    public async UniTask Say( string text)
    {
        await G.TextController.PlayTextAsync(tutorialCenterText, text);
    }
}
