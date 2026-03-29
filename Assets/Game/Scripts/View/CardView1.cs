using System;
using Data;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace View
{
    public class CardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _hoverScale = 1.1f;
        [SerializeField] private float _selectLiftY = 40f;

        public BuildingDefinition CardDefinition { get; private set; }
        
        private readonly Subject<CardView> _onCardClick = new Subject<CardView>();
        public IObservable<CardView> OnCellClick => _onCardClick;

        private Image _cardImage;
        private RectTransform _rectTransform; // 🔽 Добавляем ссылку на RectTransform
        
        private Vector2 _originalAnchoredPosition; // 🔽 Храним позицию как Vector2
        private Vector3 _originalScale;
        private bool _isSelected;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>(); // 🔽 Получаем RectTransform
            _originalScale = transform.localScale;
            
            // 🔽 Сохраняем исходную позицию в системе координат UI
            if (_rectTransform != null)
                _originalAnchoredPosition = _rectTransform.anchoredPosition;
            
            _cardImage = GetComponent<Image>();
            
            if (_cardImage == null)
                _cardImage = GetComponentInChildren<Image>();

            if (_cardImage != null)
                _cardImage.raycastTarget = true;
        }
        
        public void Init(BuildingDefinition card)
        {
            CardDefinition = card;
            
            if (_cardImage != null && card != null)
            {
                _cardImage.sprite = card.buildingCardICon;
                _cardImage.color = Color.white;
            }
            
            // 🔽 Сбрасываем позицию через anchoredPosition
            if (_rectTransform != null)
                _rectTransform.anchoredPosition = _originalAnchoredPosition;
                
            SetSelected(false, false);
        }
        
        public void SetSelected(bool selected, bool animate = true)
        {
            if (_isSelected == selected) return;
            _isSelected = selected;

            if (animate && _rectTransform != null)
            {
                transform.DOKill(); // Останавливаем все твины на трансформе
                
                // 🔽 Рассчитываем целевую позицию для UI
                var targetAnchoredPosition = selected 
                    ? _originalAnchoredPosition + Vector2.up * _selectLiftY 
                    : _originalAnchoredPosition;
                    
                var targetScale = selected 
                    ? _originalScale * _hoverScale * 1.05f 
                    : _originalScale;
                
                // 🔽 Используем DOAnchorPosY для UI!
                _rectTransform.DOAnchorPosY(targetAnchoredPosition.y, 0.2f)
                    .SetEase(Ease.OutBack);
                    
                transform.DOScale(targetScale, 0.2f)
                    .SetEase(Ease.OutBack);
            }
            else
            {
                // 🔽 Мгновенное применение
                if (_rectTransform != null)
                {
                    _rectTransform.anchoredPosition = selected 
                        ? _originalAnchoredPosition + Vector2.up * _selectLiftY 
                        : _originalAnchoredPosition;
                }
                    
                transform.localScale = selected 
                    ? _originalScale * _hoverScale * 1.05f 
                    : _originalScale;
            }

            if (_cardImage != null)
            {
                var targetColor = selected ? new Color(1.2f, 1.2f, 1.2f, 1) : Color.white;
                _cardImage.DOColor(targetColor, 0.2f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX("figureHand", 0.25f);
            _onCardClick.OnNext(this);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_cardImage == null || _isSelected) return;

            transform.DOKill();
            transform.DOScale(_originalScale * _hoverScale, 0.15f).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelected) return;
            
            transform.DOKill();
            transform.DOScale(_originalScale, 0.15f).SetEase(Ease.OutQuad);
        }
        
        private void OnDestroy()
        {
            _onCardClick.OnCompleted();
        }
    }
}