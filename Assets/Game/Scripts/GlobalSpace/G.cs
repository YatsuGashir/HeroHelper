using Core;
using Core.Base;
using Data;
using UnityEngine;

namespace GlobalSpace
{
    public static class G 
    {
        
        //Core
        public static GameManager GameManager {get; private set;}
        public static GameConfig Config {get; private set;}
        public static GridSystem  GridSystem {get; private set;}
        
        public static void Initialize(GameConfig config)
        {
            Config = config;
            GridSystem = new GridSystem();
            GameManager = new GameManager();
        }
        
        
        public static void ResetRun()
        {

        }
    }
}
