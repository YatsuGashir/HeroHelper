using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("References")]
        public List<BuildingDefinition> allBuildings;
        public List<SuccessorProfile> allSuccessors;
        
        [Header("Map generation")]
        public TerrainGenerationParams terrainGenerationParams;
    
        [Header("Grid Settings")]
        public int gridWidth;
        public int gridHeight;
    
        [Header("Economy")]
        public int baseHandSize;
        public int cardsDrawnPerTurn;
    
        [Header("Difficulty Scaling")]
        public float disasterChanceIncreasePerTurn;
        public int turnsUntilFirstDisaster;
    }
}
