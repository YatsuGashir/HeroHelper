using System.Collections.Generic;
using Data;
using UniRx;
using View;

namespace Core.Base
{
    public class CustomEventBus
    {
        //событие клеток
        public Subject<CellUpdateEventData> CellChanged { get; } = new Subject<CellUpdateEventData>();
        public Subject<BuildingInstance> BuildingDied { get; } = new Subject<BuildingInstance>();
        public Subject<BuildingInstance> BuildingStageChanged { get; } = new Subject<BuildingInstance>();

        //событие экономики
        public Subject<Dictionary<ResourceType, int>> ResourceChanged { get; } = new Subject<Dictionary<ResourceType, int>>();

        //событие кода
        public Subject<List<BuildingDefinition>> HandUpdated { get; } = new Subject<List<BuildingDefinition>>();
        public Subject<int> TurnStarted { get; } = new Subject<int>(); // int = номер хода
        public Subject<Unit> TurnEnded { get; } = new Subject<Unit>();

        // события игры
        
        public Subject<CellState[,]> GridGenerated { get; } = new Subject<CellState[,]>();
        public Subject<string> DisasterOccurred { get; } = new Subject<string>();
        
        public Subject<Unit> TurnEndRequested { get; } = new Subject<Unit>();

    
        public void Dispose()
        {
            CellChanged.Dispose();
            BuildingDied.Dispose();
            BuildingStageChanged.Dispose();
            ResourceChanged.Dispose();
            HandUpdated.Dispose();
            TurnStarted.Dispose();
            TurnEnded.Dispose();
            GridGenerated.Dispose();
            DisasterOccurred.Dispose();
            TurnEndRequested.Dispose();
        }
    }
}
