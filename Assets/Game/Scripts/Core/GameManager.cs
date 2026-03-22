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
        private bool _isInitialized = false;

        public void Initialize(GridView gridView)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[GameManager] Already initialized!");
                return;
            }
            
            var config = G.Config;

            var genParams = config.terrainGenerationParams;
            genParams.seed = (int)UnityEngine.Random.Range(0, int.MaxValue);
            
            CellState[,] gridData = TerrainGenerator.GenerateGrid(
                config.gridWidth, 
                config.gridHeight, 
                genParams
            );

            G.GridSystem.Initialize(gridData);

            if (gridView != null)
            {
                gridView.Initialize(config.gridWidth, config.gridHeight);
                gridView.SyncWithGrid(gridData); 
            }

            G.WandererManager.Initialize(G.Config.initialWandererCount);
            
            G.DeckManager.InitializeDeck(G.SuccessionManager.GetStartingHandIds());
            
            G.IncidentManager.StartRun();
            
            _isInitialized = true;
            Debug.Log("[GameManager] Systems initialized.");
        }


        public async UniTask StartNewRun()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[GameManager] Call Initialize() first!");
                return;
            }

            
            G.SuccessionManager.StartFirstRun();
            
            var startingDeck = G.SuccessionManager.GetStartingHandIds();
            Debug.Log($"[GameManager] Starting deck count: {startingDeck?.Count ?? 0}");
    
            G.DeckManager.InitializeDeck(startingDeck);
            G.DeckManager.Shuffle();
            
            int startHandSize = 5;
            var startCards = G.DeckManager.DrawCardsWithReshuffle(startHandSize);
            Debug.Log($"[GameManager] Drawn cards: {startCards?.Count ?? 0}");
    
            G.HandManager.AddCards(startCards);

            
            G.IncidentManager.StartRun();
            
            if (G.Config.allIncidents != null && G.Config.allIncidents.Count > 0)
            {
                //G.IncidentManager.StartLongTermEvent(G.Config.allIncidents[0]);
            }
            
            G.ResourceManager.ResetToStart();
            
            // G.TurnManager.Reset();
            
            Debug.Log("[GameManager] New run started!");
        }
        
        public async UniTask RestartRun(GridView gridView)
        {
            // Если нужно полностью переинициализировать сетку:
            // _isInitialized = false;
            // Initialize(gridView);
            
            await StartNewRun();
        }
    }
}