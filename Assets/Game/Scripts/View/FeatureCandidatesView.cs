using System.Collections.Generic;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class FeatureCandidatesView : MonoBehaviour
    {
        [SerializeField] private List<RectTransform>  CandidatesIconAnchor;
        
        private CompositeDisposable _disposables;
        private List<GameObject> _currentPortrait = new List<GameObject>();

        public void Init()
        {
            _disposables = new CompositeDisposable();

            _currentPortrait = new List<GameObject>(new GameObject[CandidatesIconAnchor?.Count ?? 0]);
            
            G.SuccessionManager.OnCandidatesUpdated
                .Subscribe(UpdateView)
                .AddTo(_disposables);
        }

        private void UpdateView(List<SuccessorProfile> candidates)
        {
            for (int i = 0; i <  CandidatesIconAnchor.Count; i++)
            {
                if (i < candidates.Count)
                {
                    Destroy(_currentPortrait[i]);
                    _currentPortrait[i] =G.SuccessorFaceBuilder.BuildSuccessor(candidates[i]);
                    _currentPortrait[i].transform.SetParent(CandidatesIconAnchor[i],  false);
                    _currentPortrait[i].transform.localPosition = Vector3.zero;
                    CandidatesIconAnchor[i].gameObject.SetActive(true);
                }
                else
                {
                    CandidatesIconAnchor[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
