using System;

namespace Data
{
    namespace Data
    {
        public enum GroundType
        {
            Meadow,
            Water
        }

        public enum OverlayType
        {
            None,
            Stone,
            //Crystal,
            Forest 
        }

        public class CellState
        {
            public GroundType Ground;
            public OverlayType Overlay;
        }
    }
    public enum TerrainType
    {
        None    = 0,
        Meadow  = 1,
        Stone   = 2,
        Water   = 4,
        //Crystal = 8,
        Forest  = 8,
    }
    
    [Flags]
    public enum TerrainMask
    {
        None    = 0,
        Meadow  = 1 << 0,
        Stone   = 1 << 1,
        Water   = 1 << 2,
        //Crystal = 1 << 3,
        Forest   = 1 << 3
    }
    
    public static class TerrainMaskExtensions
    {
        public static bool Contains(this TerrainMask mask, TerrainType terrain)
        {
            if (terrain == TerrainType.None) return false;
            return (mask & (TerrainMask)terrain) != 0;
        }
    }
}
