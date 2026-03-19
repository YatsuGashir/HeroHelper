using System.Collections.Generic;
using Core.Base;
using Data;

namespace Core.Effects
{
    public class EffectContext
    {
        public BuildingInstance SourceBuilding;

        public int X;
        public int Y;

        public GridSystem Grid;

        public Dictionary<ResourceType, int> CurrentResources;

        public EffectResult Result;

        public EffectContext(
            BuildingInstance source,
            GridSystem grid,
            Dictionary<ResourceType, int> resources)
        {
            SourceBuilding = source;
            X = source.x;
            Y = source.y;
            Grid = grid;

            CurrentResources = resources;
            Result = new EffectResult();
        }
        
    }
}
