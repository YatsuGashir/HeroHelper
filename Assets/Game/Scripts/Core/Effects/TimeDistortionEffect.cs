using Core.Base;
using Core.Effects;
using GlobalSpace;
using UnityEngine;

public class TimeDistortionEffect: GameEffectBase
{
    public int currentDelta = 1;
    public int duration = 3;

    public override bool IsGlobal => true;

    public override void Apply(EffectContext context)
    {
        G.TickModifierManager.ActiveModifiers.Add(new TickModifier
        {
            delta = currentDelta,
            duration = duration,
            targetTag = null
        });

        Debug.Log($"Time Distortion Applied globally for {duration} ticks");
    }
}
