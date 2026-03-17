using System;
using Core;
using Core.Base;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Data;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using View;

namespace GlobalSpace
{
    public class Bootstarp : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private GridView gridView;
        [SerializeField] private BuildingViewSystem buildingView;
        [SerializeField] private ResourceView resourceView;
        [SerializeField] private HandView handView;
        [SerializeField] private CurrentSuccessorView currentSuccessorView;
        [SerializeField] private IncidentPanel incidentPanel;

        public Fsm GameFlowFsm { get; private set; }
        

        private async UniTaskVoid Start()
        {

            G.Initialize(gameConfig);
            G.GridView= gridView;

            GameFlowFsm = new Fsm();
            InitializeGameFlowStates();

            buildingView.Init();
            resourceView.Init();
            
            handView.Init(G.HandManager);
            currentSuccessorView.Init(G.successionManager);
            
            incidentPanel.Init();
            
            G.GameManager.StartNewRun(gridView);
            

            await UniTask.Yield();

            Debug.Log("Игра успешно инициализирована.");
        }

        private void InitializeGameFlowStates()
        {
            // Раскомментируйте, когда создадите состояния
            // GameFlowFsm.AddState(new GameplayState(GameFlowFsm, this));
        }

        private void OnDestroy()
        {
            G.ResetRun();
        }

        private void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                G.ResetRun();
                G.GameManager.StartNewRun(gridView);
            }
        }
    }
}
