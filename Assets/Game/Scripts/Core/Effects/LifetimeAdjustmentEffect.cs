// Core/Effects/LifetimeAdjustmentEffect.cs
using Core.Base;
using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;

public class LifetimeAdjustmentEffect : GameEffectBase
{
    [Header("Настройки эффекта")]
    public int lifetimeDelta = 1;

    [Header("Фильтр по тегам")]
    [Tooltip("Если снято — эффект применится ко ВСЕМ зданиям на карте")]
    public bool affectSpecificTag = false;
    public BuildingTag targetTag = BuildingTag.None;

    [Header("Ограничения")]
    [Tooltip("Минимальное значение времени жизни после применения (защита от отрицательных значений)")]
    public int minLifetime = 0;
    [Tooltip("Максимальное значение времени жизни (опционально, 0 = без лимита)")]
    public int maxLifetime = 0;

    public override bool IsGlobal => true;
    public override bool IsInstant => true;
    
    protected override string GetDefaultDescription()
    {
        string deltaText = lifetimeDelta >= 0 
            ? $"+{lifetimeDelta}" 
            : lifetimeDelta.ToString();
        
        string baseDesc = $"Время жизни: <color=#4CAF50>{deltaText} ход</color>";
        
        if (affectSpecificTag && targetTag != BuildingTag.None)
        {
            baseDesc += $" <color=#888>({targetTag})</color>";
        }
        else if (affectSpecificTag)
        {
            baseDesc += " <color=#888>(без тега)</color>";
        }
        var constraints = new System.Collections.Generic.List<string>();
        
        if (minLifetime > 0)
            constraints.Add($"мин. {minLifetime}");
        
        if (maxLifetime > 0)
            constraints.Add($"макс. {maxLifetime}");
        
        if (constraints.Count > 0)
        {
            baseDesc += $" <color=#666>[{string.Join(" | ", constraints)}]</color>";
        }
        
        return baseDesc;
    }

    public override void Apply(EffectContext context)
    {
        if (context.Grid == null)
        {
            Debug.LogWarning("[LifetimeAdjustment] Grid is null, effect skipped");
            return;
        }

        var gridData = context.Grid.GetGridData();
        int width = gridData.GetLength(0);
        int height = gridData.GetLength(1);

        int affectedCount = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = gridData[x, y];
                if (cell.building == null) continue;

                var building = cell.building;
                
                if (affectSpecificTag)
                {
                    var buildingTags = building.GetDefinition().buildingTags;
                    if ((buildingTags & targetTag) == 0)
                        continue;
                }

                int newLifetime = building.remaingTime + lifetimeDelta;
                
                if (newLifetime < minLifetime) newLifetime = minLifetime;
                if (maxLifetime > 0 && newLifetime > maxLifetime) newLifetime = maxLifetime;

                building.remaingTime = newLifetime;
                affectedCount++;

                Debug.Log($"[Lifetime] {building.GetDefinition().buildingName} [{x},{y}]: {building.remaingTime} turns");
            }
        }

    }
}