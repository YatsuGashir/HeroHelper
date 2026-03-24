using System.Collections.Generic;
using Data;
using UnityEngine;

public class LifetimeBonusManager
{
    public List<TaggedLifetimeBonus> ActiveBonuses = new();

    public int GetLifetimeBonusForBuilding(BuildingInstance building)
    {
        int totalBonus = 0;
            
        foreach (var bonus in ActiveBonuses)
        {
            if (bonus.Matches(building))
            {
                totalBonus += bonus.lifetimeBonus;
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
