using System;
using UnityEngine;

namespace Core.Effects
{
    [Serializable]
    public abstract class GameEffectBase
    {
        [Header("=== Описание ===")]
        [Tooltip("Текст, который будет показан игроку в интерфейсе")]
        [Multiline(3)]
        [SerializeField] 
        protected string description = string.Empty;

        public virtual bool IsGlobal => false;
        public virtual bool IsInstant => false;
        
        public string Description => string.IsNullOrEmpty(description) 
            ? GetDefaultDescription() 
            : description;
        
        protected virtual string GetDefaultDescription() => 
            $"Эффект: {GetType().Name}";

        public abstract void Apply(EffectContext context);
    }
}