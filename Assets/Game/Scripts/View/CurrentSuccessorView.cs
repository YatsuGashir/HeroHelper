using Core.Successors;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CurrentSuccessorView : MonoBehaviour
    {
        [SerializeField] private Image successorImage;
        
        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Init(SuccessionManager  successionManager)
        {
            _disposables = new CompositeDisposable();
            
            successionManager.OnSuccessorChanged
                .Subscribe(SetPortrait)
                .AddTo(_disposables);
        }

        private void SetPortrait(SuccessorState successorState)
        {
            successorImage.sprite = successorState.CurrentProfile.portrait;
        }
    }
}
