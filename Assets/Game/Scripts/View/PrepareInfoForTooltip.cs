using System.Collections.Generic;
using System.Text;
using UnityEngine;
using View;
using Data;
using Core.Effects;

public class PrepareInfoForTooltip : MonoBehaviour
{
    [SerializeField] private TooltipTriger tooltipTriger;
    [SerializeField] private BuildingStatusView buildingStatusView;

    private void Awake()
    {
        ValidateReferences();
        if (IsValid()) InitializeTooltip();
    }
    
    private void Start()
    {
        // Если валидно, но контент еще не заполнен (например, инициализация произошла позже)
        if (IsValid() && string.IsNullOrEmpty(tooltipTriger.Content))
            InitializeTooltip();
    }

    private void ValidateReferences()
    {
        if (tooltipTriger == null)
            Debug.LogError($"[{nameof(PrepareInfoForTooltip)}] tooltipTriger is null on {gameObject.name}!");
        
        if (buildingStatusView == null)
            Debug.LogError($"[{nameof(PrepareInfoForTooltip)}] buildingStatusView is null on {gameObject.name}!");
    }

    private bool IsValid()
    {
        if (tooltipTriger == null || buildingStatusView == null) return false;
        
        // Проверка цепочки данных для здания
        if (buildingStatusView.MyBuilding == null)
        {
            Debug.LogWarning($"[{nameof(PrepareInfoForTooltip)}] MyBuilding is null on {gameObject.name}");
            return false;
        }

        if (buildingStatusView.MyBuilding.Definition == null)
        {
            Debug.LogWarning($"[{nameof(PrepareInfoForTooltip)}] Building Definition is null on {gameObject.name}");
            return false;
        }
        
        return true;
    }

    private void InitializeTooltip()
    {
        // Получаем определение здания
        var definition = buildingStatusView.MyBuilding.Definition;
        
        tooltipTriger.Header = definition.buildingName;

        var sb = new StringBuilder();

        // Примечание: Если у здания нет поля lifeCycle, замените на актуальное (например, Health или Level)
        // Если поле называется иначе в BuildingDefinition, поправьте доступ к нему.
        if (definition.GetType().GetProperty("lifeCycle") != null) 
        {
             sb.AppendLine($"Время жизни: <color=#888>{definition.lifeCycle}с</color>");
        }
        else
        {
             // Пример альтернативы, если lifeCycle нет, но есть Stage (из старого скрипта)
             sb.AppendLine($"Уровень/Стадия: <color=#888>{buildingStatusView.MyBuilding.stage}</color>");
        }
        
        sb.AppendLine();
        
        // Попытка получить эффекты (адаптируйте названия полей под вашу BuildingDefinition)
        // В скрипте карты это stage2Effect и stage3Effect. 
        // Если у здания структура иная, измените доступ к свойствам ниже.
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