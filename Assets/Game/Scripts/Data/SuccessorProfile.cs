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
        public Sprite portrait;
        
        public List<BuildingDefinition> successorDeck;
        
        //баффы и дебаФФЫ
    
    }
}
