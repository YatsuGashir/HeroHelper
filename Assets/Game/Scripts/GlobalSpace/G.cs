using Core;
using Core.Base;
using Core.Factories;
using Data;
using Core.Base;
using Core.Successors;
using Game.Scripts.Core.incidents;
using UnityEngine;
using View;

namespace GlobalSpace
{
    public static class G
    {

        //Core
        public static GameManager GameManager { get; private set; }
        public static GameConfig Config { get; private set; }
        public static GridSystem GridSystem { get; private set; }
        public static CustomEventBus Events { get; private set; }
        public static BuildingFactory  BuildingFactory { get; private set; }
        public static PlacementManager PlacementManager { get; private set; }
        
        public static ResourceManager ResourceManager { get; private set; }
        
        public static TurnManager TurnManager { get; private set; }
        public static GridView GridView { get;  set; }
        public static BuildingLifecycleManager LifecycleManager { get;  set; }
        public static DeckManager DeckManager { get;  set; }
        public static HandManager HandManager { get;  set; }
        public static SuccessionManager SuccessionManager { get;  set; }
        public static IncidentManager IncidentManager { get;  set; }
        public static TickModifierManager  TickModifierManager { get;  set; }
        
        
        public static void Initialize(GameConfig config)
        {
            Config = config;
            GridSystem = new GridSystem();
            GameManager = new GameManager();
            Events = new CustomEventBus();
            TickModifierManager =  new TickModifierManager();
            BuildingFactory = new BuildingFactory();
            PlacementManager = new PlacementManager();
            ResourceManager = new ResourceManager();
            SuccessionManager = new SuccessionManager();
            TurnManager = new TurnManager();
            LifecycleManager = new BuildingLifecycleManager();
            DeckManager = new DeckManager();
            HandManager = new HandManager();
            IncidentManager = new IncidentManager(Config);
        }
        
        
        public static void ResetRun()
        {

        }
    }
}
