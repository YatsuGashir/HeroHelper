using System.Collections.Generic;
using System.Text;
using UnityEngine;
using View;
using Data;
using Core.Effects;

public class PrepareCardForTooltip : MonoBehaviour
{
    [SerializeField] private TooltipTriger tooltipTriger;
    [SerializeField] private CardView cardView;

    private void Awake()
    {
        ValidateReferences();
        if (IsValid()) InitializeTooltip();
    }
    
    private void Start()
    {
        if (IsValid() && tooltipTriger.Content == string.Empty)
            InitializeTooltip();
    }

    private void ValidateReferences()
    {
        if (tooltipTriger == null)
            Debug.LogError($"[{nameof(PrepareCardForTooltip)}] tooltipTriger is null on {gameObject.name}!");
        
        if (cardView == null)
            Debug.LogError($"[{nameof(PrepareCardForTooltip)}] cardView is null on {gameObject.name}!");
    }

    private bool IsValid()
    {
        if (tooltipTriger == null || cardView == null) return false;
        
        if (cardView.CardDefinition == null)
        {
            Debug.LogWarning($"[{nameof(PrepareCardForTooltip)}] CardDefinition is null on {gameObject.name}");
            return false;
        }
        
        return true;
    }

    private void InitializeTooltip()
    {
        var definition = cardView.CardDefinition;
        
        tooltipTriger.Header = definition.buildingName;

        var sb = new StringBuilder();

        sb.AppendLine($"Время жизни: <color=#888>{definition.lifeCycle}с</color>");
        sb.AppendLine();
        
        AppendEffectsSection(sb, "Стадия 2:", definition.stage2Effect?.effects);
        
        AppendEffectsSection(sb, "Стадия 3:", definition.stage3Effect?.effects);
        
        sb.AppendLine();

        if (definition.allowedTerrainTypes != null)
            sb.AppendLine($"Можно ставить на: <color=#888>{definition.allowedTerrainTypes}</color>");

        if (definition.buildingTags != BuildingTag.None)
            sb.AppendLine($"Теги: <color=#888>{definition.buildingTags}</color>");

        tooltipTriger.Content = sb.ToString();
    }

    private void AppendEffectsSection(StringBuilder sb, string header, List<GameEffectBase> effects)
    {
        if (effects == null || effects.Count == 0)
        {
            sb.AppendLine($"{header} <color=#666>— нет эффектов —</color>");
            return;
        }
        
        sb.AppendLine(header);
        
        foreach (var effect in effects)
        {
            if (effect != null)
            {
                string desc = effect.Description;
                
                sb.AppendLine($"  • {desc}");
            }
        }
        sb.AppendLine();
    }
}