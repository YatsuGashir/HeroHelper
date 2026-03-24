using System.Collections.Generic;
using Core.Effects;
using Data;
using UnityEngine;

public class GlobalProductionBonusManager
{
        public List<TaggedResourceBonus> ActiveBonuses = new();
        
        public int GetBonusForBuilding(BuildingInstance building, ResourceType resourceType)
        {
            int totalBonus = 0;
            
            foreach (var bonus in ActiveBonuses)
            {
                if (bonus.Matches(building, resourceType))
                {
                    totalBonus += bonus.bonusAmount;
                }
            }
            
            return totalBonus;
        }
        
        public void Tick()
        {
            foreach (var bonus in ActiveBonuses)
                bonus.remainingTurns--;

            ActiveBonuses.RemoveAll(b => b.remainingTurns <= 0);
        }
}
