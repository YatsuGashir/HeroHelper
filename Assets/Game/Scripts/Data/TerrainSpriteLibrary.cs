using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "TerrainSpriteLibrary", menuName = "Scriptable Objects/TerrainSpriteLibrary")]
    public class TerrainSpriteLibrary : ScriptableObject
    {
        [System.Serializable]
        public class SpriteEntry
        {
            public TerrainType type;
            public List<Sprite> sprite;           // Обычные спрайты
            public List<Sprite> autoTileSprites;  // 16 спрайтов для autotile (индекс 0-15)
            public bool useAutotile;              // Флаг: использовать autotile
        }

        public List<SpriteEntry> terrainSprites;

        public Sprite GetTerrainSprite(TerrainType type)
        {
            var entry = terrainSprites.Find(e => e.type == type);
            if (entry == null || entry.sprite == null || entry.sprite.Count == 0) return null;
            return entry.sprite[Random.Range(0, entry.sprite.Count)];
        }

        public Sprite GetAutotileSprite(TerrainType type, int autotileIndex)
        {
            var entry = terrainSprites.Find(e => e.type == type && e.useAutotile);
            if (entry == null || entry.autoTileSprites == null) return null;
            
            autotileIndex = Mathf.Clamp(autotileIndex, 0, entry.autoTileSprites.Count - 1);
            return entry.autoTileSprites[autotileIndex];
        }
        

        
        // Типы для базового рендерера
        public bool IsBaseTerrain(TerrainType type) => 
            type == TerrainType.Meadow || type == TerrainType.Water;

        // Типы для оверлей-рендерера
        public bool IsOverlayTerrain(TerrainType type) => 
            type == TerrainType.Stone || 
            type == TerrainType.Crystal || type == TerrainType.Grass;
    }
}