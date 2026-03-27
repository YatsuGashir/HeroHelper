using Data;
using GlobalSpace;
using UnityEngine;
using View;

namespace Core.Factories
{
    public class BuildingFactory
    {
        public bool CanPlaceBuilding(BuildingDefinition definition, int x, int y)
        {
            if (definition == null)
            {
                Debug.Log("Нет здания");
                return false;
            }

            var cell = G.GridSystem.GetCell(x, y);
            if (cell == null)
            {
                Debug.Log("Клетка нуль");
                return false;
            }

            if (cell.isOccupied || cell.building != null)
            {
                Debug.Log("Клетка занята или стоит здание");
                return false;
            }
            
            bool terrainAllowed = definition.allowedTerrainTypes.Contains(cell.terrainType);

            if (!terrainAllowed)
            {
                Debug.Log("Не тот тип клетки");
                return false;
            }

            // добавить проверку соседей
            
            return true;
        }
        
        private int _wandererCounter = 0;

        public BuildingInstance CreateBuilding(BuildingDefinition definition, int x, int y)
        {
            if (!CanPlaceBuilding(definition, x, y))
            {
                Debug.LogWarning($"Cannot place {definition.buildingName} at [{x},{y}]");
                return null;
            }
            G.TutorialSeq.placeFirstBuilding = true;

            var cell = G.GridSystem.GetCell(x, y);
            G.GridSystem.SetCellTerrain(x,y,TerrainType.Meadow);
            var instance = new BuildingInstance();
            instance.Initialize(definition, x, y);
            
            int bonus = G.LifetimeBonusManager.GetLifetimeBonusForBuilding(instance);
            if (bonus != 0)
            {
                instance.remaingTime += bonus;
        
                Debug.Log($"[Bonus] {definition.buildingName} получил {bonus:+#;-#} к жизни. Осталось: {instance.remaingTime}");
            }
            
            
            if (_wandererCounter == 0)
            {
                _wandererCounter = 0;
                G.WandererManager.SpawnWandererAt(x, y);
            }
            else
            {
                _wandererCounter++;
            }

            cell.building = instance;
            cell.isOccupied = true;
            
            G.Events.CellChanged.OnNext(new CellUpdateEventData
            {
                X = x,
                Y = y,
                State = cell
            });

            Debug.Log($"Built: {definition.buildingName} at [{x},{y}]");
            return instance;
        }


        public void DestroyBuilding(int x, int y, bool isTransformation = false)
        {
            var cell = G.GridSystem.GetCell(x, y);
            if (cell == null || cell.building == null) return;

            var building = cell.building;
            

            cell.building = null;
            cell.isOccupied = false;

            
            G.Events.CellChanged.OnNext(new CellUpdateEventData
            {
                X = x,
                Y = y,
                State = cell
            });
            
            G.Events.BuildingDied.OnNext(building);
        }
    }
}
