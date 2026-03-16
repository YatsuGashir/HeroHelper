using System;
using System.Collections.Generic;
using Core.Base;
using Core.Effects;
using Data;
using UnityEngine;

namespace Core.Effects
{
    [Serializable]
    public class ProduceResourceEffect : ConditionalEffectBase
    {
        public ResourceType type;
        public int amount;

        protected override void ApplyEffect(EffectContext context)
        {
            Debug.Log("Applying produce resource");
            context.Result.ResourceChanges.Add(
                new ResourceAmount(type, amount));
        }
    }
}
