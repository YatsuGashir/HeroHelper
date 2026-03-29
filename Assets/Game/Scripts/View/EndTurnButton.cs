using DG.Tweening;
using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField] private ClockView  _clockAnimator;
        [SerializeField] private float _postAnimationDelay = 0.3f;
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

            G.TutorialSeq.turnPushing = true;
            button.interactable = false;

            G.Events.TurnEndRequested.OnNext(Unit.Default);
            _clockAnimator.PlayEndTurnAnimation(() =>
            {
                // 3. После анимации — небольшая пауза для акцента
                DOTween.To(() => 0, x => {}, 1, _postAnimationDelay)
                    .OnComplete(() =>
                    {

                    });
            });
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