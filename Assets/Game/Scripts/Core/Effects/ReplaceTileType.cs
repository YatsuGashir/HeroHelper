using System.Collections.Generic;
using Core.Effects;
using Data;
using GlobalSpace;
using UnityEngine;
using View;

public class ReplaceTileType: GameEffectBase
{
    public TerrainType tileType = TerrainType.Forest;

    [Tooltip("Во что превращать очищенную землю.")]
    public TerrainType targetType = TerrainType.Meadow;

    [Tooltip("Уничтожать ли здания, стоящие на этом лесу? (Опционально)")]
    public bool destroyBuildingsOnTop = false;
 public override void Apply(EffectContext context)
        {
            if (context.SourceBuilding == null) return;

            int x = context.SourceBuilding.x;
            int y = context.SourceBuilding.y;

            var targetsToUpdate = new List<(int cx, int cy)>();
            
            var neighbors = context.Grid.GetNeighbors(x, y);
            foreach (var cell in neighbors)
            {
                if (cell.terrainType == tileType)
                {
                    targetsToUpdate.Add((cell.x, cell.y));
                }
            }
            
            var selfCell = context.Grid.GetCell(x, y);
            if (selfCell != null && selfCell.terrainType == tileType)
            {
                targetsToUpdate.Add((x, y));
            }
            
            foreach (var coords in targetsToUpdate)
            {
                var cell = context.Grid.GetCell(coords.cx, coords.cy);
                if (cell == null) continue;
                
                if (destroyBuildingsOnTop && cell.building != null)
                {
                    Debug.Log($"[Эффект] Уничтожение здания на Клетке ({coords.cx}, {coords.cy})");
                    G.BuildingFactory.DestroyBuilding(coords.cx, coords.cy);
                    cell.building = null; 
                    cell.isOccupied = false;
                }
                
                cell.terrainType = targetType;

                Debug.Log($"[Эффект] Превращение ({coords.cx}, {coords.cy}) из {tileType} в {targetType}");


                G.Events.CellChanged.OnNext(new CellUpdateEventData
                {
                    X = coords.cx,
                    Y = coords.cy,
                    State = cell
                });
            }

        }
}
