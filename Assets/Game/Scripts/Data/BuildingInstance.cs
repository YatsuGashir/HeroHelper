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
        public int age;
        
        private BuildingDefinition _definition;

        public void Initialize(BuildingDefinition definition, int xCord, int yCord)
        {
            buildingId = definition.buildingId;
            instanceId = Guid.NewGuid();
            x = xCord;
            y = yCord;

            stage = BuildingStage.Stage1;
            age = 0;
            
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
    }
}
