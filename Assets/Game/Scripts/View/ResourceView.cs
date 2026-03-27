// File: View/ResourceView.cs (обновлённый)
using System;
using System.Collections.Generic;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;
using Data;
using DG.Tweening;
using Core.Base; // 🔥 Для ResourcePreviewManager

namespace View
{
    public class ResourceView : MonoBehaviour
    {
        [System.Serializable]
        public class ResourceSlot
        {
            public ResourceType Type;
            public GameObject TextGameObj;      // Основной текст: "10"
            public GameObject ForecastTextObj;  // 🔥 Новый: текст прогноза: "→ +5 = 15"
            public GameObject RootObject;
            
            [NonSerialized] public TMP_Text TextValue;
            [NonSerialized] public TMP_Text ForecastText; // 🔥
            [NonSerialized] public RectTransform RectValue;
            [NonSerialized] public RectTransform ForecastRect; // 🔥
            
            public void CacheReferences()
            {
                TextValue = TextGameObj?.GetComponent<TMP_Text>();
                ForecastText = ForecastTextObj?.GetComponent<TMP_Text>(); // 🔥
                RectValue = TextGameObj?.GetComponent<RectTransform>();
                ForecastRect = ForecastTextObj?.GetComponent<RectTransform>(); // 🔥
            }
            
            public void InitText()
            { 
                TextValue.text = "0";
                ForecastText?.SetText(""); // 🔥 Пусто по умолчанию
            }
            
            // 🔥 Обновление отображения с прогнозом
            public void UpdateDisplay(ResourceForecast forecast)
            {
                TextValue.text = forecast.Current.ToString();
                
                if (ForecastText != null)
                {
                    if (forecast.Delta != 0)
                    {
                        ForecastText.text = forecast.GetDisplayString();
                        ForecastTextObj.SetActive(true);
                        
                        // 🔥 Анимация при изменении прогноза
                        if (ForecastRect != null)
                            PulseText(ForecastRect);
                    }
                    else
                    {
                        ForecastTextObj.SetActive(false);
                    }
                }
            }
            
            private void PulseText(RectTransform rect)
            {
                rect.DOKill();
                rect.DOScale(1.15f, 0.12f).SetEase(Ease.OutQuad)
                    .OnComplete(() => rect.DOScale(1f, 0.12f).SetEase(Ease.InQuad));
            }
        }

        [Header("Floating Text")]
        [SerializeField] private TMP_Text floatingTextPrefab;
        [SerializeField] private Vector3 floatingTextSpawnOffset = new Vector3(1f, 1f, 0f);
        [SerializeField] private Vector3 floatOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private float animDuration = 0.5f;
        
        [Header("Slots")]
        [SerializeField] private List<ResourceSlot> _slots;

        private Dictionary<ResourceType, int> _previousValues = new();
        private ResourcePreviewManager _previewManager; // 🔥
        private CompositeDisposable _disposables;
        private bool _isFirstPlay = true;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
            
            foreach (var slot in _slots)
            {
                slot.CacheReferences();
                slot.InitText();
            }
        }

        // В ResourceView, после Awake():
        private void Start()
        {
            Debug.Log($"[ResourceView Debug] Slots count: {_slots?.Count ?? 0}");
            for (int i = 0; i < _slots?.Count; i++)
            {
                var slot = _slots[i];
                Debug.Log($"  Slot {i} ({slot.Type}):" +
                          $"\n    TextGameObj: {slot.TextGameObj?.name ?? "NULL"}" +
                          $"\n    TextValue: {slot.TextValue != null}" +
                          $"\n    ForecastTextObj: {slot.ForecastTextObj?.name ?? "NULL"}" +
                          $"\n    ForecastText: {slot.ForecastText != null}");
            }
        }
        public void Init()
        {
            _isFirstPlay = true;
            _previewManager = G.ResourcePreviewManager; // 🔥 Получаем менеджер
            
            if (G.ResourceManager == null)
            {
                Debug.LogError("ResourceManager not found!");
                return;
            }
            
            // 🔥 Подписка на реальные изменения ресурсов
            G.ResourceManager.OnResourcesChanged
                .Subscribe(UpdateActualValues)
                .AddTo(_disposables);
            
            // 🔥 Подписка на обновление прогноза
            if (_previewManager != null)
            {
                _previewManager.OnForecastUpdated
                    .Subscribe(UpdateForecastDisplay)
                    .AddTo(_disposables);
                
                // 🔥 Инициализация прогноза
                _previewManager.RecalculateForecast();
            }
            
            _previousValues = G.ResourceManager.GetResources();
            UpdateActualValues(_previousValues);
        }

        // 🔥 Обновление только текущих значений (без прогноза)
        private void UpdateActualValues(Dictionary<ResourceType, int> resources)
        {
            if (!_isFirstPlay)
            {
                AudioManager.Instance.PlaySFX("ResourcePlus", 0.1f);
            }
            else
            {
                _isFirstPlay = false;
            }
            
            foreach (var slot in _slots)
            {
                if (resources.TryGetValue(slot.Type, out int amount))
                {
                    int delta = amount - _previousValues.GetValueOrDefault(slot.Type, 0);
                    
                    if (delta != 0 && !_isFirstPlay)
                    {
                        SpawnFloatingText(slot, delta);
                        if (slot.RectValue != null)
                            PulseText(slot.RectValue);
                    }

                    slot.TextValue.text = amount.ToString();
                    slot.RootObject?.SetActive(true);
                }
            }
            _previousValues = new Dictionary<ResourceType, int>(resources);
        }

        // 🔥 Обновление отображения прогноза
        private void UpdateForecastDisplay(Dictionary<ResourceType, ResourceForecast> forecasts)
        {
            foreach (var slot in _slots)
            {
                if (forecasts.TryGetValue(slot.Type, out var forecast))
                {
                    slot.UpdateDisplay(forecast); // 🔥 Использует новый метод
                }
            }
        }
        
        // 🔥 Публичный метод для принудительного обновления (если нужно)
        public void RefreshForecast() => _previewManager?.RecalculateForecast();
        
        private void SpawnFloatingText(ResourceSlot slot, int amount) { /* ваш код */ }
        private void PulseText(RectTransform rect) { /* ваш код */ }
        
        private void OnDestroy() => _disposables?.Dispose();
    }
}