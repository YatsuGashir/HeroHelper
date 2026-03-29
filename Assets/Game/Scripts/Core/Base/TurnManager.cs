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
        await UniTask.Delay(500);
        G.IncidentManager.OnTurnStart(_currentTurn);
        await UniTask.Delay(500);
        G.SuccessionManager.CanDeath();
        await UniTask.Delay(500);
        await DrawPhaseAsync();

        await UniTask.Delay(500);
        
        GlobalIncidentSelector();
        AudioManager.Instance.PlaySFX("Voice1");
        G.Events.TurnEnded.OnNext(Unit.Default);
        
    }

    private void GlobalIncidentSelector()
    {
        if (_currentTurn == 2)
        {
            G.IncidentManager.StartLongTermEvent(2);
        }
        if (_currentTurn == 10)
        {
            G.IncidentManager.StartLongTermEvent(1);
        }
        if (_currentTurn == 15)
        {
            G.IncidentManager.StartLongTermEvent(2);
        }
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
