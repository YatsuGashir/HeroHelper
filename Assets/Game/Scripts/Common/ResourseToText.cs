using Data;
using UnityEngine;

public static class ResourseToText
{
    public static string ConvertToText(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food:
                return "еды";
            case ResourceType.Wood:
                return "дерева";
            case ResourceType.Stone:
                return "камня";
            case ResourceType.Spore:
                return "споры";
            case ResourceType.Water:
                return "воды";
            default:
                return type.ToString();
        }
    }
}
