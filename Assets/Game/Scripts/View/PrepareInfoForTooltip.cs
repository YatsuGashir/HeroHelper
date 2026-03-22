using System;
using UnityEngine;
using View;

public class PrepareInfoForTooltip : MonoBehaviour
{
    [SerializeField] private TooltipTriger tooltipTriger;
    [SerializeField] private BuildingStatusView buildingStatusView;

    private string _bodyText = "";

    private void Awake()
    {
        // 🔥 Проверка 1: назначены ли поля в Инспекторе?
        if (tooltipTriger == null)
        {
            Debug.LogError($"[PrepareInfoForTooltip] tooltipTriger is null on {gameObject.name}!");
            return;
        }
        
        if (buildingStatusView == null)
        {
            Debug.LogError($"[PrepareInfoForTooltip] buildingStatusView is null on {gameObject.name}!");
            return;
        }

        // 🔥 Проверка 2: инициализирован ли MyBuilding?
        if (buildingStatusView.MyBuilding == null)
        {
            Debug.LogWarning($"[PrepareInfoForTooltip] MyBuilding is null on {gameObject.name}. Retrying in Start()...");
            // Откладываем инициализацию на Start(), если MyBuilding ещё не готов
            return;
        }

        // 🔥 Проверка 3: есть ли Definition?
        if (buildingStatusView.MyBuilding.Definition == null)
        {
            Debug.LogError($"[PrepareInfoForTooltip] Definition is null for building {buildingStatusView.MyBuilding}!");
            return;
        }

        InitializeTooltip();
    }

    // 🔥 Попытка инициализации во Start(), если в Awake() не вышло
    private void Start()
    {
        if (tooltipTriger == null || buildingStatusView == null) return;
        
        if (buildingStatusView.MyBuilding != null && 
            buildingStatusView.MyBuilding.Definition != null &&
            string.IsNullOrEmpty(tooltipTriger.Header))
        {
            InitializeTooltip();
        }
    }

    private void InitializeTooltip()
    {
        tooltipTriger.Header = buildingStatusView.MyBuilding.Definition.buildingName;
        
        _bodyText = buildingStatusView.MyBuilding.Definition.buildingTags.ToString() + "\n"
            + buildingStatusView.MyBuilding.stage.ToString();

        tooltipTriger.Content = _bodyText;
    }
}