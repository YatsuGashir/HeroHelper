using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;

public class TaggedResourceBonusEffect: GameEffectBase
{
    [Header("Настройки эффекта")]
    public BuildingTag targetTag;
    public ResourceType resourceType;
    public int bonusAmount = 1;
    public int duration = 3;

    public override bool IsGlobal => true;
    public override bool IsInstant => false;

    public override void Apply(EffectContext context)
    {
        G.ProductionBonusManager.ActiveBonuses.Add(new TaggedResourceBonus
        {
            targetTag = targetTag,
            resourceType = resourceType,
            bonusAmount = bonusAmount,
            remainingTurns = duration
        });

        Debug.Log($"[GlobalBonus] +{bonusAmount} {resourceType} for tag {targetTag} during {duration} turns");
    }
}
