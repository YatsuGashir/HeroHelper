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
        
        await Say("Внемли… я — Калоплака, первый король");

        await Say("власть наша кратка… но не должна быть глупа");

        slideElements[1].SlideIn();

        await Say("здания — твоя опора");

        await Say("выбери и размести их на земле");

        tutorialCenterText.gameObject.SetActive(false);
        await UniTask.WaitUntil(() => placeFirstBuilding);

        tutorialCenterText.gameObject.SetActive(true);

        await Say("хорошо… ты начинаешь понимать");

        slideElements[2].SlideIn();

        await Say("это — ресурсы");

        await Say("каждый ход постройки приносят их");

        slideElements[3].SlideIn();

        await Say("заверши ход");

        tutorialCenterText.gameObject.SetActive(false);
        await UniTask.WaitUntil(() =>  turnPushing);

        tutorialCenterText.gameObject.SetActive(true);

        await Say("видишь… всё движется");

        await Say("но ничто не длится вечно");

        slideElements[4].SlideIn();

        await Say("наша жизнь коротка");

        await Say("скоро тебе предстоит выбрать наследника");

        await Say("каждый из них изменит ход событий");

        await Say("выбирай… с умом, а пока я ещё не умер, просто продолжай ходить дальше");

        tutorialCenterText.gameObject.SetActive(false);


    }
    
    public async UniTask Say( string text)
    {
        await G.TextController.PlayTextAsync(tutorialCenterText, text);
    }
}
