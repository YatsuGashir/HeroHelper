using System;
using System.Collections.Generic;
using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;

[Serializable]
public class NeighborResourceBuffEffect: GameEffectBase
{
        [Header("📦 Ресурс")]
        [Tooltip("Какой ресурс раздавать соседям.")]
        public ResourceType resourceType;
        
        [Tooltip("Сколько ресурса получает КАЖДЫЙ подходящий сосед.")]
        public int amountPerNeighbor = 5;
        
        [Tooltip("Раздавать ли ресурс самому зданию-источнику.")]
        public bool includeSelf = false;

        [Header("🎯 Фильтр получателей")]
        [Tooltip("Раздавать только зданиям с этим тегом (None = всем зданиям).")]
        public BuildingTag targetTagFilter = BuildingTag.None;
        
        [Tooltip("Раздавать только если соседняя клетка имеет этот тип ландшафта (пусто = игнорировать).")]
        public TerrainType[] targetTerrainTypes = new TerrainType[0];
        
        [Tooltip("Максимальное количество получателей (0 = без лимита).")]
        public int maxTargets = 0;

        [Header("⚙️ Логика")]
        [Tooltip("Если включено — ресурс делится поровну между соседями вместо умножения.")]
        public bool splitTotalAmount = false;
        
        [Tooltip("Общее количество ресурса для разделения (работает только с splitTotalAmount).")]
        public int totalAmountToSplit = 50;

        public override void Apply(EffectContext context)
        {
            if (context.SourceBuilding == null) return;

            int x = context.SourceBuilding.x;
            int y = context.SourceBuilding.y;

            var neighbors = context.Grid.GetNeighbors(x, y);
            var targets = new List<(int cx, int cy)>();

            bool hasTerrainFilter = targetTerrainTypes != null && targetTerrainTypes.Length > 0;

            // --- 1. СБОР ПОДХОДЯЩИХ СОСЕДЕЙ ---
            foreach (var neighborCell in neighbors)
            {
                if (neighborCell.building == null) continue;

                var neighborBuilding = neighborCell.building;
                var def = neighborBuilding.GetDefinition();

                // Проверка тега здания
                if (targetTagFilter != BuildingTag.None && 
                    !def.buildingTags.HasFlag(targetTagFilter))
                {
                    continue;
                }

                // Проверка типа ландшафта
                if (hasTerrainFilter)
                {
                    bool terrainMatch = false;
                    foreach (var terrainType in targetTerrainTypes)
                    {
                        if (neighborCell.terrainType == terrainType)
                        {
                            terrainMatch = true;
                            break;
                        }
                    }
                    if (!terrainMatch) continue;
                }

                targets.Add((neighborCell.x, neighborCell.y));
            }

            // --- 2. ОПЦИОНАЛЬНО: САМО ЗДАНИЕ ---
            if (includeSelf)
            {
                var selfCell = context.Grid.GetCell(x, y);
                if (selfCell?.building != null)
                {
                    var selfDef = selfCell.building.GetDefinition();
                    
                    bool tagOk = targetTagFilter == BuildingTag.None || 
                                selfDef.buildingTags.HasFlag(targetTagFilter);
                    
                    bool terrainOk = !hasTerrainFilter;
                    if (hasTerrainFilter)
                    {
                        foreach (var t in targetTerrainTypes)
                        {
                            if (selfCell.terrainType == t) { terrainOk = true; break; }
                        }
                    }
                    
                    if (tagOk && terrainOk)
                        targets.Add((x, y));
                }
            }

            // --- 3. ПРИМЕНЕНИЕ ЛИМИТА ---
            if (maxTargets > 0 && targets.Count > maxTargets)
                targets.RemoveRange(maxTargets, targets.Count - maxTargets);

            if (targets.Count == 0) return;

            // --- 4. РАСЧЁТ И ДОБАВЛЕНИЕ РЕСУРСОВ ---
            if (splitTotalAmount)
            {
                // Режим "разделить поровну"
                int perTarget = totalAmountToSplit / targets.Count;
                int remainder = totalAmountToSplit % targets.Count;
                
                for (int i = 0; i < targets.Count; i++)
                {
                    var (tx, ty) = targets[i];
                    int amount = perTarget + (i < remainder ? 1 : 0); // Честное распределение остатка
                    
                    context.Result.ResourceChanges.Add(
                        new ResourceAmount(resourceType, amount));
                    
                    Debug.Log($"[Раздача] {tx},{ty}: +{amount} {resourceType} (разделено)");
                }
            }
            else
            {
                // Режим "каждому по фиксированному количеству"
                foreach (var (tx, ty) in targets)
                {
                    context.Result.ResourceChanges.Add(
                        new ResourceAmount(resourceType, amountPerNeighbor));
                    
                    Debug.Log($"[Раздача] {tx},{ty}: +{amountPerNeighbor} {resourceType}");
                }
            }
        }
}
