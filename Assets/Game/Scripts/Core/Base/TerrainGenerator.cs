using Data;
using UnityEngine;

namespace Core.Base
{
    public class TerrainGenerator
    {
        public static CellState[,] GenerateGrid(int width, int height, TerrainGenerationParams p)
        {
            CellState[,] grid = new CellState[width, height];
            var random = new System.Random(p.seed);

            float baseOffsetX = random.Next(0, 10000);
            float baseOffsetY = random.Next(0, 10000);

            float waterOffsetX = random.Next(0, 10000);
            float waterOffsetY = random.Next(0, 10000);

            // 1. БАЗОВАЯ КАРТА (поляна / лес)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = new CellState();
                    cell.x = x;
                    cell.y = y;
                    cell.building = null;

                    float noise = Mathf.PerlinNoise(
                        x * p.baseNoiseScale + baseOffsetX,
                        y * p.baseNoiseScale + baseOffsetY
                    );

                    cell.terrainType = noise > p.forestThreshold
                        ? TerrainType.Threes
                        : TerrainType.Meadow;

                    grid[x, y] = cell;
                }
            }

            // 2. ВОДА (реки и озёра)
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noise = Mathf.PerlinNoise(
                        x * p.waterNoiseScale + waterOffsetX,
                        y * p.waterNoiseScale + waterOffsetY
                    );

                    if (noise > p.waterThreshold)
                    {
                        grid[x, y].terrainType = TerrainType.Water;
                    }
                }
            }

            // 3. КАМЕННЫЕ КЛАСТЕРЫ
            GenerateStoneClusters(grid, width, height, random, p);

            return grid;
        }


        private static void GenerateStoneClusters(
            CellState[,] grid,
            int width,
            int height,
            System.Random random,
            TerrainGenerationParams p)
        {
            float offsetX = random.Next(0, 10000);
            float offsetY = random.Next(0, 10000);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y].terrainType == TerrainType.Water)
                        continue;

                    float noise = Mathf.PerlinNoise(
                        x * p.stoneNoiseScale + offsetX,
                        y * p.stoneNoiseScale + offsetY
                    );

                    if (noise > p.stoneThreshold)
                    {
                        int clusterSize = random.Next(
                            p.stoneClusterMin,
                            p.stoneClusterMax + 1
                        );

                        SpreadStoneCluster(
                            grid,
                            width,
                            height,
                            x,
                            y,
                            clusterSize,
                            random,
                            p
                        );
                    }
                }
            }
        }


        private static void SpreadStoneCluster(
            CellState[,] grid,
            int width,
            int height,
            int startX,
            int startY,
            int size,
            System.Random random,
            TerrainGenerationParams p)
        {
            int placed = 0;

            for (int i = 0; i < size * 3 && placed < size; i++)
            {
                int dx = random.Next(-2, 3);
                int dy = random.Next(-2, 3);

                int nx = startX + dx;
                int ny = startY + dy;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                    continue;

                if (grid[nx, ny].terrainType == TerrainType.Water)
                    continue;

                if (grid[nx, ny].terrainType == TerrainType.Stone)
                    continue;

                grid[nx, ny].terrainType = TerrainType.Stone;

                /*// шанс кристалла
                if (random.NextDouble() < p.crystalChance)
                    grid[nx, ny].terrainType = TerrainType.Crystal;*/

                placed++;
            }
        }
    }
}