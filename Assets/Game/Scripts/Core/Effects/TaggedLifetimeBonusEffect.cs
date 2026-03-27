using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;

public class TaggedLifetimeBonusEffect: GameEffectBase
{
    [Header("Настройки эффекта")]
    public BuildingTag targetTag = BuildingTag.None;
    public int lifetimeBonus = 2;
    public int windowDuration = 3;
    
    public override bool IsGlobal => true;
    public override bool IsInstant => false;

    protected override string GetDefaultDescription()
    {
        string tagInfo = targetTag != BuildingTag.None ? $"с тегом {targetTag}" : "все здания";
        return $"Постройки с тегом {tagInfo} получают {lifetimeBonus:+#;-#} к жизни при постройке";
    }
    public override void Apply(EffectContext context)
    {
        G.LifetimeBonusManager.ActiveBonuses.Add(new TaggedLifetimeBonus
        {
            targetTag = targetTag,
            lifetimeBonus = lifetimeBonus,
            remainingTurns = windowDuration
        });

        string tagInfo = targetTag != BuildingTag.None ? $"с тегом {targetTag}" : "все здания";
        Debug.Log($"[LifetimeBonus] Окно на {windowDuration} ходов: {tagInfo} получают {lifetimeBonus:+#;-#} к жизни при постройке");
    }
}
