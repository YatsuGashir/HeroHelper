using System.Collections.Generic;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;

namespace Core.Base
{
    public class DeckManager
    {
        private List<BuildingDefinition> _deck;
        private List<BuildingDefinition> _discardPile;
        

        public readonly ReactiveProperty<int> DeckCount = new ReactiveProperty<int>(0);
        public readonly ReactiveProperty<int> DiscardCount = new ReactiveProperty<int>(0);
        public readonly Subject<BuildingDefinition> OnCardDrawn = new Subject<BuildingDefinition>();
        
        public readonly Subject<Unit> OnDeckEmpty = new Subject<Unit>();


        public DeckManager()
        {
            _deck = new List<BuildingDefinition>();
            _discardPile = new List<BuildingDefinition>();
        }


        public void InitializeDeck(List<BuildingDefinition> cards)
        {
            _deck.Clear();
            _discardPile.Clear();
            
            _deck.AddRange(cards);
            
            DeckCount.Value = _deck.Count;
            DiscardCount.Value = 0;
        }
        
        public void ClearAll()
        {
            _deck.Clear();
            _discardPile.Clear();
            
            DeckCount.Value = 0;
            DiscardCount.Value = 0;
        }

        public void Shuffle()
        {
            int n = _deck.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_deck[i], _deck[j]) = (_deck[j], _deck[i]);
            }
        }
        
        public void ShuffleDiscardIntoDeck()
        {
            if (_discardPile.Count == 0) return;
            
            _deck.AddRange(_discardPile);
            _discardPile.Clear();
            
            DiscardCount.Value = 0;
            DeckCount.Value = _deck.Count;
            
            Shuffle();
        }


        public BuildingDefinition DrawCard()
        {
            if (_deck.Count == 0)
            {
                OnDeckEmpty.OnNext(Unit.Default);
                Debug.LogWarning("Колода пуста!");
                return null;
            }

            BuildingDefinition card = _deck[0];
            _deck.RemoveAt(0);
            
            DeckCount.Value = _deck.Count;
            OnCardDrawn.OnNext(card);

            return card;
        }

        public List<BuildingDefinition> DrawCards(int count)
        {
            List<BuildingDefinition> drawnCards = new List<BuildingDefinition>();
            
            for (int i = 0; i < count; i++)
            {
                BuildingDefinition card = DrawCard();
                if (card != null)
                {
                    drawnCards.Add(card);
                }
                else
                {
                    break;
                }
            }

            return drawnCards;
        }

        public BuildingDefinition DrawCardWithReshuffle()
        {
            if (_deck.Count == 0)
            {
                if (_discardPile.Count > 0)
                {
                    ShuffleDiscardIntoDeck();
                }
                else
                {
                    OnDeckEmpty.OnNext(Unit.Default);
                    Debug.LogWarning("Колода и сброс пусты!");
                    return null;
                }
            }

            return DrawCard();
        }

        public void DiscardCard(BuildingDefinition card)
        {
            if (card == null) return;
            
            _discardPile.Add(card);
            DiscardCount.Value = _discardPile.Count;
        }

        public void DiscardCards(List<BuildingDefinition> cards)
        {
            foreach (var card in cards)
            {
                DiscardCard(card);
            }
        }


        public int TotalCardsInDeck => _deck.Count;
        public int TotalCardsInDiscard => _discardPile.Count;
        public bool IsDeckEmpty => _deck.Count == 0;
        public bool IsDiscardEmpty => _discardPile.Count == 0;
        
        public void AddCardToDeck(BuildingDefinition card)
        {
            if (card == null) return;
            
            _deck.Add(card);
            DeckCount.Value = _deck.Count;
        }


        public void RemoveCardFromGame(BuildingDefinition card)
        {
            if (card == null) return;
            
            _deck.Remove(card);
            _discardPile.Remove(card);
            
            DeckCount.Value = _deck.Count;
            DiscardCount.Value = _discardPile.Count;
        }

        public List<BuildingDefinition> DrawCardsWithReshuffle(int count)
        {
            List<BuildingDefinition> result = new List<BuildingDefinition>();

            for (int i = 0; i < count; i++)
            {
                if (_deck.Count == 0)
                {
                    if (_discardPile.Count > 0)
                    {
                        ShuffleDiscardIntoDeck();
                        Debug.Log("Колода пуста! Сброс перетасован в колоду.");
                    }
                    else
                    {
                        Debug.LogWarning("Колода и сброс пусты! Невозможно вытянуть больше карт.");
                        break;
                    }
                }

                var card = DrawCard();
                if (card != null)
                {
                    result.Add(card);
                }
            }

            return result;
        }

        public void Dispose()
        {
            DeckCount.Dispose();
            DiscardCount.Dispose();
            OnCardDrawn.OnCompleted();
        }


    }
    
}
