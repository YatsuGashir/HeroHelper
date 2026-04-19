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
        
        await Say("А сейчас, ваше величество, я обязан зачитать вам послание от Первого Короля!");

        await Say("Внемли, спора! Аз есмь Калаплака, князь первейший!");

        await Say("Правление наше скоротечно. Но не подобно ему быть бездумным");

        slideElements[1].SlideIn();

        await Say("Вотчина твоя – есть власть твоя");

        await Say("Дабы узреть подробности, укажи на вотчину свою");

        await Say("Избери и размести их на свои земли");

        tutorialCenterText.gameObject.SetActive(false);

        var waitForAction = UniTask.WaitUntil(() => placeFirstBuilding);
        var timeout = UniTask.Delay(TimeSpan.FromSeconds(5));

        var completed = await UniTask.WhenAny(waitForAction, timeout);

        if (completed == 1) // таймаут
        {
            tutorialCenterText.gameObject.SetActive(true);
            await Say("Не спеши. Возьми фишку и укажи место");
            tutorialCenterText.gameObject.SetActive(false);
        }
        
        await UniTask.WaitUntil(() => placeFirstBuilding);

        tutorialCenterText.gameObject.SetActive(true);

        await Say("Добро… Разумение в тебе зачинается");

        slideElements[2].SlideIn();

        await Say("Сие твои богатства");

        await Say("Каждый ход вотчина несёт их и забирает");

        slideElements[3].SlideIn();

        await Say("Окончи ход");

        tutorialCenterText.gameObject.SetActive(false);
        await UniTask.WaitUntil(() =>  turnPushing);

        tutorialCenterText.gameObject.SetActive(true);

        await Say("Узри. Всё движется");

        await Say("но ничто не пребывает вечно");

        slideElements[4].SlideIn();

        await Say("житие наше краткое.");

        await Say("Вскоре надлежит тебе указати избранника на наследие");

        await Say("Каждый из них оборотит бытие люда простого");

        await Say("Доколе не слёг ещё, правь мудро и по правде");

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
