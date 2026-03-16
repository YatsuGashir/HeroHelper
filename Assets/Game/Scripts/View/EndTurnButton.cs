using GlobalSpace;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace View
{
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            Debug.Log("Нажал на кнопку");
            //button.interactable = false;

            G.Events.TurnEndRequested.OnNext(Unit.Default);
        
            // Опционально: можно подписаться на TurnEnded, чтобы разблокировать кнопку потом
            /*G.Events.TurnEnded
                .Take(1) // Взять только первое событие
                .Subscribe(_ => button.interactable = true)
                .AddTo(this); // Отпишется автоматически при уничтожении кнопки*/
        }
    }
}
