using System;

namespace Data
{
    public enum TerrainType
    {
        Meadow,
        Stone,
        Water,
        Crystal,
        Grass,
    }
    
    [Flags]
    public enum TerrainMask
    {
        None    = 0,
        Meadow  = 1 << 0,
        Stone   = 1 << 1,
        Water   = 1 << 2,
        Crystal = 1 << 3,
        Grass   = 1 << 4
    }
    
    public static class TerrainMaskExtensions
    {
        public static bool Contains(this TerrainMask mask, TerrainType terrain)
        {
            return (mask & (TerrainMask)(1 << (int)terrain)) != 0;
        }
    }
}
