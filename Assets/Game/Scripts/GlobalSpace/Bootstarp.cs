using Core;
using Core.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GlobalSpace
{
    public class Bootstarp : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        public Fsm GameFlowFsm { get; private set; }

        async void Start()
        {
            G.GameManager = gameManager;

            GameFlowFsm = new Fsm();
            InitializeGameFlowStates();
            
            //GameFlowFsm.SetState<GameplayState>();
            //PlayerFsm.SetState<IdleState>();

        }
        
            
        private void InitializeGameFlowStates()
        {
            //GameFlowFsm.AddState(new GameplayState(GameFlowFsm, this));
        }
    }
}
