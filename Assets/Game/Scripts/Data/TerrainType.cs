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
    [Serializable]
    public struct TerrainTypeStructure
    {
        public TerrainType type;
        public Type value;
    }
}
