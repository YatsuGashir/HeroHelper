using System.Collections.Generic;
using Core.Base;
using Core.Effects;
using Data;
using UnityEngine;

namespace Core.Effects
{
    public class EffectResolver
    {
        public EffectResult ResolveEffects(
            List<GameEffectBase> effects,
            BuildingInstance building,
            GridSystem grid,
            Dictionary<ResourceType, int> currentResources)
        {
            if (effects == null)
                return new EffectResult();

            var context = new EffectContext(building, grid, currentResources);

            foreach (var effect in effects)
            {
                if (effect == null) continue;

                try
                {
                    Debug.Log("applyEffect");
                    effect.Apply(context);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Effect error: {effect} {e}");
                }
            }

            return context.Result;
        }
    }
}
