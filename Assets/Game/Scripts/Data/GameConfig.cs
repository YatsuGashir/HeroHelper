using System.Collections.Generic;
using Core.incidents;
using Data;
using UnityEngine;
using View;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("References")]
        public List<BuildingDefinition> allBuildings;
        public List<SuccessorProfile> allSuccessors;
        public List<IncidentData> allIncidents;
        
        [Header("Map generation")]
        public TerrainGenerationParams terrainGenerationParams;
    
        [Header("Grid Settings")]
        public int gridWidth;
        public int gridHeight;

        [Header("Successor Look variants")] 
        public List<Sprite> headSprites;
        public List<Sprite> faceSprites;
        public List<Sprite> bodySprites;
        
        [SerializeField] public WandererView wandererPrefab;
        [SerializeField] public int initialWandererCount = 5;

        //[Header("Economy")]
        //public int baseHandSize;
        //public int cardsDrawnPerTurn;

        //[Header("Difficulty Scaling")]
        //добавить связь с событиями
    }
}
