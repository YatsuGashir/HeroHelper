using System.Collections.Generic;
using Core.Effects;
using Data;
using UnityEngine;

namespace Core.Effects
{

    [System.Serializable]
    public class ConsumeResourceEffect : GameEffectBase
    {
        public ResourceType type;
        public int amount;

        public override void Apply(EffectContext context)
        {
            Debug.Log("Applying produce resource");
            context.Result.ResourceChanges.Add (
                new ResourceAmount(type, -amount));
        }
    }
}