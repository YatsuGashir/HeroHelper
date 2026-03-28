using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SuccessorProfile", menuName = "Data/SuccessorProfile")]
    public class SuccessorProfile : ScriptableObject
    {
        public string successorId;
        public string successorName;
        //public Sprite portrait;
        
        public int FaceSeed { get; private set; }
        public int NameSeed { get; private set; }
        
        public void GenerateVisualSeed()
        {
            if (FaceSeed == 0)
            {
                FaceSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
        }

        public void GenerateNameSeed()
        {
            if (NameSeed == 0)
            {
                NameSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            }
        }
        
        public void GenerateAll()
        {
            GenerateVisualSeed();
            GenerateNameSeed();
            successorName = SuccessorNameGenerator.GenerateName(NameSeed);
        }
        
        public int timeToDeath;
        
        public List<BuildingDefinition> successorDeck;
        
        public EffectDefinition SuccessorEffect;
    
    }
}
