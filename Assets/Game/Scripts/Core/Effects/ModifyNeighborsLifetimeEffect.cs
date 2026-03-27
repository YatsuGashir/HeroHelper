using Core.Effects;
using Data;
using UnityEngine;

[System.Serializable]
public class ModifyNeighborsLifetimeEffect: GameEffectBase
{
    public BuildingTag affectedTag;

    public int lifetimeChange;
    
    protected override string GetDefaultDescription()
    {
        if (lifetimeChange > 0)
        {
            return $"продлевает жизнь всех построек с тегом {affectedTag} на {lifetimeChange} хода";
        }
        else
        {
            return $"сокращает жизнь всех построек с тегом {affectedTag} на {lifetimeChange} хода";
        }
    }
public override void Apply(EffectContext context)
        {
            int x = context.SourceBuilding.x;
            int y = context.SourceBuilding.y;
            

            var neighbors = context.Grid.GetNeighbors(x, y);

            int affectedCount = 0;

            foreach (var neighborCell in neighbors)
            {
                if (neighborCell.building == null) continue;

                var neighborBuilding = neighborCell.building;
                var def = neighborBuilding.GetDefinition();
                
                if (affectedTag != BuildingTag.None && !def.buildingTags.HasFlag(affectedTag))
                {
                    Debug.Log(" НЕ Применил эффект изменения жизни");
                    continue;
                }
                
                int newLifetime = neighborBuilding.remaingTime + lifetimeChange;
                
                Debug.Log("Применил эффект изменения жизни");
                neighborBuilding.remaingTime = newLifetime;
                affectedCount++;

            }
        }
}
