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
    
    private const string TutorialCompleteKey = "TUTORIAL_COMPLETE";

    private void Awake()
    {
        G.TutorialSeq = this;

        tutorialComplete = false;
        //tutorialComplete = PlayerPrefs.GetInt(TutorialCompleteKey, 0) == 1;

        tutorialCenterText.gameObject.SetActive(false);
    }
    public async void nStartButtonClick()
    {
        if (tutorialComplete)
        {
            gameStartAnimation.PlayStartSequence();
            return;
        }

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

        var waitForAction = UniTask.WaitUntil(() => placeFirstBuilding);
        var timeout = UniTask.Delay(TimeSpan.FromSeconds(5));

        var completed = await UniTask.WhenAny(waitForAction, timeout);

        if (completed == 1) // таймаут
        {
            tutorialCenterText.gameObject.SetActive(true);
            await Say("не спеши… выбери карту и размести её на поле");
            tutorialCenterText.gameObject.SetActive(false);
        }
        
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

        CompleteTutorial();
        
        tutorialCenterText.gameObject.SetActive(false);


    }
    
    private void CompleteTutorial()
    {
        tutorialComplete = true;

        PlayerPrefs.SetInt(TutorialCompleteKey, 1);
        PlayerPrefs.Save();

        Debug.Log("Tutorial completed and saved");
    }
    
    public async UniTask Say( string text)
    {
        await G.TextController.PlayTextAsync(tutorialCenterText, text);
    }
}
