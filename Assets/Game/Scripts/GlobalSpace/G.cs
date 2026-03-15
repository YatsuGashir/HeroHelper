using Core;
using Core.Base;
using Core.Factories;
using Data;
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
        
        public static GridView GridView { get;  set; }
        
        public static void Initialize(GameConfig config)
        {
            Config = config;
            GridSystem = new GridSystem();
            GameManager = new GameManager();
            Events = new CustomEventBus();
            BuildingFactory = new BuildingFactory();
            PlacementManager = new PlacementManager();
        }
        
        
        public static void ResetRun()
        {

        }
    }
}
