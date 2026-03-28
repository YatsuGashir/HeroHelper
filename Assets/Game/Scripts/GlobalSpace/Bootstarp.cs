using System;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private IncidentInfoPanel incidentInfoPanel;
        [SerializeField] private SuccessorSelectionUI successorSelectionUI;
        [SerializeField] private FeatureCandidatesView featureCandidatesView;
        [SerializeField] private EndTurnButton endTurnButton;
        [SerializeField] private AudioSettingsUI audioSettingsUI;

        public Fsm GameFlowFsm { get; private set; }
        

        
        private async UniTaskVoid Start()
        {
            _isRestarting = false;
            RadioManager.Instance.StartRadio();
            
            
            G.Initialize(gameConfig);
            await UniTask.Yield();
            
            G.GridView= gridView;

            GameFlowFsm = new Fsm();
            InitializeGameFlowStates();

            buildingView.Init();
            resourceView.Init();
            
            handView.Init(G.HandManager);
            currentSuccessorView.Init(G.SuccessionManager);
            successorSelectionUI.Init();
            featureCandidatesView.Init();
            
            incidentInfoPanel.Init();
            incidentPanel.Init();
            
            endTurnButton.Init();
            
            await UniTask.Yield(); 
            G.GameManager.Initialize(gridView);
            

            await UniTask.Yield();
            AudioManager.Instance.PlaySFX("ambiend", 1.6f);
            audioSettingsUI.Init(AudioManager.Instance);
            
            Debug.Log("Игра успешно инициализирована.");
        }

        private void InitializeGameFlowStates()
        {
            // Раскомментируйте, когда создадите состояния
            // GameFlowFsm.AddState(new GameplayState(GameFlowFsm, this));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }
        }
        private static bool _isRestarting = false;

        public void RestartScene()
        {
            if (_isRestarting) return;

            _isRestarting = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnDestroy()
        {
            G.ResetRun();
        }


    }
}
