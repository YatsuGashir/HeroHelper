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
        [SerializeField] private float _selectLiftY = 20f; // 🔼 Насколько приподнимать карту при выборе (в пикселях Canvas)

        public BuildingDefinition CardDefinition { get; private set; }
        
        private readonly Subject<CardView> _onCardClick = new Subject<CardView>();
        public IObservable<CardView> OnCellClick => _onCardClick;

        private Image _cardImage;
        private Vector3 _originalLocalPosition; // 🔼 Запоминаем исходную позицию
        private Vector3 _originalScale;
        private bool _isSelected;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _originalLocalPosition = transform.localPosition; // 🔼 Сохраняем исходную позицию
            _cardImage = GetComponent<Image>();
            
            if (_cardImage == null)
            {
                _cardImage = GetComponentInChildren<Image>();
            }

            if (_cardImage != null)
            {
                _cardImage.raycastTarget = true;
            }
        }
        
        public void Init(BuildingDefinition card)
        {
            CardDefinition = card;
            
            if (_cardImage != null && card != null)
            {
                _cardImage.sprite = card.buildingIcon;
                _cardImage.color = Color.white;
            }
            
            // Сброс позиции при инициализации (на случай пулинга)
            transform.localPosition = _originalLocalPosition;
            SetSelected(false, false);
        }
        
        public void SetSelected(bool selected, bool animate = true)
        {
            if (_isSelected == selected) return;
            _isSelected = selected;

            // 🔼 Анимация подъёма/опускания по Y
            if (animate)
            {
                transform.DOKill(); // Останавливаем предыдущие твины
                
                var targetPosition = selected 
                    ? _originalLocalPosition + Vector3.up * _selectLiftY 
                    : _originalLocalPosition;
                    
                var targetScale = selected 
                    ? _originalScale * _hoverScale * 1.05f 
                    : _originalScale;
                
                // Анимируем позицию и масштаб одновременно
                transform.DOMoveY(targetPosition.y, 0.2f).SetEase(Ease.OutBack);
                transform.DOScale(targetScale, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                // Мгновенное применение без анимации
                transform.localPosition = selected 
                    ? _originalLocalPosition + Vector3.up * _selectLiftY 
                    : _originalLocalPosition;
                    
                transform.localScale = selected 
                    ? _originalScale * _hoverScale * 1.05f 
                    : _originalScale;
            }

            // 🎨 Подсветка цветом (опционально)
            if (_cardImage != null)
            {
                var targetColor = selected ? new Color(1.2f, 1.2f, 1.2f, 1) : Color.white;
                _cardImage.DOColor(targetColor, 0.2f);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onCardClick.OnNext(this);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_cardImage == null || _isSelected) return; // Не масштабируем при ховере, если карта уже выбрана

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