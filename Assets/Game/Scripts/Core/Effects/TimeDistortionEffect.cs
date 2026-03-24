using Core.Base;
using Core.Effects;
using Data; // Для BuildingTag
using GlobalSpace;
using UnityEngine;

public class TimeDistortionEffect : GameEffectBase
{
    [Header("Настройки времени")]
    public int currentDelta = 1;
    public int duration = 3;

    [Header("Фильтр по тегам")]
    [Tooltip("Если выбрано None — эффект действует на ВСЕ здания. Если выбран тег — только на здания с этим тегом.")]
    public bool affectSpecificTag = false;
    public BuildingTag targetTag = BuildingTag.None;

    public override bool IsGlobal => true;
    public override bool IsInstant => false;

    public override void Apply(EffectContext context)
    {
        BuildingTag? tagToSend = affectSpecificTag ? targetTag : null;

        G.TickModifierManager.ActiveModifiers.Add(new TickModifier
        {
            delta = currentDelta,
            duration = duration,
            targetTag = tagToSend
        });

    }
}