using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace Data
{
    [Serializable]
    public class PlayerResources
    {
        public Dictionary<ResourceType, int> amounts = new Dictionary<ResourceType, int>();

        public PlayerResources()
        {
            foreach (ResourceType res in Enum.GetValues(typeof(ResourceType)))
            {
                amounts[res] = 0;
            }
        }

        public void AddResource(ResourceType res, int amount)
        {
            amounts[res] += amount;
        }

        public void RemoveResource(ResourceType res, int amount)
        {
            amounts[res] -= amount;
            if(amounts[res] < 0) amounts[res] = 0;
        }

        public bool HasEnough(ResourceCost cost)
        {
            return cost.CanAfford(amounts);
        }

        public void Pay(ResourceCost cost)
        {
            foreach (var c in cost.costs)
            {
                RemoveResource(c.Type, c.Amount);
            }
        }
    }
}
