using System;
using Data;

namespace Core.Effects
{
    [Serializable]
    public class TaggedResourceBonus
    {
        public BuildingTag targetTag;
        public ResourceType resourceType;
        public int bonusAmount;
        public int remainingTurns;

        public bool Matches(BuildingInstance building, ResourceType type)
        {
            if (resourceType != type) return false;
            var tags = building.GetDefinition().buildingTags;
            return (tags & targetTag) != 0;
        }
    }
}