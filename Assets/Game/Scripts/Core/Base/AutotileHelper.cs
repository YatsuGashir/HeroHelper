using UnityEngine;

namespace Data
{
    public static class AutotileHelper
    {
        // Битовая маска: верх=1, право=2, низ=4, лево=8
        public static int CalculateAutotileIndex(bool top, bool right, bool bottom, bool left)
        {
            int index = 0;
            if (top) index |= 1;
            if (right) index |= 2;
            if (bottom) index |= 4;
            if (left) index |= 8;
            return index; // 0–15
        }

        // 🔥 Расширенная версия для 20 спрайтов (с углами)
        public static int CalculateAutotileIndexWithCorners(
            bool top, bool right, bool bottom, bool left,
            bool topLeft, bool topRight, bool bottomRight, bool bottomLeft)
        {
            // Сначала базовый индекс 0–15
            int index = CalculateAutotileIndex(top, right, bottom, left);

            // 🔍 Проверка на внутренние углы (нужен специальный спрайт 16–19)
            // Угол нужен, когда два ортогональных соседа есть, а диагональ между ними — нет
            
            // ↖ Внутренний угол: есть верх+лево, но нет topLeft
            if (top && left && !topLeft) return 16;
            // ↗ Внутренний угол: есть верх+право, но нет topRight  
            if (top && right && !topRight) return 17;
            // ↘ Внутренний угол: есть низ+право, но нет bottomRight
            if (bottom && right && !bottomRight) return 18;
            // ↙ Внутренний угол: есть низ+лево, но нет bottomLeft
            if (bottom && left && !bottomLeft) return 19;

            // Если угол не нужен — возвращаем базовый индекс
            return index;
        }

        // Хелпер: является ли тип "прерывающим" для автотайла луга
        public static bool IsBlockingTerrain(TerrainType type)
        {
            return type == TerrainType.Water || type == TerrainType.None;
        }
    }
}