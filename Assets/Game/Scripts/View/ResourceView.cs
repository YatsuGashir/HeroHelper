using System;
using System.Collections.Generic;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;
using Data;
using DG.Tweening;

namespace View
{
    public class ResourceView : MonoBehaviour
    {
        [System.Serializable]
        public class ResourceSlot
        {
            public ResourceType Type;
            public GameObject TextGameObj;
            public GameObject RootObject;
            
            [NonSerialized]public TMP_Text TextValue;
            [NonSerialized]public RectTransform RectValue;
            
            public void CacheReferences()
            {
                TextValue = TextGameObj.GetComponent<TMP_Text>();
                RectValue = TextGameObj.GetComponent<RectTransform>();
            }
            public void InitText()
            { 
                TextValue.text = $"{Type}: 0";
            }
        }

        [SerializeField] private TMP_Text floatingTextPrefab;
        [SerializeField] private Vector3 floatingTextSpawnOffset = new Vector3(1f, 1f, 0f);
        [SerializeField] private Vector3 floatOffset = new Vector3(0f, 1f, 0f);
        [SerializeField] private float animDuration = 0.5f;
        
        [SerializeField] private List<ResourceSlot> _slots;

        private Dictionary<ResourceType, int> _previousValues = new Dictionary<ResourceType, int>();
        
        private CompositeDisposable _disposables;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
            
            foreach (var slot in _slots)
            {
                slot.CacheReferences();
                slot.InitText();
            }
        }

        public void Init()
        {
            Debug.Log("ResourceView Init");
            if (G.ResourceManager == null)
            {
                Debug.LogError("ResourceManager not found in G!");
                return;
            }
            
            G.ResourceManager.OnResourcesChanged
                .Subscribe(UpdateVisuals)
                .AddTo(_disposables);
            
            _previousValues = G.ResourceManager.GetResources();
            UpdateVisuals(G.ResourceManager.GetResources()); 
        }

        private void UpdateVisuals(Dictionary<ResourceType, int> resources)
        {
            foreach (var slot in _slots)
            {
                if (resources.TryGetValue(slot.Type, out int amount))
                {
                    int delta = amount - _previousValues[slot.Type];
                    if (delta != 0)
                    {
                        SpawnFloatingText(slot, delta);
                        PulseText(slot.RectValue);
                    }

                    slot.TextValue.text =slot.Type.ToString() + amount.ToString();
                    
                    if (slot.RootObject != null)
                        slot.RootObject.SetActive(true);
                }
            }
            _previousValues = resources;
        }
        
        private void SpawnFloatingText(ResourceSlot slot, int amount)
        {
            if (floatingTextPrefab == null || slot.TextValue == null) return;

            TMP_Text floatText = Instantiate(floatingTextPrefab, slot.TextValue.transform.parent);

            floatText.text = (amount > 0 ? "+" : "-") + amount;

            RectTransform rt = floatText.rectTransform;

            // КРИТИЧНО: сначала сбрасываем трансформ
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;

            // якорь в центр
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            // теперь позиция относительно текста ресурса
            rt.anchoredPosition = slot.RectValue.anchoredPosition + (Vector2)floatingTextSpawnOffset;

            var graphic = floatText.GetComponent<UnityEngine.UI.Graphic>();

            Sequence seq = DOTween.Sequence();

            seq.Append(graphic.DOFade(0f, animDuration));

            seq.Join(rt.DOAnchorPos(rt.anchoredPosition + (Vector2)floatOffset, animDuration)
                .SetEase(Ease.OutQuad));

            seq.OnComplete(() => Destroy(floatText.gameObject));
        }
        
        private void PulseText(RectTransform rectTransform)
        {
            if (rectTransform == null) return;
            
            rectTransform.DOKill();

            rectTransform.DOScale(1.1f, 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    rectTransform.DOScale(1f, 0.1f).SetEase(Ease.InQuad);
                });
        }
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
