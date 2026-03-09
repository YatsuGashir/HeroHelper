
namespace Core.StateMachine
{
    public abstract class FsmState
    {
        public readonly Fsm Fsm;

        public FsmState(Fsm fsm)
        {
            Fsm = fsm;
        }

        public virtual void Enter() {}
        public virtual void Exit() {}
        public virtual void Update() {}
        
    }
}
