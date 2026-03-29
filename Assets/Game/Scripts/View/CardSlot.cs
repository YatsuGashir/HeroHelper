using Data;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CardSlot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _spawnPoint; // Точка спавна карты

        public bool IsOccupied { get; private set; }
        public BuildingDefinition AssignedCard { get; private set; }

        /// <summary>
        /// Занять слот картой
        /// </summary>
        public void AssignCard(Image cardPrefab, BuildingDefinition cardData)
        {
            if (IsOccupied)
            {
                Debug.LogWarning($"[CardSlot] Slot {name} already occupied!");
                return;
            }

            IsOccupied = true;
            AssignedCard = cardData;

            if (cardPrefab != null && _spawnPoint != null)
            {
                var cardInstance = Instantiate(cardPrefab, _spawnPoint);
                cardInstance.transform.localPosition = Vector3.zero;
                cardInstance.sprite = cardData?.buildingCardICon;
                cardInstance.name = $"Card_{cardData?.name}";
                
                // Добавляем логику карты
                var cardView = cardInstance.GetComponent<CardView>();
                if (cardView == null) cardView = cardInstance.AddComponent<CardView>();
                cardView.Init(cardData);
                
                // 🔥 БЕЗОПАСНОЕ получение CanvasGroup (исправляет MissingComponentException)
                var canvasGroup = cardInstance.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = cardInstance.AddComponent<CanvasGroup>();
                }
                
                // Анимация появления
                canvasGroup.alpha = 0f;
                DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 1f, 0.2f)
                    .SetEase(Ease.OutQuad);
                
                // Регистрируем для взаимодействия
                GlobalSpace.G.PlacementManager?.RegisterCard(cardView);
            }
        }

        /// <summary>
        /// Освободить слот
        /// </summary>
        public void Clear()
        {
            IsOccupied = false;
            AssignedCard = null;
            
            // Удаляем дочерние объекты (карты), но не сам слот
            if (_spawnPoint != null)
            {
                foreach (Transform child in _spawnPoint)
                    Destroy(child.gameObject);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsOccupied ? Color.green : new Color(1, 0.5f, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            UnityEditor.Handles.Label(transform.position, $"Slot: {name}");
        }
#endif
    }
}