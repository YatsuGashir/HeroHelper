using System.Collections.Generic;
using Core.Base;
using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;

namespace Core.Base
{
    public class BuildingLifecycleManager
    {
        private readonly EffectResolver _resolver;
        private readonly GridSystem _grid;

        public BuildingLifecycleManager()
        {
            _resolver = new EffectResolver();
            _grid = G.GridSystem;
        }

        public void ProcessEndOfTurn()
        {
            var gridData = _grid.GetGridData();
            int width = gridData.GetLength(0);
            int height = gridData.GetLength(1);
        
            var playerResources = G.ResourceManager.GetResources(); 
        
            List<ResourceAmount> totalResourceChanges = new();
            List<CellChange> totalCellChanges = new();
            List<BuildingChange> totalBuildingChanges = new();
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = gridData[x, y];
                    if (cell.building == null) continue;

                    var building = cell.building;

                    bool stageChanged = building.Tick();
                    if (stageChanged)
                    {
                        G.Events.BuildingStageChanged.OnNext(building);
    
                        // Также можно отправить CellChanged, если нужно перерисовать спрайт
                        // G.Events.CellChanged.OnNext(...);
                    }

                    var effects = building.GetEffectDefinition()?.effects;
                    if (effects == null) continue;

                    var result = _resolver.ResolveEffects(
                        effects,
                        building,
                        _grid,
                        playerResources
                    );

                    totalResourceChanges.AddRange(result.ResourceChanges);
                    totalCellChanges.AddRange(result.CellChanges);
                    totalBuildingChanges.AddRange(result.BuildingChanges);
                
               
                }
            }
            ApplyResults(totalResourceChanges, totalCellChanges, totalBuildingChanges);
        
            G.TickModifierManager.Tick();
            
            G.Events.ResourceChanged.OnNext(G.ResourceManager.GetResources());
        }
        private void ApplyResults(
            List<ResourceAmount> resources,
            List<CellChange> cells,
            List<BuildingChange> buildings)
        {
            foreach (var r in resources)
            {
                Debug.Log("add res" + r.Type + " to " + r.Amount );
                G.ResourceManager.AddResource(r.Type, r.Amount);
            }

            /*foreach (var c in cells)
        {
            var cell = _grid.GetCell(c.X, c.Y);

            if (c.newTerrain != null)
                cell.terrainType = c.newTerrain.Value;

            G.Events.CellChanged.OnNext(new CellUpdateEventData
            {
                X = c.x,
                Y = c.y,
                State = cell
            });
        }*/

            foreach (var b in buildings)
            {
                if (b.Destroy)
                {
                    G.BuildingFactory.DestroyBuilding(b.X, b.Y);
                }
            }
        }

    }
}