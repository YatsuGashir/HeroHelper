using Core.incidents;

namespace Core.incidents
{
    public class ActiveIncident
    {
        public IncidentData Data;
        public int TurnsRemaining;

        public ActiveIncident(IncidentData data, int turns)
        {
            Data = data;
            TurnsRemaining = turns;
        }
    }
}
