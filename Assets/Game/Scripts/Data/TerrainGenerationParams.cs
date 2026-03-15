using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class TerrainGenerationParams
    {
        public int seed = 1;

        [Header("Base terrain (meadow / forest)")]
        public float baseNoiseScale = 0.08f;
        public float forestThreshold = 0.6f;

        [Header("Water")]
        public float waterNoiseScale = 0.03f;
        public float waterThreshold = 0.75f;

        [Header("Stone clusters")]
        public float stoneNoiseScale = 0.07f;
        public float stoneThreshold = 0.72f;

        public int stoneClusterMin = 5;
        public int stoneClusterMax = 6;

        [Header("Crystal")]
        [Range(0f,1f)]
        public float crystalChance = 0.15f;
    }
}