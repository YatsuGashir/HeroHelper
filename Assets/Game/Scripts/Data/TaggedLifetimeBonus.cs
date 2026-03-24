using System;
using Data;
using UnityEngine;

[Serializable]
public class TaggedLifetimeBonus
{
    public BuildingTag targetTag;
    public int lifetimeBonus;
    public int remainingTurns;

    public bool Matches(BuildingInstance building)
    {
        var tags = building.GetDefinition().buildingTags;
        return (tags & targetTag) != 0;
    }
}
