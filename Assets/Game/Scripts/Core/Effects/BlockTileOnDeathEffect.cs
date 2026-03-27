using System;
using Core.Effects;

namespace Core.Effects
{
    [Serializable]
    public class BlockTileOnDeathEffect : GameEffectBase
    {
        public bool blockPermanently = true;
        
        protected override string GetDefaultDescription()
        {
            return $"Блокирует клетку для строительства ";
        }
        public override void Apply(EffectContext context)
        {
            throw new NotImplementedException();
        }
    }
}
