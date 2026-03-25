using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace View
{
    public class CoronationAnimation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform portraitContainer;  // Контейнер с портретом
        [SerializeField] private List<TrumpetAnimation> trumpets;  // Список труб (обычно 2: левая и правая)
        [SerializeField] private RectTransform crownContainer;
        
        [Header("Portrait Animation")]
        [SerializeField] private float slideUpDuration = 0.5f;
        [SerializeField] private Vector2 portraitStartPos = new Vector2(0, -300); // Старт снизу
        [SerializeField] private Vector2 portraitEndPos = Vector2.zero;           // Финиш в центре
        [SerializeField] private Vector2 crownEndPos;
        
        [Header("Frame Animation")]
        [SerializeField] private RectTransform frameToMove;         // Рамка, которая едет в финальную позицию
        [SerializeField] private Vector2 frameTargetPosition;       // Куда приехать в конце
        [SerializeField] private float frameMoveDuration = 0.4f;
        [SerializeField] private float frameMoveDelay = 0.3f;       // Задержка после труб
        
        [Header("Timing")]
        [SerializeField] private float trumpetDelay = 0.1f;         // Задержка между появлением труб
        [SerializeField] private float betweenPhasesDelay = 0.2f;   // Пауза между этапами
        [SerializeField] private float textDisplayDelay = 1.5f;
        [SerializeField] private float crownDelay = 0.2f;

        private Sequence _coronationSequence;
        private bool _isAnimating = false;
        private TextFader textFader;

        public void Init()
        {
            for (int i = 0; i < trumpets.Count; i++)
            {
                if (trumpets[i] != null)
                {
                    trumpets[i].Init();
                }
            }
        }

public async UniTask PlayCoronation(GameObject newPortrait, TextFader fader)
{
    if (_isAnimating) return;
    _isAnimating = true;
    textFader = fader;
    
    // === ПОДГОТОВКА ===
    // Позиция короны: старт сверху за экраном, финиш - в crownEndPos
    if (crownContainer != null)
    {
        crownContainer.anchoredPosition = new Vector2(crownEndPos.x, 400f); // Старт высоко сверху
        crownContainer.gameObject.SetActive(true);
    }

    if (portraitContainer != null && newPortrait != null)
    {
        newPortrait.transform.SetParent(portraitContainer, false);
        var portraitRect = newPortrait.GetComponent<RectTransform>();
        if (portraitRect != null)
        {
            portraitRect.anchorMin = new Vector2(0.5f, 0.5f);
            portraitRect.anchorMax = new Vector2(0.5f, 0.5f);
            portraitRect.pivot = new Vector2(0.5f, 0.5f);
            portraitRect.anchoredPosition = portraitStartPos;
        }
    }

    foreach (var trumpet in trumpets)
    {
        if (trumpet != null) trumpet.gameObject.SetActive(false);
    }
    
    // === ЭТАП 1: Портрет выезжает снизу ===
    if (portraitContainer != null && newPortrait != null)
    {
        var portraitRect = newPortrait.GetComponent<RectTransform>();
        if (portraitRect != null)
        {
            await portraitRect.DOAnchorPos(portraitEndPos, slideUpDuration)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();
        }
    }

    await UniTask.Delay((int)(betweenPhasesDelay * 1000));

    // === ЭТАП 2: Трубы появляются и играют ===
    var trumpetTasks = new List<UniTask>();
    for (int i = 0; i < trumpets.Count; i++)
    {
        if (trumpets[i] != null)
        {
            await UniTask.Delay((int)(trumpetDelay * 1000 * i));
            trumpetTasks.Add(trumpets[i].PlayFullAnimation());
        }
    }
    await UniTask.WhenAll(trumpetTasks);

    await UniTask.Delay((int)(betweenPhasesDelay * 1000));

    // === ЭТАП 3: Корона падает сверху на портрет ===
    if (crownContainer != null)
    {
        await crownContainer.DOAnchorPos(crownEndPos, crownDelay)
            .SetEase(Ease.OutBounce) // Пружинящий эффект при приземлении
            .AsyncWaitForCompletion();
        
        // Небольшая пауза после приземления короны для драматизма
        await UniTask.Delay(200);
    }

    // === ЭТАП 4: Показываем надпись "Да здравствует король!" ===
    if (textFader != null)
    {
        textFader.ShowText("Да здравствует король!");
        await UniTask.Delay((int)(textDisplayDelay * 1000));
        textFader.HideText();
        await UniTask.Delay((int)(betweenPhasesDelay * 1000));
    }
    
    // === ЭТАП 5: Рамка едет в финальную позицию ===
    if (frameToMove != null)
    {
        await frameToMove.DOAnchorPos(frameTargetPosition, frameMoveDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();
    }

    _isAnimating = false;
}

        public void ResetAnimation()
        {
            _coronationSequence?.Kill();
            
            if (portraitContainer != null)
            {
                var portrait = portraitContainer.GetChild(0)?.GetComponent<RectTransform>();
                if (portrait != null) portrait.anchoredPosition = portraitStartPos;
            }
            
            if (frameToMove != null)
            {
                frameToMove.anchoredPosition = frameToMove.anchoredPosition;
            }
            
            foreach (var trumpet in trumpets)
            {
                if (trumpet != null) trumpet.gameObject.SetActive(false);
            }
            
            _isAnimating = false;
        }

        private void OnDestroy()
        {
            _coronationSequence?.Kill();
        }
    }
}