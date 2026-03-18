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
            G.SuccessionManager.StartFirstRun();
            G.IncidentManager.StartRun();
            G.IncidentManager.StartLongTermEvent(config.allIncidents[0]);
            G.DeckManager.InitializeDeck(G.SuccessionManager.GetStartingHandIds());
            G.DeckManager.Shuffle();
            
            int startHandSize = 5;
            var startCards = G.DeckManager.DrawCardsWithReshuffle(startHandSize);
            G.HandManager.AddCards(startCards);
        }
        

    }
}
