using System;
using UnityEngine;

namespace Core.Effects
{
    [Serializable]
    public abstract class GameEffectBase
    {
        public virtual bool IsGlobal => false;
        public virtual bool IsInstant => false;

        [SerializeField]
        [TextArea(3, 5)] private string description = string.Empty;
        
        public abstract void Apply(EffectContext context);
    }
}