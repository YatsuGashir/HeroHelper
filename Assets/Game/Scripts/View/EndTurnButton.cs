using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private CompositeDisposable _disposables;

        public void Init()
        {
            if (button == null) button = GetComponent<Button>();
            _disposables = new CompositeDisposable();
            
            button.onClick.AddListener(OnButtonClick);

            G.Events.TurnEnded
                .Subscribe(_ => OnTurnEnded())
                .AddTo(_disposables);
        }

        private void OnButtonClick()
        {
            button.interactable = false;

            G.Events.TurnEndRequested.OnNext(Unit.Default);
        }

        private void OnTurnEnded()
        {
            if (button != null)
                button.interactable = true;
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}