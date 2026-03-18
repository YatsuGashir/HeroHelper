using System.Collections.Generic;
using Data;

namespace Core.Successors
{
    [System.Serializable]
    public class SuccessionSelector
    {
        public List<SuccessorProfile> Candidates { get; private set; } = new List<SuccessorProfile>(3);
        public SuccessorProfile Chosen { get; private set; }
        public bool IsResolved => Chosen != null;

        public void SetCandidates(List<SuccessorProfile> candidates)
        {
            Candidates.Clear();
            Candidates.AddRange(candidates);
            Chosen = null;
        }

        public void Choose(SuccessorProfile profile)
        {
            if (!Candidates.Contains(profile))
            {
                return;
            }
            Chosen = profile;
        }

        public void Clear()
        {
            Candidates.Clear();
            Chosen = null;
        }
    }
}
