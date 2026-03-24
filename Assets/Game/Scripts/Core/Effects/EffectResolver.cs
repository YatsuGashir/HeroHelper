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
            BuildingInstance building = null,
            GridSystem grid = null,
            Dictionary<ResourceType, int> currentResources = null)
        {
            if (effects == null)
                return new EffectResult();
            
            var safeGrid = grid ?? GlobalSpace.G.GridSystem; 
            var safeResources = currentResources ?? new Dictionary<ResourceType, int>();

            var context = new EffectContext(building, safeGrid, safeResources);

            foreach (var effect in effects)
            {
                if (effect == null) continue;

                try
                {
                    Debug.Log($"Apply effect: {effect.GetType().Name}");
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
