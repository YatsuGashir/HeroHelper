using Core.Successors;
using Cysharp.Threading.Tasks;
using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CurrentSuccessorView : MonoBehaviour
    {
        [SerializeField] private RectTransform successorAnchor;
        [SerializeField] private SuccessorDiedAnimation  diedAnimation;
        [SerializeField] private CoronationAnimation coronationAnimation;
        [SerializeField] private TextFader textFader;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameObject _currentPortrait;

        private bool _isFirstRun = false;

        public void Init(SuccessionManager  successionManager)
        {
            _disposables = new CompositeDisposable();
            
            successionManager.OnSuccessorChanged
                .Subscribe(SetPortrait)
                .AddTo(_disposables);
        }

        private async void SetPortrait(SuccessorState successorState)
        {

            if (_currentPortrait != null)
            {
                var portraitRect = _currentPortrait.GetComponent<RectTransform>();
                await diedAnimation.PlayAnimation(portraitRect, textFader);
                Destroy(_currentPortrait);
                _currentPortrait = null;
                _isFirstRun = false;
            }
            
            _currentPortrait = G.SuccessorFaceBuilder.BuildSuccessor(successorState.CurrentProfile);
            _currentPortrait.transform.SetParent(successorAnchor, false);
            
            if(!_isFirstRun)
                await coronationAnimation.PlayCoronation(_currentPortrait, textFader);
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
            
            // Очищаем портрет при уничтожении объекта
            if (_currentPortrait != null) Destroy(_currentPortrait);
        }
    }
}
