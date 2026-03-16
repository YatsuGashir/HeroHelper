using System.Collections.Generic;
using Data;
using UniRx;
using UnityEngine;

namespace Core.Base
{
    public class HandManager
    {
        private List<BuildingDefinition> _hand;


        public readonly ReactiveProperty<List<BuildingDefinition>> HandCards = new ReactiveProperty<List<BuildingDefinition>>(new List<BuildingDefinition>());
        public readonly ReactiveProperty<int> HandCount = new ReactiveProperty<int>(0);
        public readonly Subject<BuildingDefinition> OnCardAdded = new Subject<BuildingDefinition>();
        public readonly Subject<BuildingDefinition> OnCardRemoved = new Subject<BuildingDefinition>();
        public readonly Subject<Unit> OnHandCleared = new Subject<Unit>();

        public HandManager()
        {
            _hand = new List<BuildingDefinition>();
            HandCards.Value = _hand;
        }


        public void AddCard(BuildingDefinition card)
        {
            if (card == null)
            {
                Debug.LogWarning("Попытка добавить null карту в руку");
                return;
            }

            _hand.Add(card);
            
            HandCards.Value = new List<BuildingDefinition>(_hand);
            HandCount.Value = _hand.Count;
            
            OnCardAdded.OnNext(card);
        }


        public void AddCards(List<BuildingDefinition> cards)
        {
            if (cards == null) return;
            
            foreach (var card in cards)
            {
                if (card != null)
                {
                    _hand.Add(card);
                    OnCardAdded.OnNext(card);
                }
            }
            
            HandCards.Value = new List<BuildingDefinition>(_hand);
            HandCount.Value = _hand.Count;
        }

        public bool RemoveCard(BuildingDefinition card)
        {
            if (card == null) return false;
            
            if (_hand.Remove(card))
            {
                HandCards.Value = new List<BuildingDefinition>(_hand);
                HandCount.Value = _hand.Count;
                
                OnCardRemoved.OnNext(card);
                return true;
            }
            
            return false;
        }


        public BuildingDefinition RemoveCardAt(int index)
        {
            if (index < 0 || index >= _hand.Count)
            {
                Debug.LogWarning($"Индекс {index} вне диапазона руки");
                return null;
            }

            BuildingDefinition card = _hand[index];
            _hand.RemoveAt(index);
            
            HandCards.Value = new List<BuildingDefinition>(_hand);
            HandCount.Value = _hand.Count;
            
            OnCardRemoved.OnNext(card);
            
            return card;
        }


        public void ClearHand()
        {
            var removedCards = new List<BuildingDefinition>(_hand);
            _hand.Clear();
            
            HandCards.Value = new List<BuildingDefinition>(_hand);
            HandCount.Value = 0;
            
            foreach (var card in removedCards)
            {
                OnCardRemoved.OnNext(card);
            }
            
            OnHandCleared.OnNext(Unit.Default);
        }


        public BuildingDefinition GetCard(int index)
        {
            if (index >= 0 && index < _hand.Count)
            {
                return _hand[index];
            }
            return null;
        }


        public List<BuildingDefinition> GetAllCards()
        {
            return new List<BuildingDefinition>(_hand);
        }

        public bool HasCard(BuildingDefinition card)
        {
            return _hand.Contains(card);
        }


        public BuildingDefinition FindCard(System.Predicate<BuildingDefinition> predicate)
        {
            return _hand.Find(predicate);
        }


        public List<BuildingDefinition> FindCards(System.Predicate<BuildingDefinition> predicate)
        {
            return _hand.FindAll(predicate);
        }



        public int Count => _hand.Count;
        public bool IsEmpty => _hand.Count == 0;
        public bool IsFull { get; set; } = false; 
        public int MaxHandSize { get; set; } = 10; 


        public bool CanAddCard()
        {
            if (!IsFull) return true;
            return _hand.Count < MaxHandSize;
        }


        public void Dispose()
        {
            HandCards.Dispose();
            HandCount.Dispose();
            OnCardAdded.OnCompleted();
            OnCardRemoved.OnCompleted();
            OnHandCleared.OnCompleted();
        }

        
    }
}
