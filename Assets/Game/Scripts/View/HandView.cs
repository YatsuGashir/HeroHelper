using System.Collections.Generic;
using Data;
using Core.Base;
using GlobalSpace;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class HandView : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private Image cardPrefab;
        [SerializeField] private Transform cardsContainer;
        [SerializeField] private float cardSpacing = 1.2f;
        [SerializeField] private Vector3 cardOffset = new Vector3(0, 0, -0.1f);

        private CompositeDisposable _disposables = new CompositeDisposable();
        private HandManager _handManager;
        private List<Image> _cardViews = new List<Image>();


        public void Init(HandManager handManager)
        {
            _handManager = handManager;
            _disposables = new CompositeDisposable();
            
            _handManager.HandCards
                .Subscribe(cards => 
                {
                    RefreshHand(); 
                })
                .AddTo(_disposables);
            
/*
            _handManager.HandCards
                .Subscribe(OnHandCardsChanged)
                .AddTo(_disposables);

            _handManager.OnCardAdded
                .Subscribe(OnCardAdded)
                .AddTo(_disposables);

            _handManager.OnCardRemoved
                .Subscribe(OnCardRemoved)
                .AddTo(_disposables);

            _handManager.OnHandCleared
                .Subscribe(_ => OnHandCleared())
                .AddTo(_disposables);*/

            RefreshHand();
        }

        private void OnHandCardsChanged(List<BuildingDefinition> cards)
        {
            RefreshHand();
        }

        private void OnCardAdded(BuildingDefinition card)
        {
            SpawnCard(card);
            UpdateCardsPosition();
        }

        private void OnCardRemoved(BuildingDefinition card)
        {
            RemoveCardView(card);
            UpdateCardsPosition();
        }

        private void OnHandCleared()
        {
            ClearAllCards();
        }

        private void RefreshHand()
        {
            ClearAllCards();
            
            var cards = _handManager.GetAllCards();
            foreach (var card in cards)
            {
                SpawnCard(card);
            }
            
            UpdateCardsPosition();
        }

        private void SpawnCard(BuildingDefinition card)
        {
            if (card == null || cardPrefab == null) return;

            var go = Instantiate(cardPrefab, cardsContainer);
            go.transform.localPosition = Vector3.zero;
            go.sprite = card.buildingCardICon;
            go.name = $"Card_{card.name}";

            var cardView = go.GetComponent<CardView>();
            if (cardView == null)
            {
                cardView = go.AddComponent<CardView>();
            }
            cardView.Init(card);

            _cardViews.Add(go);
            
            G.PlacementManager?.RegisterCard(cardView);
        }

        private void RemoveCardView(BuildingDefinition card)
        {
            foreach (var cardView in _cardViews)
            {
                if (cardView == null) continue;
                
                var cardViewComponent = cardView.GetComponent<CardView>();
                if (cardViewComponent != null && cardViewComponent.CardDefinition == card)
                {
                    _cardViews.Remove(cardView);
                    Destroy(cardView.gameObject);
                    return;
                }
            }
        }

        private void ClearAllCards()
        {
            foreach (var cardView in _cardViews)
            {
                if (cardView != null)
                {
                    Destroy(cardView.gameObject);
                }
            }
            _cardViews.Clear();
        }

        private void UpdateCardsPosition()
        {
            for (int i = 0; i < _cardViews.Count; i++)
            {
                if (_cardViews[i] != null)
                {
                    float x = (i - _cardViews.Count / 2f) * cardSpacing;
                    _cardViews[i].transform.localPosition = new Vector3(x, 0, i * cardOffset.z);
                }
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        public void Cleanup()
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            ClearAllCards();
        }
    }
}