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
            
            // 1. Генерация данных
            var genParams = config.terrainGenerationParams;
            genParams.seed = (int)UnityEngine.Random.Range(0, int.MaxValue);
            
            CellState[,] newGridData = TerrainGenerator.GenerateGrid(
                config.gridWidth, 
                config.gridHeight, 
                genParams
            );

            // 2. Инициализация логики
            G.GridSystem.Initialize(newGridData);

            // 3. Инициализация ВИЗУАЛА (ЕДИНСТВЕННОЕ МЕСТО!)
            if (gridView != null)
            {
                // Передаем правильные размеры (Width, Height)
                gridView.Initialize(config.gridWidth, config.gridHeight);
                
                // Синхронизируем данные
                gridView.SyncWithGrid(newGridData); 
            }
            else
            {
                Debug.LogError("GridView is null! Проверьте ссылку в Bootstrap.");
            }
        }
    }
}
