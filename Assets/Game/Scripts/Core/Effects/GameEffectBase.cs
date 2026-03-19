using System;
using UnityEngine;

namespace Core.Effects
{
    [Serializable]
    public abstract class GameEffectBase
    {
        public virtual bool IsInstant => false;

        public abstract void Apply(EffectContext context);
    }
}