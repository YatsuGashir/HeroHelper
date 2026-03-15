using System;
using System.Collections.Generic;

namespace Data
{
    [Flags]
    public enum ResourceType
    {
        None,
        Spore,
        Stone, 
        Water, 
        Crystal
    }

    [Flags]
    public enum BuildingTag
    {
        None = 0,
        Organic = 1 << 0,
        Stone = 1 << 1,
        Water = 1 << 2,
        Crystal = 1 << 3,
        Magic = 1 << 4,
        Generator = 1 << 5,
        Buffer = 1 << 6,
        Converter = 1 << 7
    }

    public enum BuildingStage
    {
        Stage1,
        Stage2,
        Stage3Trasformation
    }

    [Serializable]
    public struct ResourceAmount
    {
        public ResourceType Type;
        public int Amount;
    }
    
    [Serializable]
    public struct ResourceCost
    {
        public List<ResourceAmount> costs;
    

        public bool CanAfford(Dictionary<ResourceType, int> currentResources)
        {
            foreach (var cost in costs)
            {
                if (!currentResources.TryGetValue(cost.Type, out var amount) || amount < cost.Amount)
                    return false;
            }
            return true;
        }
    }

    [Serializable]
    public class EffectDefinition
    {
        public List<ResourceAmount> production;
        public List<ResourceAmount> consumption;
        public BuildingDefinition blockTileAfterStage;
    }
}
