using System.Collections.Generic;
using Core.Base;
using Data;
using UnityEngine;

public class TickModifierManager
{
    public List<TickModifier> ActiveModifiers = new();

    public int GetLifeDelta(BuildingInstance building)
    {
        int total = 0;

        foreach (var mod in ActiveModifiers)
        {
            if (mod.targetTag.HasValue)
            {
                var tags = building.GetDefinition().buildingTags;

                if ((tags & mod.targetTag.Value) == 0)
                    continue;
            }

            total += mod.delta;
        }

        return total;
    }

    public void Tick()
    {
        foreach (var mod in ActiveModifiers)
            mod.duration--;

        ActiveModifiers.RemoveAll(m => m.duration <= 0);
    }
}
