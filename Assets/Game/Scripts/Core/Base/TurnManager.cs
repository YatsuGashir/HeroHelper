using Core.Base;
using Cysharp.Threading.Tasks;
using Core.Base;
using GlobalSpace;
using UniRx;
using UnityEngine;

public class TurnManager
{
    private CompositeDisposable _disposables;

    private BuildingLifecycleManager _lifecycleManager;
    
    public TurnManager()
    {
        _disposables = new CompositeDisposable();
        SubscribeToEvents();
        _lifecycleManager = new BuildingLifecycleManager();
    }

    private void SubscribeToEvents()
    {
        G.Events.TurnEndRequested
            .Subscribe(_ => EndTurnAsync().Forget())
            .AddTo(_disposables);
    }

    public async UniTask EndTurnAsync()
    {
        Debug.Log("Фаза расчетов...");
        _lifecycleManager.ProcessEndOfTurn(); 
        
        await UniTask.Delay(500);

    }
    
    public void Dispose() => _disposables.Dispose();
}
