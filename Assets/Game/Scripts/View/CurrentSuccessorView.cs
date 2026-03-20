using Core.Successors;
using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CurrentSuccessorView : MonoBehaviour
    {
        [SerializeField] private RectTransform successorAnchor;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameObject _currentPortrait;

        public void Init(SuccessionManager  successionManager)
        {
            _disposables = new CompositeDisposable();
            
            successionManager.OnSuccessorChanged
                .Subscribe(SetPortrait)
                .AddTo(_disposables);
        }

        private void SetPortrait(SuccessorState successorState)
        {
            Destroy(_currentPortrait);
            _currentPortrait =G.SuccessorFaceBuilder.BuildSuccessor(successorState.CurrentProfile);
            _currentPortrait.transform.SetParent(successorAnchor, false);
            _currentPortrait.transform.localPosition = Vector3.zero;
        }
    }
}
