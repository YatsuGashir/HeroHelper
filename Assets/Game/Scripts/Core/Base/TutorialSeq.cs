using System;
using Cysharp.Threading.Tasks;
using GlobalSpace;
using TMPro;
using UnityEngine;

public class TutorialSeq: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialCenterText;
    
    [HideInInspector] public bool  SkipStartScreen = false;

    private void Awake()
    {
        G.TutorialSeq = this;
    }

    public async UniTask TryStartTutorial()
    {
        await Say("You have different coins in your hand and in your wallet");
        //await UniTask.WaitUntil(() => coinPlayed);
    }
    
    public async UniTask Say( string text)
    {
        await G.TextController.PlayTextAsync(tutorialCenterText, text);
    }
}
