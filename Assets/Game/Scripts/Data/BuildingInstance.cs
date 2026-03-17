using System;
using GlobalSpace;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class BuildingInstance
    {
        public string buildingId;
        public Guid instanceId;
        public int x, y;
        
        public BuildingStage stage;
        public int remaingTime;
        
        private BuildingDefinition _definition;

        public void Initialize(BuildingDefinition definition, int xCord, int yCord)
        {
            buildingId = definition.buildingId;
            instanceId = Guid.NewGuid();
            x = xCord;
            y = yCord;

            stage = BuildingStage.Stage2;
            remaingTime = definition.lifeCycle;
            
            _definition = definition;
        }

        public BuildingDefinition GetDefinition()
        {
            return _definition;
        }

        public EffectDefinition GetEffectDefinition()
        {
            var def = GetDefinition();
            return stage switch
            {
                //BuildingStage.Stage1 => def.stage1Effect,
                BuildingStage.Stage2 => def.stage2Effect,
                BuildingStage.Stage3Trasformation => def.stage3Effect,
                _ => null
            };
        }

        public bool Tick()
        {
            Debug.Log("Тик идёт");
            if (stage != BuildingStage.Stage2) 
                return false;
    
            if (remaingTime > 0)
            {
                Debug.Log("Осталось "+ remaingTime);
                remaingTime--;
                G.Events.Ticked.OnNext(this);
                return false;
            }
            
            stage = BuildingStage.Stage3Trasformation;
    
            Debug.Log($"[Building] Stage 2 completed at ({x},{y}), moving to Stage 3");
            G.Events.Ticked.OnNext(this);
            G.Events.BuildingStageChanged.OnNext(this);
    
            return true; 
        }
    }
}
