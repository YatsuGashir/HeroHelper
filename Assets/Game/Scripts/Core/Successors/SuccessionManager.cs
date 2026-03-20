using System.Collections.Generic;
using System.Linq;
using Data;
using Core.Successors;
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
        private SuccessionSelector _pendingSelection;
        private List<SuccessorProfile> _currentCandidates = new List<SuccessorProfile>();
        public IReadOnlyList<SuccessorProfile> CurrentCandidates => _currentCandidates;
        
        public Subject<SuccessorState> OnSuccessorChanged = new Subject<SuccessorState>();
        public Subject<SuccessionSelector> OnSelectionReady = new Subject<SuccessionSelector>();
        public Subject<List<SuccessorProfile>> OnCandidatesUpdated = new Subject<List<SuccessorProfile>>();
        public SuccessorState CurrentState => _currentState;
        
        public SuccessionManager()
        {
            _historyProfiles = new List<SuccessorProfile>();
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            _availableProfiles = G.Config.allSuccessors;
            foreach (var profile in _availableProfiles)
            {
                profile.GenerateVisualSeed();
            }
        }
        
         public void StartFirstRun()
        {

            var starter = _availableProfiles[0];
            StartSuccession(starter, 1);
            
            GenerateNewCandidates();
        }

        private void GenerateNewCandidates()
        {
            _currentCandidates = PickRandomCandidates(3);
            OnCandidatesUpdated.OnNext(_currentCandidates);
        }
        public void StartNextSuccession(SuccessorProfile forcedProfile = null)
        {
            if (forcedProfile != null)
            {
                if (_currentState != null)
                {
                    _currentState.Deactivate();
                    _historyProfiles.Add(_currentState.CurrentProfile);
                }

                StartSuccession(forcedProfile, (_currentState?.GenerationNumber ?? 0) + 1);
                
            }
            else
            {
                RequestSuccessorSelection();
                GenerateNewCandidates();
            }
        }
        
        public void RequestSuccessorSelection()
        {
            if (_pendingSelection != null && !_pendingSelection.IsResolved)
            {
                Debug.LogWarning("[SuccessionManager] Selection already pending!");
                return;
            }

            _pendingSelection = new SuccessionSelector();
            _pendingSelection.SetCandidates(_currentCandidates);

            OnSelectionReady.OnNext(_pendingSelection);
        }
        
        public void ConfirmSelection(SuccessorProfile chosen)
        {
            if (_pendingSelection == null || _pendingSelection.IsResolved)
            {
                return;
            }

            _pendingSelection.Choose(chosen);
            StartSuccession(chosen, (_currentState?.GenerationNumber ?? 0) + 1);
            _pendingSelection = null;
        }
        private List<SuccessorProfile> PickRandomCandidates(int count)
        {
            var excluded = new HashSet<SuccessorProfile>();

            if (_currentState?.CurrentProfile != null)
                excluded.Add(_currentState.CurrentProfile);

            foreach (var profile in _historyProfiles.Take(2))
                excluded.Add(profile);

            var pool = _availableProfiles.Where(p => !excluded.Contains(p)).ToList();

            if (pool.Count < count)
                pool = _availableProfiles.Where(p => p != _currentState?.CurrentProfile).ToList();

            var shuffled = pool.OrderBy(x => Random.value).Take(count).ToList();

            if (shuffled.Count == 0 && _availableProfiles.Count > 0)
                shuffled.Add(_availableProfiles[Random.Range(0, _availableProfiles.Count)]);
                
            return shuffled;
        }

        public void CanDeath()
        {
            _currentState.CurrentLifeTime--;
            Debug.Log($"Новому наследнику осталось {_currentState.CurrentLifeTime} хода");

            if (_currentState.CurrentLifeTime == 0)
            {
                StartNextSuccession();
            }
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
