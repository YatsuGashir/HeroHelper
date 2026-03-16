using System.Collections.Generic;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;
using Data;

namespace View
{
    public class ResourceView : MonoBehaviour
    {
 [System.Serializable]
        public class ResourceSlot
        {
            public ResourceType Type;
            public TMP_Text TextValue;
            public GameObject RootObject;
        }

        [SerializeField] private List<ResourceSlot> _slots;
        
        private CompositeDisposable _disposables;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
            
            foreach (var slot in _slots)
            {
                if (slot.TextValue != null) slot.TextValue.text= slot.Type.ToString() + "0";
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
            
            UpdateVisuals(G.ResourceManager.GetResources()); 
        }

        private void UpdateVisuals(Dictionary<ResourceType, int> resources)
        {
            foreach (var slot in _slots)
            {
                if (resources.TryGetValue(slot.Type, out int amount))
                {
                    if (slot.TextValue != null)
                    {
                        slot.TextValue.text =slot.Type.ToString() + amount.ToString();
                        
                        // анимация
                    }
                    
                    if (slot.RootObject != null)
                        slot.RootObject.SetActive(true);
                }
            }
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }
    }
}
