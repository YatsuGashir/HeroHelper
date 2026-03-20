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
        
        public float MoveDelay { get; set; } = 2f;
        public float MoveDuration { get; set; } = 0.3f;

        public WandererData(string id, int startX, int startY)
        {
            Id = id;
            X = new ReactiveProperty<int>(startX);
            Y = new ReactiveProperty<int>(startY);
            IsMoving = new ReactiveProperty<bool>(false);
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