using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "NewBuilding", menuName = "Data/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        [Header("Basic info")] 
        public string buildingId;
        public string buildingName;
        public Sprite buildingIcon;
        
        [Header("Placement Rules")]
        public TerrainMask allowedTerrainTypes;
        //public bool requiresSpecificNeighbor;
        public BuildingTag requiredNeighborTag;

        [Header("Resource Cost")] 
        public ResourceCost cost;
        
        [Header("Life Cycle")]
        public int lifeCycle;
        
        [Header("Effect by stage")]
        //public EffectDefinition stage1Effect;
        public EffectDefinition stage2Effect;
        public EffectDefinition stage3Effect;
        
        [Header("Tags")]
        public BuildingTag buildingTags;
    }
}
