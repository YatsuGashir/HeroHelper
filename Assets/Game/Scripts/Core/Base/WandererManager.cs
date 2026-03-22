using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using GlobalSpace;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using View;

namespace Core.Base
{
    public class WandererManager : IDisposable
    {
        public IReadOnlyList<Wanderer> Wanderers => _wanderers;
        
        private readonly List<Wanderer> _wanderers = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly System.Random _random = new();
        
        private IDisposable _updateLoop;
        private bool _isActive;

        public void Initialize(int initialCount)
        {
            if (G.GridSystem == null || G.GridView == null)
            {
                Debug.LogError("WandererManager: Grid not initialized!");
                return;
            }

            // Спавним начальных путников на валидных клетках
            /*for (int i = 0; i < initialCount; i++)
            {
                SpawnRandomWanderer();
            }*/
            
            StartUpdateLoop();
            
            G.Events.TurnEnded.Subscribe(_ => OnTurnEnded()).AddTo(_disposables);
        }

        private void StartUpdateLoop()
        {
            _isActive = true;
            
            _updateLoop = Observable.Interval(TimeSpan.FromSeconds(1.5f))
                .Where(_ => _isActive)
                .Subscribe(_ => UpdateWanderers())
                .AddTo(_disposables);
        }

        private void UpdateWanderers()
        {
            foreach (var wanderer in _wanderers.ToList())
            {
                if (_random.NextDouble() < 0.6f)
                {
                    wanderer.TryRandomMove();
                }
            }
        }

        private void OnTurnEnded()
        {
            foreach (var wanderer in _wanderers)
            {
                wanderer.TryRandomMove();
            }
        }

        public void SpawnWandererAt(int x, int y)
        {
            
            string shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            var id = $"Wanderer_{shortGuid}";

    
            var data = new WandererData(id, x, y)
            {
                MoveDelay = UnityEngine.Random.Range(0.2f, 0.5f),
                MoveDuration = UnityEngine.Random.Range(3f, 5f)
            };
    
            var wanderer = new Wanderer(data);

            var view = UnityEngine.Object.Instantiate(
                G.Config.wandererPrefab,
                G.GridView.transform
            ).GetComponent<WandererView>();
    
            view.transform.position = new Vector3(x, y, 1f);
    
            wanderer.AttachView(view);
            _wanderers.Add(wanderer);
        }

       /* public void SpawnRandomWanderer()
        {
            var (x, y) = FindRandomValidPosition();
    
            // 🔧 Вариант 1: короткая строка из Guid (первые 8 символов)
            string shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);
            var id = $"Wanderer_{shortGuid}";
    
            // 🔧 Вариант 2 (альтернатива): простой счётчик + рандом
            // static int _spawnCounter = 0;
            // var id = $"Wanderer_{++_spawnCounter}_{Random.Range(0, 9999):D4}";
    
            var data = new WandererData(id, x, y)
            {
                MoveDelay = UnityEngine.Random.Range(1.5f, 3f),
                MoveDuration = UnityEngine.Random.Range(0.25f, 0.4f)
            };
    
            var wanderer = new Wanderer(data);
    
            // Создаём View
            var view = UnityEngine.Object.Instantiate(
                G.Config.wandererPrefab,
                G.GridView.transform
            ).GetComponent<WandererView>();
    
            view.transform.position = new Vector3(x, y, 1f);
    
            wanderer.AttachView(view);
            _wanderers.Add(wanderer);
        }*/

        private (int x, int y) FindRandomValidPosition()
        {
            var grid = G.GridSystem.GetGridData();
            var attempts = 0;
            
            while (attempts < 100)
            {
                int x = _random.Next(0, G.GridSystem.Width);
                int y = _random.Next(0, G.GridSystem.Height);
                
                if (grid[x, y].terrainType != TerrainType.Water)
                    return (x, y);
                    
                attempts++;
            }
            
            // Фоллбэк: поиск первой валидной клетки
            for (int x = 0; x < G.GridSystem.Width; x++)
                for (int y = 0; y < G.GridSystem.Height; y++)
                    if (grid[x, y].terrainType != TerrainType.Water)
                        return (x, y);
            
            return (0, 0);
        }

        public void RemoveWanderer(Wanderer wanderer)
        {
            wanderer?.Dispose();
            _wanderers.Remove(wanderer);
        }

        public void ClearAll()
        {
            foreach (var w in _wanderers.ToList())
                RemoveWanderer(w);
        }

        public void Dispose()
        {
            _isActive = false;
            ClearAll();
            _disposables?.Dispose();
            _updateLoop?.Dispose();
        }
    }
}