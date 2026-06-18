using System;
using System.Collections.Generic;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;

namespace Core.Base
{
    public class ResourceManager
    {
        private Dictionary<ResourceType, int> _resources;

        private readonly Subject<Dictionary<ResourceType, int>> _onResourcesChanged = new Subject<Dictionary<ResourceType, int>>();
        
        public IObservable<Dictionary<ResourceType, int>> OnResourcesChanged => _onResourcesChanged;

        private readonly Subject<Unit> _onGameOver = new Subject<Unit>();
        public IObservable<Unit> OnGameOver => _onGameOver;
        
        private bool _isGameOver = false;
        
        private void TriggerGameOver()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            _onGameOver.OnNext(Unit.Default);
        }

        public ResourceManager()
        {
            ResetToStart();
        }

        public void ResetToStart()
        {
            _resources = new Dictionary<ResourceType, int>();
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                _resources[type] = 10;
            }
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (!_resources.ContainsKey(type)) return;

            _resources[type] += amount;
            
            _onResourcesChanged.OnNext(new Dictionary<ResourceType, int>(_resources));
        }

        public void AddResource(List<ResourceAmount> resourceAmount)
        {
            foreach (ResourceAmount resource in resourceAmount)
            {
                if (!_resources.ContainsKey(resource.Type)) continue;
            
                _resources[resource.Type] += resource.Amount;
            
                _onResourcesChanged.OnNext(new Dictionary<ResourceType, int>(_resources));
                Debug.Log("add res" + _resources);
            }

        }

        public void RemoveResource(ResourceType type, int amount)
        {
            if (!_resources.ContainsKey(type)) return;

            _resources[type] -= amount;
            
            if (_resources[type] < 0) _resources[type] = 0;
            
            _onResourcesChanged.OnNext(new Dictionary<ResourceType, int>(_resources));
        }

        public void RemoveResource(List<ResourceAmount> resourceAmount)
        {
            foreach (ResourceAmount resource in resourceAmount)
            {
                if (!_resources.ContainsKey(resource.Type)) continue;
            
                _resources[resource.Type] -= resource.Amount;
                if (_resources[resource.Type] < 0)
                {
                    TriggerGameOver();
                    _resources[resource.Type] = 0;
                }
                _onResourcesChanged.OnNext(new Dictionary<ResourceType, int>(_resources));
            }

        }
        
        public void ApplyChanges(List<ResourceAmount> changes)
        {
            if (changes == null || changes.Count == 0) return;

            var aggregatedChanges = new Dictionary<ResourceType, int>();

            foreach (var change in changes)
            {
                if (!aggregatedChanges.ContainsKey(change.Type))
                {
                    aggregatedChanges[change.Type] = 0;
                }
                aggregatedChanges[change.Type] += change.Amount;
            }

            bool hasAnyChange = false;

            foreach (var kvp in aggregatedChanges)
            {
                ResourceType type = kvp.Key;
                int delta = kvp.Value;

                if (!_resources.ContainsKey(type))
                {
                    Debug.LogWarning($"Попытка изменить неизвестный ресурс: {type}");
                    continue;
                }

                int newValue = _resources[type] + delta;

                if (newValue < 0)
                {
                    TriggerGameOver();
                    Debug.Log($"Ресурс {type} не может быть отрицательным ({_resources[type]} + {delta}). Установлено в 0.");
                    newValue = 0;

                }

                if (_resources[type] != newValue)
                {
                    _resources[type] = newValue;
                    hasAnyChange = true;
                }
            }

            if (hasAnyChange)
            {
                _onResourcesChanged.OnNext(new Dictionary<ResourceType, int>(_resources));
            }
        }
        
        public bool TrySpend(ResourceCost cost)
        {
            if (!HasEnough(cost)) return false;

            foreach (var item in cost.costs)
            {
                RemoveResource(item.Type, item.Amount);
            }
            return true;
        }

        public bool HasEnough(ResourceCost cost)
        {
            foreach (var item in cost.costs)
            {
                if (_resources[item.Type] < item.Amount)
                    return false;
            }
            return true;
        }

        public int GetAmount(ResourceType type)
        {
            return _resources.ContainsKey(type) ? _resources[type] : 0;
        }

        public Dictionary<ResourceType, int> GetResources()
        {
            return _resources;
        }
        
        public void Dispose()
        {
            _onResourcesChanged.Dispose();
        }
    }
}
