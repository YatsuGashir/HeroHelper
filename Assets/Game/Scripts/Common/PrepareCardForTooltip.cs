using UnityEngine;
using View;

public class PrepareCardForTooltip : MonoBehaviour
{
 [SerializeField] private TooltipTriger tooltipTriger;
    [SerializeField] private CardView cardVIew;

    private string _bodyText = "";

    private void Awake()
    {
        if (tooltipTriger == null)
        {
            Debug.LogError($"[PrepareInfoForTooltip] tooltipTriger is null on {gameObject.name}!");
            return;
        }
        
        if (cardVIew == null)
        {
            Debug.LogError($"[PrepareInfoForTooltip] buildingStatusView is null on {gameObject.name}!");
            return;
        }

        if (cardVIew.CardDefinition == null)
        {
            Debug.LogWarning($"[PrepareInfoForTooltip] MyBuilding is null on {gameObject.name}. Retrying in Start()...");
            return;
        }
        

        InitializeTooltip();
    }
    
    private void Start()
    {
        InitializeTooltip();
    }

    private void InitializeTooltip()
    {
        tooltipTriger.Header = cardVIew.CardDefinition.buildingName;

        _bodyText = "Время жизни: "+cardVIew.CardDefinition.lifeCycle.ToString() + '\n' +
                    "Эффект 1 стадии: "+cardVIew.CardDefinition.stage2Effect.ToString() + '\n' +
                    "Эффект 2 стадии: "+cardVIew.CardDefinition.stage3Effect.ToString() + '\n' +
                    "Ставиться на "+cardVIew.CardDefinition.allowedTerrainTypes.ToString() + '\n' +
                    "Теги постройки: "+cardVIew.CardDefinition.buildingTags.ToString() + '\n';

        tooltipTriger.Content = _bodyText;
    }
}
