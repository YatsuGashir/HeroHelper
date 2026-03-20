using Core.Effects;
using Data;
using System;
using UnityEngine;

namespace Core.Effects
{
    [Serializable]
    public class NeighborBonusResourceEffect : GameEffectBase
    {
        [Header("📦 Ресурс")]
        [Tooltip("Тип ресурса, который выдаётся.")]
        public ResourceType resourceType;
        
        [Tooltip("Базовое количество ресурса (без бонусов).")]
        public int baseAmount = 10;
        
        [Tooltip("Бонус за каждое подходящее соседнее здание.")]
        public int bonusPerBuilding = 5;
        
        [Tooltip("Бонус за каждую подходящую соседнюю клетку ландшафта.")]
        public int bonusPerTerrain = 3;

        [Header("🏢 Фильтр зданий")]
        [Tooltip("Учитывать только здания с этим тегом (None = все здания).")]
        public BuildingTag neighborTagFilter = BuildingTag.None;
        
        [Tooltip("Максимальное количество зданий для учёта (0 = без лимита).")]
        public int maxBuildingNeighbors = 0;

        [Header("🌿 Фильтр ландшафта")]
        [Tooltip("Какие типы земли дают бонус (пусто = игнорируется).")]
        public TerrainType[] bonusTerrainTypes = new TerrainType[0];
        
        [Tooltip("Максимальное количество клеток ландшафта для учёта (0 = без лимита).")]
        public int maxTerrainNeighbors = 0;

        public override void Apply(EffectContext context)
        {
            if (context.SourceBuilding == null) return;

            int x = context.SourceBuilding.x;
            int y = context.SourceBuilding.y;

            var neighbors = context.Grid.GetNeighbors(x, y);
            
            int buildingCount = 0;
            int terrainCount = 0;

            bool hasTerrainFilter = bonusTerrainTypes != null && bonusTerrainTypes.Length > 0;

            foreach (var neighborCell in neighbors)
            {
                if (neighborCell.building != null)
                {
                    var neighborBuilding = neighborCell.building;
                    var def = neighborBuilding.GetDefinition();

                    if (neighborTagFilter == BuildingTag.None || 
                        def.buildingTags.HasFlag(neighborTagFilter))
                    {
                        buildingCount++;
                    }
                }
                else if (hasTerrainFilter)
                {
                    foreach (var terrainType in bonusTerrainTypes)
                    {
                        if (neighborCell.terrainType == terrainType)
                        {
                            terrainCount++;
                            break;
                        }
                    }
                }
            }

            if (maxBuildingNeighbors > 0 && buildingCount > maxBuildingNeighbors)
                buildingCount = maxBuildingNeighbors;
                
            if (maxTerrainNeighbors > 0 && terrainCount > maxTerrainNeighbors)
                terrainCount = maxTerrainNeighbors;

            int totalAmount = baseAmount 
                            + (buildingCount * bonusPerBuilding) 
                            + (terrainCount * bonusPerTerrain);

            context.Result.ResourceChanges.Add(
                new ResourceAmount(resourceType, totalAmount));
        }
    }
}