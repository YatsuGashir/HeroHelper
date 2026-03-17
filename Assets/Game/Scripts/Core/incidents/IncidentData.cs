using System.Collections.Generic;
using Core.Effects;
using Data;
using UnityEngine;

namespace Core.incidents
{
    [CreateAssetMenu(fileName = "incidentData", menuName = "Data/incidentData")]
    public class IncidentData : ScriptableObject
    {
        [Header("Info")]
        public string incidentId;
        public string incidentName;
        public string description;
        
        [Header("Type & Timing")]
        public IncidentType durationType;
        
        public int turnsUntilTrigger; 
        public float chancePerTurn; 

        [Header("Consequences (The Penalty)")]
        [Tooltip("Что заберут или сломают, когда время выйдет (или сразу для коротких)")]
        public List<ResourceAmount> resourceLoss;
        
        [Tooltip("Сломать случайное здание? (опционально)")]
        public bool destroyRandomBuilding;
        
        [Tooltip("Максимум зданий к уничтожению")]
        public int maxBuildingsToDestroy = 1;
        
        [Tooltip("Урон по времени жизни зданий (отрицательное число)")]
        public int buildingLifetimeDamage; 
    }
    

}
public enum IncidentTrigger
{
    TurnBased,
    ResourceThreshold,
    Random
}
public enum IncidentType
{
    Instant,
    Incoming 
}
