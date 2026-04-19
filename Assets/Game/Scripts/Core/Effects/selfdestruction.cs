using Core.Effects;
using GlobalSpace;
using UnityEngine;

public class selfdestruction: GameEffectBase
{
    protected override string GetDefaultDescription()
    {
        return $"Уничтожеается при наступлении фазы ";
    }
    public override void Apply(EffectContext context)
    {
        G.BuildingFactory.DestroyBuilding(context.X, context.Y, false);
    }
}
