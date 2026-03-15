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
            public Sprite sprite;
        }

        public List<SpriteEntry> terrainSprites;

        public Sprite GetTerrainSprite(TerrainType type)
        {
            var entry = terrainSprites.Find(e => e.type == type);
            return entry?.sprite;
        }
    }
}
