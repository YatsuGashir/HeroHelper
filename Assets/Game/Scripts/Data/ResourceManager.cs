using System;
using System.Collections.Generic;
using Data;
using UniRx;
using UnityEngine;

namespace Core.Base
{
    public class ResourceManager
    {
        private Dictionary<ResourceType, int> _resources;

        private readonly Subject<Dictionary<ResourceType, int>> _onResourcesChanged = new Subject<Dictionary<ResourceType, int>>();
        
        public IObservable<Dictionary<ResourceType, int>> OnResourcesChanged => _onResourcesChanged;

        public ResourceManager()
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
                if (_resources[resource.Type] < 0) _resources[resource.Type] = 0;
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
