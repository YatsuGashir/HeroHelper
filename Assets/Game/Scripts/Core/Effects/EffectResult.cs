using System.Collections.Generic;
using Data;

namespace Core.Effects
{
    public class EffectResult
    {
        public List<ResourceAmount> ResourceChanges = new();
        public List<CellChange> CellChanges = new();
        public List<BuildingChange> BuildingChanges = new();
    }

    public abstract class CellChange
    {
        public int X;
        public int Y;
        public TerrainType? NewTerrain;
    }

    public abstract class BuildingChange
    {
        public int X;
        public int Y;
        public bool Destroy;
        public int LifeDelta;
    }
}