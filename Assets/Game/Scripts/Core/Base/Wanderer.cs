using System;
using System.Collections.Generic;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;
using View;
using Random = Unity.Mathematics.Random;

namespace Core.Base
{
    public class Wanderer : IDisposable
    {
        public WandererData Data { get; }
        public WandererView View { get; private set; }
        
        private readonly IDisposable _moveSubscription;
        private readonly CompositeDisposable _disposables = new();
        private bool _isDisposed;

        public Wanderer(WandererData data)
        {
            Data = data;
            
            _moveSubscription = Observable.CombineLatest(
                    Data.X.Skip(1),
                    Data.Y.Skip(1),
                    (x, y) => new { x, y }
                )
                .Subscribe(_ => TriggerViewMove());
        }

        public void AttachView(WandererView view)
        {
            View = view;
            View.Bind(this);
            View.MoveToImmediate(Data.X.Value, Data.Y.Value);
        }

        private void TriggerViewMove()
        {
            if (_isDisposed || View == null) return;
            
            Data.IsMoving.Value = true;
            
            View.MoveTo(Data.X.Value, Data.Y.Value, Data.MoveDuration);
            
            Observable.Timer(TimeSpan.FromSeconds(Data.MoveDuration))
                .Subscribe(_ => 
                {
                    if (!_isDisposed) 
                        Data.IsMoving.Value = false;
                })
                .AddTo(_disposables);
        }
        
        public bool TryRandomMove()
        {
            if (_isDisposed || Data.IsMoving.Value) return false;
            
            var validNeighbors = GetValidNeighbors(Data.X.Value, Data.Y.Value);
            if (validNeighbors.Count == 0) return false;

            var target = validNeighbors[UnityEngine.Random.Range(0, validNeighbors.Count)];
            Data.SetPosition(target.x, target.y);
            return true;
        }
        
        private List<(int x, int y)> GetValidNeighbors(int x, int y)
        {
            var result = new List<(int, int)>();
            var gridSystem = G.GridSystem;
    
            if (gridSystem == null) return result;

            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i], ny = y + dy[i];

                if (nx < 0 || nx >= gridSystem.Width || ny < 0 || ny >= gridSystem.Height)
                    continue;

                var cell = gridSystem.GetCell(nx, ny);
                if (cell.terrainType == TerrainType.Water)
                    continue;

                if (!Data.IsPositionInSpawnRadius(nx, ny))
                    continue;
            
                result.Add((nx, ny));
            }
    
            return result;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            
            _moveSubscription?.Dispose();
            _disposables?.Dispose();
            Data?.Dispose();
            
            if (View != null)
            {
                View.Unbind();
                UnityEngine.Object.Destroy(View.gameObject);
            }
        }
    }
}