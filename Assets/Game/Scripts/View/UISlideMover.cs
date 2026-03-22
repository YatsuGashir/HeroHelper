using UnityEngine;
using DG.Tweening;
using System;

namespace View.UI
{
    public class UISlideMover : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform targetRect; // Объект для анимации (если != null, иначе — этот GameObject)

        [Header("Animation Settings")]
        [SerializeField] private Vector2 startPos = new Vector2(-500, 0);  // Начальная позиция (якорные координаты)
        [SerializeField] private Vector2 endPos = Vector2.zero;            // Конечная позиция
        [SerializeField] private float duration = 0.3f;                    // Длительность анимации
        [SerializeField] private Ease easeType = Ease.OutQuad;             // Кривая анимации
        
        [Header("Optional")]
        [SerializeField] private bool startHidden = true;                  // Скрыть объект в начальной позиции
        [SerializeField] private bool loop = false;                        // Зациклить анимацию
        [SerializeField] private bool pingPong = false;                    // Двигаться туда-обратно

        private RectTransform _rect;
        private Sequence _slideSequence;
        private bool _isActive = false;

        public bool IsActive => _isActive;
        public event Action OnSlideComplete;

        private void Awake()
        {
            _rect = targetRect != null ? targetRect : GetComponent<RectTransform>();
            
            if (startHidden)
            {
                _rect.anchoredPosition = startPos;
                _rect.gameObject.SetActive(false);
            }
        }
        
        public void SlideIn()
        {
            if (_isActive) return;
            _isActive = true;
            
            _rect.gameObject.SetActive(true);
            _rect.anchoredPosition = startPos;
            
            CreateSequence(endPos);
        }

        public void SlideOut()
        {
            if (!_isActive) return;
            _isActive = false;
            
            CreateSequence(startPos);
        }

        public void Toggle()
        {
            if (_isActive) SlideOut();
            else SlideIn();
        }

        public void SetPosition(bool show)
        {
            _rect.gameObject.SetActive(true);
            _rect.anchoredPosition = show ? endPos : startPos;
            _isActive = show;
        }

        private void CreateSequence(Vector2 targetPos)
        {
            _slideSequence?.Kill();

            _slideSequence = DOTween.Sequence();
            
            _slideSequence.Append(
                _rect.DOAnchorPos(targetPos, duration)
                    .SetEase(easeType)
            );
            
            if (loop)
            {
                _slideSequence.SetLoops(-1, pingPong ? LoopType.Yoyo : LoopType.Restart);
            }
            else
            {
                _slideSequence.OnComplete(() => OnSlideComplete?.Invoke());
            }

            _slideSequence.Play();
        }
        
        public void Stop()
        {
            _slideSequence?.Kill();
            _isActive = false;
        }

        private void OnDestroy()
        {
            _slideSequence?.Kill();
            OnSlideComplete = null;
        }

#if UNITY_EDITOR
        [ContextMenu("Slide In")]
        private void Editor_SlideIn() => SlideIn();
        
        [ContextMenu("Slide Out")]
        private void Editor_SlideOut() => SlideOut();
        
        [ContextMenu("Toggle")]
        private void Editor_Toggle() => Toggle();
#endif
    }
}