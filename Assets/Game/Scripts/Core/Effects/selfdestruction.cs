using Core.Effects;
using GlobalSpace;
using UnityEngine;

public class selfdestruction: GameEffectBase
{
    public override void Apply(EffectContext context)
    {
        G.BuildingFactory.DestroyBuilding(context.X, context.Y, false);
    }
}
