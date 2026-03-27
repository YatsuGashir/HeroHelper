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
        InitializeTooltip();
    }
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