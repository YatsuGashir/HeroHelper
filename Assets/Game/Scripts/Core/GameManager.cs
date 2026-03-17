using System;
using Core.Base;
using Cysharp.Threading.Tasks;
using Data;
using GlobalSpace;
using UnityEngine;
using View;

namespace Core
{
    public class GameManager 
    {
        public void StartNewRun(GridView gridView)
        {
            var config = G.Config;

            var genParams = config.terrainGenerationParams;
            genParams.seed = (int)UnityEngine.Random.Range(0, int.MaxValue);
            
            CellState[,] newGridData = TerrainGenerator.GenerateGrid(
                config.gridWidth, 
                config.gridHeight, 
                genParams
            );


            G.GridSystem.Initialize(newGridData);

            if (gridView != null)
            {
                gridView.Initialize(config.gridWidth, config.gridHeight);

                gridView.SyncWithGrid(newGridData); 
            }
            G.successionManager.StartFirstRun();
            G.IncidentManager.StartRun();
            G.IncidentManager.StartLongTermEvent(config.allIncidents[0]);
            G.DeckManager.InitializeDeck(G.successionManager.GetStartingHandIds());
            G.DeckManager.Shuffle();
            GiveFullHandForTest(); 
        }
        
        public void GiveFullHandForTest()
        {
            // 1. Получаем лимит руки (по умолчанию 10)
            int maxHandSize = G.HandManager.MaxHandSize;
    
            // 2. Считаем, сколько не хватает
            int cardsNeeded = maxHandSize - G.HandManager.Count;
    
            if (cardsNeeded > 0)
            {
                // 3. Тянем карты из колоды
                var drawnCards = G.DeckManager.DrawCards(cardsNeeded);
        
                // 4. Добавляем их в руку
                G.HandManager.AddCards(drawnCards);
        
                Debug.Log($"[TEST] Выдано {drawnCards.Count} карт. Всего в руке: {G.HandManager.Count}");
            }
            else
            {
                Debug.Log("[TEST] Рука уже полная!");
            }
        }
    }
}
