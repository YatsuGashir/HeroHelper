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
        [SerializeField] private Image _cardPrefab;
        [SerializeField] private List<CardSlot> _cardSlots = new List<CardSlot>();
        
        [Header("Fallback (если слотов меньше чем карт)")]
        [SerializeField] private Transform _overflowContainer;
        [SerializeField] private float _overflowSpacing = 1.2f;

        private CompositeDisposable _disposables;
        private HandManager _handManager;
        
        // 🔥 Словарь: уникальный ключ экземпляра → слот
        // Ключ формируется как "buildingId_instanceIndex_instanceHash"
        private readonly Dictionary<string, CardSlot> _cardSlotMap = new Dictionary<string, CardSlot>();

        public void Init(HandManager handManager)
        {
            _handManager = handManager;
            _disposables = new CompositeDisposable();
            
            _handManager.HandCards
                .Subscribe(_ => RefreshHand())
                .AddTo(_disposables);

            RefreshHand();
        }

        /// <summary>
        /// Генерирует уникальный ключ для экземпляра карты в руке
        /// </summary>
        private string GetCardInstanceKey(BuildingDefinition cardDef, int instanceIndex)
        {
            // Используем хеш объекта + индекс, чтобы отличать одинаковые ассеты
            return $"{cardDef.buildingId}_{instanceIndex}_{cardDef.GetHashCode()}";
        }

        private void RefreshHand()
        {
            var cards = _handManager.GetAllCards(); // List<BuildingDefinition>
            
            // 1. Собираем множество активных ключей, чтобы понять, что удалять
            var activeKeys = new HashSet<string>();
            for (int i = 0; i < cards.Count; i++)
            {
                string key = GetCardInstanceKey(cards[i], i);
                activeKeys.Add(key);
            }
            
            // 2. Удаляем слоты, карт в которых больше нет в руке
            var toRemove = new List<string>();
            foreach (var kvp in _cardSlotMap)
            {
                if (!activeKeys.Contains(kvp.Key) && kvp.Value != null)
                {
                    kvp.Value.Clear();
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var key in toRemove)
                _cardSlotMap.Remove(key);

            // 3. Добавляем новые карты в свободные слоты
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                string key = GetCardInstanceKey(card, i);
                
                // Если карта уже имеет слот — пропускаем
                if (_cardSlotMap.ContainsKey(key))
                    continue;

                // Ищем первый свободный слот
                CardSlot freeSlot = FindFirstFreeSlot();
                
                if (freeSlot != null)
                {
                    freeSlot.AssignCard(_cardPrefab, card);
                    _cardSlotMap[key] = freeSlot;
                }
                else
                {
                    // Слоты кончились — спавним в "переполнение"
                    SpawnOverflowCard(card, i - _cardSlots.Count);
                }
            }
        }

        private CardSlot FindFirstFreeSlot()
        {
            for (int i = 0; i < _cardSlots.Count; i++)
            {
                if (_cardSlots[i] != null && !_cardSlots[i].IsOccupied)
                    return _cardSlots[i];
            }
            return null;
        }

        private void SpawnOverflowCard(BuildingDefinition card, int index)
        {
            if (_overflowContainer == null || card == null) return;
            
            var go = Instantiate(_cardPrefab, _overflowContainer);
            go.transform.localPosition = new Vector3(index * _overflowSpacing, 0, -index * 0.1f);
            go.sprite = card.buildingCardICon;
            go.name = $"Card_Overflow_{card.name}_{index}";

            var cardView = go.GetComponent<CardView>();
            if (cardView == null) cardView = go.AddComponent<CardView>();
            cardView.Init(card);
            
            GlobalSpace.G.PlacementManager?.RegisterCard(cardView);
        }

        private void ClearAllSlots()
        {
            foreach (var slot in _cardSlots)
                slot?.Clear();
            
            if (_overflowContainer != null)
            {
                foreach (Transform child in _overflowContainer)
                    Destroy(child.gameObject);
            }
            
            // 🔥 Очищаем словарь привязок
            _cardSlotMap.Clear();
        }

        /// <summary>
        /// Найти слот по карте (для анимаций, выделения)
        /// </summary>
        public CardSlot GetSlotForCard(BuildingDefinition card, int instanceIndex)
        {
            string key = GetCardInstanceKey(card, instanceIndex);
            return _cardSlotMap.TryGetValue(key, out var slot) ? slot : null;
        }

        /// <summary>
        /// Освободить слот для карты (например, при взятии карты в игру)
        /// </summary>
        public void ReleaseCard(BuildingDefinition card, int instanceIndex)
        {
            string key = GetCardInstanceKey(card, instanceIndex);
            if (_cardSlotMap.TryGetValue(key, out var slot) && slot != null)
            {
                slot.Clear();
                _cardSlotMap.Remove(key);
            }
        }

        private void OnDestroy() => _disposables?.Dispose();

        public void Cleanup()
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            ClearAllSlots();
        }

#if UNITY_EDITOR
        [ContextMenu("Debug: Show Card Mapping")]
        private void DebugMapping()
        {
            Debug.Log($"[HandView] Mapped: {_cardSlotMap.Count} cards");
            foreach (var kvp in _cardSlotMap)
            {
                Debug.Log($"  {kvp.Key} → {kvp.Value?.name ?? "null"}");
            }
        }
#endif
    }
}