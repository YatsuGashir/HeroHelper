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

        /// <summary>
        /// Инициализация HandView с подключением к HandManager
        /// </summary>
        public void Init(HandManager handManager)
        {
            _handManager = handManager;
            _disposables = new CompositeDisposable();

            // Подписка на изменение состава руки (полная перерисовка)
            _handManager.HandCards
                .Subscribe(OnHandCardsChanged)
                .AddTo(_disposables);

            // Подписка на добавление карты
            _handManager.OnCardAdded
                .Subscribe(OnCardAdded)
                .AddTo(_disposables);

            // Подписка на удаление карты
            _handManager.OnCardRemoved
                .Subscribe(OnCardRemoved)
                .AddTo(_disposables);

            // Подписка на очистку руки
            _handManager.OnHandCleared
                .Subscribe(_ => OnHandCleared())
                .AddTo(_disposables);

            // Первичная отрисовка
            RefreshHand();
        }

        /// <summary>
        /// Полная перерисовка руки (при изменении списка карт)
        /// </summary>
        private void OnHandCardsChanged(List<BuildingDefinition> cards)
        {
            RefreshHand();
        }

        /// <summary>
        /// Обработка добавления карты
        /// </summary>
        private void OnCardAdded(BuildingDefinition card)
        {
            SpawnCard(card);
            UpdateCardsPosition();
        }

        /// <summary>
        /// Обработка удаления карты
        /// </summary>
        private void OnCardRemoved(BuildingDefinition card)
        {
            RemoveCardView(card);
            UpdateCardsPosition();
        }

        /// <summary>
        /// Обработка очистки руки
        /// </summary>
        private void OnHandCleared()
        {
            ClearAllCards();
        }

        /// <summary>
        /// Полное обновление отображения руки
        /// </summary>
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

        /// <summary>
        /// Создать визуальное представление карты
        /// </summary>
        private void SpawnCard(BuildingDefinition card)
        {
            if (card == null || cardPrefab == null) return;

            var go = Instantiate(cardPrefab, cardsContainer);
            go.transform.localPosition = Vector3.zero;
            go.sprite = card.buildingIcon;
            go.name = $"Card_{card.name}";

            // Добавляем ссылку на карту в компонент для последующего поиска
            var cardView = go.GetComponent<CardView>();
            if (cardView == null)
            {
                cardView = go.AddComponent<CardView>();
            }
            cardView.Init(card);

            _cardViews.Add(go);
            
            G.PlacementManager?.RegisterCard(cardView);
        }

        /// <summary>
        /// Удалить визуальное представление конкретной карты
        /// </summary>
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

        /// <summary>
        /// Очистить все карты из руки
        /// </summary>
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

        /// <summary>
        /// Обновить позиции карт (выстроить в ряд)
        /// </summary>
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

        /// <summary>
        /// Очистка подписок при уничтожении
        /// </summary>
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        /// Принудительная очистка (например, при смене сцены)
        /// </summary>
        public void Cleanup()
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            ClearAllCards();
        }
    }
}