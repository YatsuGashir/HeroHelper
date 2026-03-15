using System;

namespace Data
{
    [Serializable]
    public class CellState
    {
        public int x;
        public int y;
        
        public TerrainType terrainType;
        public BuildingInstance building;
        public bool isOccupied;

    }
}
