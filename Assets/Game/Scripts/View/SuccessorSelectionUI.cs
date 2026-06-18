using System;
using System.Collections.Generic;
using Core.Successors;
using Data;
using Core.Successors;
using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class SuccessorSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _selectionPanel;
        [SerializeField] private SuccessorCardView _cardPrefab;
        [SerializeField] private List<Transform> _cardsContainers;
        [SerializeField] private GameObject nextTurnButton;
        
        private SuccessionManager _successionManager;
        private CompositeDisposable _disposables;

        public void Init()
        {
            _successionManager = G.SuccessionManager;
            _disposables = new CompositeDisposable();
            _successionManager.OnSelectionReady
                .Subscribe(OnSelectionReady)
                .AddTo(_disposables);
                
            _selectionPanel.SetActive(false);
        }

        private void OnSelectionReady(SuccessionSelector selection)
        {
            ClearCards();
            int index = 0;
            foreach (var candidate in selection.Candidates)
            {
                var card = Instantiate(_cardPrefab, _cardsContainers[index]);
                card.Setup(candidate);
                
                card.OnSelected
                    .Subscribe(_ => OnCandidateChosen(candidate))
                    .AddTo(_disposables);
                index++;
            }
            
            _selectionPanel.SetActive(true);
            nextTurnButton.SetActive(false);
            //Time.timeScale = 0;
        }

        private void OnCandidateChosen(SuccessorProfile chosen)
        {
            _successionManager.ConfirmSelection(chosen);
            _selectionPanel.SetActive(false);
            nextTurnButton.SetActive(true);
            //Time.timeScale = 1; 
        }
        
        private void ClearCards()
        {
            foreach (Transform container in _cardsContainers)
            {
                for (int i = container.childCount - 1; i >= 0; i--)
                {
                    Destroy(container.GetChild(i).gameObject);
                }

            }
        }

        private void OnDestroy() => _disposables?.Dispose();
    }
}