using System;

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

            stage = BuildingStage.Stage1;
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
                BuildingStage.Stage1 => def.stage1Effect,
                BuildingStage.Stage2 => def.stage2Effect,
                BuildingStage.Stage3Trasformation => def.stage3Effect,
                _ => null
            };
        }

        public bool Tick()
        {
            remaingTime--;
            
            bool stageChanged = false;
            
            
            if (stage == BuildingStage.Stage2)
            {
                remaingTime--;
            }

            if (stage == BuildingStage.Stage1)
            {
                stage = BuildingStage.Stage2;
                stageChanged = true;
            }

            if (remaingTime <= 0)
            {
                stage = BuildingStage.Stage3Trasformation;
                stageChanged = true;
            }
            return stageChanged;
            
        }
    }
}
