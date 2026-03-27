using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;

public class TransformationEffect: GameEffectBase
{
    public BuildingDefinition Definition;
    
    protected override string GetDefaultDescription()
    {
        return $"Превращается в {Definition.buildingName}";
    }
    public override void Apply(EffectContext context)
    {
        G.BuildingFactory.DestroyBuilding(context.X, context.Y, true);
        G.BuildingFactory.CreateBuilding(Definition, context.X, context.Y );
    }
}
