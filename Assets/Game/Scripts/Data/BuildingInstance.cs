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

        public BuildingDefinition Definition
        {
            get { return _definition; }
        }

        public void Initialize(BuildingDefinition definition, int xCord, int yCord)
        {
            buildingId = definition.buildingId;
            instanceId = Guid.NewGuid();
            x = xCord;
            y = yCord;

            
            remaingTime = definition.lifeCycle;
            if (remaingTime > 0)
            {
                stage = BuildingStage.Stage2;
            }
            else
            {
                stage = BuildingStage.Stage3;
            }
            
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
                BuildingStage.Stage3 => def.stage3Effect,
                _ => null
            };
        }

        public bool Tick()
        {
            if (stage != BuildingStage.Stage2) 
                return false;

            int delta = 1;

            delta += G.TickModifierManager.GetLifeDelta(this);

            remaingTime -= delta;

            if (remaingTime > 0)
            {
                G.Events.Ticked.OnNext(this);
                return false;
            }

            stage = BuildingStage.Stage3;

            G.Events.Ticked.OnNext(this);
            G.Events.BuildingStageChanged.OnNext(this);

            return true;
        }
    }
}
