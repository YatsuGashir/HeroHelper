using System;
using UniRx;

namespace Data
{
    public class WandererData
    {
        public string Id { get; }
        public ReactiveProperty<int> X { get; }
        public ReactiveProperty<int> Y { get; }
        public ReactiveProperty<bool> IsMoving { get; }
        
        public int SpawnX { get; }
        public int SpawnY { get; }
        
        public float MoveDelay { get; set; } = 2f;
        public float MoveDuration { get; set; } = 0.3f;
        
        public int MaxSpawnRadius { get; set; } = 3;

        public WandererData(string id, int startX, int startY)
        {
            Id = id;
            SpawnX = startX;
            SpawnY = startY;
            
            X = new ReactiveProperty<int>(startX);
            Y = new ReactiveProperty<int>(startY);
            IsMoving = new ReactiveProperty<bool>(false);
        }

        public bool IsPositionInSpawnRadius(int x, int y)
        {
            int manhattan = Math.Abs(x - SpawnX) + Math.Abs(y - SpawnY);
            return manhattan <= MaxSpawnRadius;
        }
        public void SetPosition(int x, int y)
        {
            X.Value = x;
            Y.Value = y;
        }

        public void Dispose()
        {
            X.Dispose();
            Y.Dispose();
            IsMoving.Dispose();
        }
    }
}