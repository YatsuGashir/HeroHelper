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

        public BuildingDefinition CardDefinition { get; private set; }
        
        private readonly Subject<BuildingDefinition> _onCardClick = new Subject<BuildingDefinition>();
        public IObservable<BuildingDefinition> OnCellClick => _onCardClick;

        private Image _cardImage;
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
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
        }

        public void OnPointerClick(PointerEventData eventData)
        { ;
            _onCardClick.OnNext(CardDefinition);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            
            if (_cardImage == null)
            {
                return;
            }

            transform.DOKill();
            transform.DOScale(_originalScale * _hoverScale, 0.15f).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
            transform.DOKill();
            transform.DOScale(_originalScale, 0.15f).SetEase(Ease.OutQuad);
        }
        
        private void OnDestroy()
        {
            _onCardClick.OnCompleted();
        }
    }
}