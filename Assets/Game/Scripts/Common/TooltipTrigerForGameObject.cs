using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigerForGameObject : TooltipTriger
{
    public void  OnMouseEnter()
    {
        Enter();
    }

    public void OnMouseExit()
    {
        Exit();
    }
}
