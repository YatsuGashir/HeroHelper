using Core.Effects;
using Data;
using UnityEngine;

namespace Core.Effects
{
    public abstract class ConditionalEffectBase: GameEffectBase
    {
        public ResourceCost cost;

        public override void Apply(EffectContext context)
        {
            if (HasEnoughResources(context))
            {
                SpendResources(context);
                ApplyEffect(context);
            }
            else
            {
                Debug.Log($"{this.GetType().Name} не применён — недостаточно ресурсов");
            }
        }

        protected bool HasEnoughResources(EffectContext context)
        {
            foreach (var item in cost.costs)
            {
                if (!context.CurrentResources.TryGetValue(item.Type, out int amount) || amount < item.Amount)
                    return false;
            }
            return true;
        }

        protected void SpendResources(EffectContext context)
        {
            foreach (var item in cost.costs)
            {
                context.Result.ResourceChanges.Add(new ResourceAmount(item.Type, -item.Amount));
                //context.CurrentResources[item.Type] -= item.Amount;
            }
        }
        
        protected abstract void ApplyEffect(EffectContext context);
    }
}
