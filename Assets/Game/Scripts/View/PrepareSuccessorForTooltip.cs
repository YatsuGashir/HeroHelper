using System.Collections.Generic;
using System.Text;
using Core.Effects;
using Data;
using UnityEngine;

public class PrepareSuccessorForTooltip
{
    public TooltipTriger tooltipTriger;
    public SuccessorProfile successorStatus;

    private string _bodyText = "";
    
    public void InitializeTooltip()
    {
        tooltipTriger.Header = successorStatus.successorName ;
        
        _bodyText = $"Время жизни: {successorStatus.timeToDeath} ходов \n " +
                    $"Эффекты {AppendEffectsSection(successorStatus.SuccessorEffect.effects)}";

        tooltipTriger.Content = _bodyText;
    }

    private string AppendEffectsSection(List<GameEffectBase> effects)
    {
        if (effects == null || effects.Count == 0)
        { 
            return "";
        }
        
        string text= "";
        
        foreach (var effect in effects)
        {
            if (effect != null)
            {
                string desc = effect.Description;
                
                text +=$"  • {desc}";
            }
        }
        return text;
    }
}
