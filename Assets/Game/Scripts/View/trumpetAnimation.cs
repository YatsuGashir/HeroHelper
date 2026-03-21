using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

namespace View
{
    public class TrumpetAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float appearDuration = 0.3f;
        [SerializeField] private float pulseDuration = 0.15f;
        [SerializeField] private int pulseCount = 4;
        [SerializeField] private Vector2 startScale = Vector2.one;
        [SerializeField] private Vector2 pulseScale = new Vector2(1.1f, 0.9f);
        [SerializeField] private float disappearDuration = 0.25f;
        
        [Header("Offsets")]
        [SerializeField] private Vector2 startOffset = new Vector2(-100, -100); // Откуда выезжает
        [SerializeField] private Vector2 endOffset = new Vector2(20, 20);        // Позиция "игры"
        
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        public async UniTask PlayFullAnimation()
        {
            _canvasGroup.alpha = 0f;
            _rectTransform.anchoredPosition = startOffset;
            _rectTransform.localScale = startScale;
            gameObject.SetActive(true);

            await DOTween.Sequence()
                .Join(_rectTransform.DOAnchorPos(endOffset, appearDuration).SetEase(Ease.OutBack))
                .Join(_canvasGroup.DOFade(1f, appearDuration))
                .AsyncWaitForCompletion();

            for (int i = 0; i < pulseCount; i++)
            {
                await _rectTransform.DOScale(pulseScale, pulseDuration)
                    .SetEase(Ease.InOutSine)
                    .AsyncWaitForCompletion();
                await _rectTransform.DOScale(startScale, pulseDuration)
                    .SetEase(Ease.InOutSine)
                    .AsyncWaitForCompletion();
            }
            
            await DOTween.Sequence()
                .Join(_rectTransform.DOAnchorPos(startOffset, disappearDuration).SetEase(Ease.InBack))
                .Join(_canvasGroup.DOFade(0f, disappearDuration))
                .AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }

        public async UniTask Appear()
        {
            _canvasGroup.alpha = 0f;
            _rectTransform.anchoredPosition = startOffset;
            gameObject.SetActive(true);

            await DOTween.Sequence()
                .Join(_rectTransform.DOAnchorPos(endOffset, appearDuration).SetEase(Ease.OutBack))
                .Join(_canvasGroup.DOFade(1f, appearDuration))
                .AsyncWaitForCompletion();
        }

        public async UniTask Pulse(int count = -1)
        {
            int pulses = count > 0 ? count : pulseCount;
            for (int i = 0; i < pulses; i++)
            {
                await _rectTransform.DOScale(pulseScale, pulseDuration)
                    .SetEase(Ease.InOutSine)
                    .AsyncWaitForCompletion();
                await _rectTransform.DOScale(startScale, pulseDuration)
                    .SetEase(Ease.InOutSine)
                    .AsyncWaitForCompletion();
            }
        }

        public async UniTask Disappear()
        {
            await DOTween.Sequence()
                .Join(_rectTransform.DOAnchorPos(startOffset, disappearDuration).SetEase(Ease.InBack))
                .Join(_canvasGroup.DOFade(0f, disappearDuration))
                .AsyncWaitForCompletion();
            
            gameObject.SetActive(false);
        }
    }
}