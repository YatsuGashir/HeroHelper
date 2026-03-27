// File: Data/ResourceForecast.cs
using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public struct ResourceForecast
    {
        public ResourceType Type;
        public int Current;      // Текущее значение
        public int Delta;        // Изменение в конце хода (+/-)
        public int Predicted => Current + Delta; // Итог
        
        public ResourceForecast(ResourceType type, int current, int delta)
        {
            Type = type;
            Current = current;
            Delta = delta;
        }
        
        // 🔥 Форматирование для отображения
        public string GetDisplayString()
        {
            if (Delta == 0) return Current.ToString();
            
            string sign = Delta > 0 ? "+" : "";
            string deltaColor = Delta > 0 ? "#4beb68" : "#9b3737"; // зелёный/красный

            return $"<color={deltaColor}>({sign}{Delta})</color> ";
        }
    }
}