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
        [SerializeField] private List<Image>  CandidatesIcon;
        
        private CompositeDisposable _disposables;

        public void Init()
        {
            _disposables = new CompositeDisposable();

            G.SuccessionManager.OnCandidatesUpdated
                .Subscribe(UpdateView)
                .AddTo(_disposables);
        }

        private void UpdateView(List<SuccessorProfile> candidates)
        {
            for (int i = 0; i < CandidatesIcon.Count; i++)
            {
                if (i < candidates.Count)
                {
                    CandidatesIcon[i].sprite = candidates[i].portrait;
                    CandidatesIcon[i].gameObject.SetActive(true);
                }
                else
                {
                    CandidatesIcon[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
