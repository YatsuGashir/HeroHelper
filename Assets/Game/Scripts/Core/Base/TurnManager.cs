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
    
    private int _currentTurn;
    
    public TurnManager()
    {
        _disposables = new CompositeDisposable();
        SubscribeToEvents();
        _lifecycleManager = new BuildingLifecycleManager();
        _currentTurn = 0;
    }

    private void SubscribeToEvents()
    {
        G.Events.TurnEndRequested
            .Subscribe(_ => EndTurnAsync().Forget())
            .AddTo(_disposables);
    }

    public async UniTask EndTurnAsync()
    {
        Debug.Log($"=== ЗАВЕРШЕНИЕ ХОДА {_currentTurn} ===");

        _lifecycleManager.ProcessEndOfTurn();

        _currentTurn++;

        G.IncidentManager.OnTurnStart(_currentTurn);
        G.SuccessionManager.CanDeath();

        await DrawPhaseAsync();

        await UniTask.Delay(500);
        
    }
    
    private async UniTask DrawPhaseAsync()
    {
        int maxHandSize = G.HandManager.MaxHandSize;
        int currentHandSize = G.HandManager.Count;
        
        if (currentHandSize >= maxHandSize)
        {
            return;
        }

        int cardsToDraw = maxHandSize - currentHandSize;
        
        var newCards = G.DeckManager.DrawCardsWithReshuffle(cardsToDraw);

        if (newCards.Count > 0)
        {
            G.HandManager.AddCards(newCards);
        }
        
        await UniTask.Delay(1000); 
    }
    
    public void Dispose() => _disposables.Dispose();
}
