// File: Core/Base/ResourcePreviewManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Effects;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;

namespace Core.Base
{
    public class ResourcePreviewManager
    {
        private readonly EffectResolver _resolver;
        private readonly GridSystem _grid;
        private readonly ResourceManager _resourceManager;
        
        // 🔥 Текущий прогноз по типам ресурсов
        private Dictionary<ResourceType, int> _currentForecast;
        
        // 🔥 Событие для UI: новый прогноз готов
        private readonly Subject<Dictionary<ResourceType, ResourceForecast>> _onForecastUpdated 
            = new Subject<Dictionary<ResourceType, ResourceForecast>>();
        
        public IObservable<Dictionary<ResourceType, ResourceForecast>> OnForecastUpdated 
            => _onForecastUpdated;

        public ResourcePreviewManager()
        {
            _resolver = new EffectResolver();
            _grid = G.GridSystem;
            _resourceManager = G.ResourceManager;
            _currentForecast = new Dictionary<ResourceType, int>();
            
            // 🔥 Автоматический пересчёт при изменении карты
            SubscribeToGridChanges();
        }

        private void SubscribeToGridChanges()
        {
            // Пересчитывать прогноз при:
            // • Постройке здания
            // • Уничтожении здания  
            // • Изменении стадии здания (если эффекты зависят от стадии)
            
            G.Events.CellChanged
                .Subscribe(_ => RecalculateForecast())
                .AddTo(G.Disposables); // Используйте глобальный CompositeDisposable
                
            G.Events.BuildingStageChanged
                .Subscribe(_ => RecalculateForecast())
                .AddTo(G.Disposables);
        }

        /// <summary>
        /// Пересчитать прогноз ресурсов на конец текущего хода
        /// </summary>// File: Core/Base/ResourcePreviewManager.cs
public void RecalculateForecast()
{
    // 🔥 Защита от неинициализированных зависимостей
    if (_grid == null || _resourceManager == null)
    {
        Debug.LogWarning("[ResourcePreviewManager] Dependencies not ready, skipping forecast");
        return;
    }
    
    _currentForecast.Clear();
    
    var gridData = _grid.GetGridData();
    if (gridData == null) return; // 🔥 Защита
    
    int width = gridData.GetLength(0);
    int height = gridData.GetLength(1);
    
    var currentResources = _resourceManager.GetResources();

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            var cell = gridData[x, y];
            if (cell?.building == null) continue; // 🔥 Null-conditional

            var building = cell.building;
            var effects = building.GetEffectDefinition()?.effects;
            if (effects == null || effects.Count == 0) continue;

            var result = _resolver.ResolveEffects(effects, building, _grid, currentResources);

            foreach (var change in result.ResourceChanges)
            {
                int amount = change.Amount;
                
                // 🔥 Защита от null ProductionBonusManager
                if (G.ProductionBonusManager != null)
                {
                    int globalBonus = G.ProductionBonusManager.GetBonusForBuilding(building, change.Type);
                    amount += globalBonus;
                }
                
                if (!_currentForecast.ContainsKey(change.Type))
                    _currentForecast[change.Type] = 0;
                    
                _currentForecast[change.Type] += amount;
            }
        }
    }
    
    // Формируем прогноз
    var fullForecast = new Dictionary<ResourceType, ResourceForecast>();
    foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
    {
        if (type == ResourceType.None) continue;
        
        int current = _resourceManager.GetAmount(type);
        int delta = _currentForecast.TryGetValue(type, out var d) ? d : 0;
        
        fullForecast[type] = new ResourceForecast(type, current, delta);
    }
    
    _onForecastUpdated.OnNext(fullForecast);
}

        /// <summary>
        /// Получить прогноз для конкретного ресурса
        /// </summary>
        public ResourceForecast GetForecastForResource(ResourceType type)
        {
            int current = _resourceManager.GetAmount(type);
            int delta = _currentForecast.TryGetValue(type, out var d) ? d : 0;
            return new ResourceForecast(type, current, delta);
        }

        /// <summary>
        /// Принудительно обновить прогноз (например, после применения бонусов)
        /// </summary>
        public void ForceRefresh() => RecalculateForecast();

        public void Dispose()
        {
            _onForecastUpdated.Dispose();
        }
    }
}