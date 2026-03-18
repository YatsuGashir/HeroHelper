using Data;

namespace Core.Successors
{
    public class SuccessorState
    {
        public SuccessorProfile CurrentProfile;
        public int GenerationNumber; 
        public int CurrentLifeTime;
        public bool IsActive;

        public void Initialize(SuccessorProfile profile, int generation)
        {
            CurrentProfile = profile;
            GenerationNumber = generation;
            IsActive = true;
            CurrentLifeTime = profile.timeToDeath;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
