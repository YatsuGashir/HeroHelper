using System.Collections.Generic;
using System.Linq;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;

namespace Core.Successors
{
    public class SuccessionManager
    {
        private SuccessorState _currentState;
        private List<SuccessorProfile> _availableProfiles; 
        private List<SuccessorProfile> _historyProfiles;
        
        
        public Subject<SuccessorState> OnSuccessorChanged = new Subject<SuccessorState>();
        public SuccessorState CurrentState => _currentState;
        
        public SuccessionManager()
        {
            _historyProfiles = new List<SuccessorProfile>();
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            _availableProfiles = G.Config.allSuccessors;

        }
        
         public void StartFirstRun()
        {

            var starter = _availableProfiles[0];
            StartSuccession(starter, 1);
        }


        public void StartNextSuccession(SuccessorProfile forcedProfile = null)
        {
            if (_currentState != null)
            {
                _currentState.Deactivate();
                _historyProfiles.Add(_currentState.CurrentProfile);
            }

            SuccessorProfile next;
            if (forcedProfile != null)
            {
                next = forcedProfile;
            }
            else
            {
                var candidates = _availableProfiles.Where(p => p != _currentState?.CurrentProfile).ToList();
                next = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            }

            StartSuccession(next, (_currentState?.GenerationNumber ?? 0) + 1);
        }

        private void StartSuccession(SuccessorProfile profile, int generation)
        {
            _currentState = new SuccessorState();
            _currentState.Initialize(profile, generation);

            OnSuccessorChanged.OnNext(_currentState);
            Debug.Log($"=== НОВЫЙ НАСЛЕДНИК: {profile.successorName} (Поколение {generation}) ===");
            
        }


/*
        public ResourceCost GetModifiedCost(ResourceCost originalCost)
        {

        }


        public int GetModifiedProductionAmount(int baseAmount)
        {
            if (_currentState == null) return baseAmount;
            return Mathf.CeilToInt(baseAmount * _currentState.CurrentProfile.globalProductionMultiplier);
        }*/

        public List<BuildingDefinition> GetStartingHandIds()
        {
            return _currentState?.CurrentProfile.successorDeck ?? new List<BuildingDefinition>();
        }

    }
}
