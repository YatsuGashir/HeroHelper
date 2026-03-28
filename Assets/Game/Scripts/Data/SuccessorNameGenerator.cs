using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public struct MushroomNamePair
    {
        public string maleName;
        public string femaleName;
        public string maleNickname;
        public string femaleNickname;
    }

    public static class SuccessorNameGenerator
    {
private static readonly List<MushroomNamePair> _names = new List<MushroomNamePair>
{
    new MushroomNamePair { maleName = "Абеликс", femaleName = "Антродия", maleNickname = "Азартный", femaleNickname = "Азартная" },
    new MushroomNamePair { maleName = "Абортипорус", femaleName = "Вёшенка", maleNickname = "Аппетитный", femaleNickname = "Аппетитная" },
    new MushroomNamePair { maleName = "Болетелл", femaleName = "Виннея", maleNickname = "Артистичный", femaleNickname = "Артистичная" },
    new MushroomNamePair { maleName = "Боровик", femaleName = "Вкуснопашка", maleNickname = "Бедный", femaleNickname = "Бедная" },
    new MushroomNamePair { maleName = "Бубовик", femaleName = "Воздуханка", maleNickname = "Без шляпки", femaleNickname = "Без шляпки" },
    new MushroomNamePair { maleName = "Валуй", femaleName = "Волокница", maleNickname = "Безбожный", femaleNickname = "Безбожная" },
    new MushroomNamePair { maleName = "Водохлёб", femaleName = "Галерина", maleNickname = "Безграничный", femaleNickname = "Безграничная" },
    new MushroomNamePair { maleName = "Всеслик", femaleName = "Горькушка", maleNickname = "Безмамный", femaleNickname = "Безмамная" },
    new MushroomNamePair { maleName = "Головач", femaleName = "Грифола", maleNickname = "Белый", femaleNickname = "Белая" },
    new MushroomNamePair { maleName = "Груздь", femaleName = "Дениска", maleNickname = "Бесконтактный", femaleNickname = "Бесконтактная" },
    new MushroomNamePair { maleName = "Дубовик", femaleName = "Домоседка", maleNickname = "Бесстыжий", femaleNickname = "Бесстыжая" },
    new MushroomNamePair { maleName = "Ежовик", femaleName = "Зеленушка", maleNickname = "Благоразумный", femaleNickname = "Благоразумная" },
    new MushroomNamePair { maleName = "Землежуй", femaleName = "Калаплака", maleNickname = "Бледный", femaleNickname = "Бледная" },
    new MushroomNamePair { maleName = "Ивишень", femaleName = "Кривоножка", maleNickname = "Богатый", femaleNickname = "Богатая" },
    new MushroomNamePair { maleName = "Короед", femaleName = "Лёшка", maleNickname = "Бодрый", femaleNickname = "Бодрая" },
    new MushroomNamePair { maleName = "Кубовик", femaleName = "Лисичка", maleNickname = "Болтливый", femaleNickname = "Болтливая" },
    new MushroomNamePair { maleName = "Лопастик", femaleName = "Многошляпка", maleNickname = "Буйный", femaleNickname = "Буйная" },
    new MushroomNamePair { maleName = "Маслёнок", femaleName = "Обабок", maleNickname = "Бурый", femaleNickname = "Бурая" },
    new MushroomNamePair { maleName = "Междузёмник", femaleName = "Пальценожка", maleNickname = "Великий", femaleNickname = "Великая" },
    new MushroomNamePair { maleName = "Мицегляд", femaleName = "Простоглядка", maleNickname = "Весёлый", femaleNickname = "Весёлая" },
    new MushroomNamePair { maleName = "Моховичок", femaleName = "Самоглядка", maleNickname = "Весомый", femaleNickname = "Весомая" },
    new MushroomNamePair { maleName = "Мухомор", femaleName = "Свинушка", maleNickname = "Вонючий", femaleNickname = "Вонючая" },
    new MushroomNamePair { maleName = "Надберёзовик", femaleName = "Серянка", maleNickname = "Восклицающий", femaleNickname = "Восклицающая" },
    new MushroomNamePair { maleName = "Надлиственник", femaleName = "Сыроежка", maleNickname = "Всесильный", femaleNickname = "Всесильная" },
    new MushroomNamePair { maleName = "Надосиновик", femaleName = "Толстолшляпка", maleNickname = "Высокий", femaleNickname = "Высокая" },
    new MushroomNamePair { maleName = "Опёнок", femaleName = "Чисмяк", maleNickname = "Габаритный", femaleNickname = "Габаритная" },
    new MushroomNamePair { maleName = "Остерикс", femaleName = "Шиитака", maleNickname = "Гигантский", femaleNickname = "Гигантская" },
    new MushroomNamePair { maleName = "Пазовик", femaleName = "Шимеджа", maleNickname = "Глубоководный", femaleNickname = "Глубоководная" },
    new MushroomNamePair { maleName = "Паутинник", femaleName = "", maleNickname = "Гребнетчатый", femaleNickname = "Гребнетчатая" },
    new MushroomNamePair { maleName = "Подберёзовик", femaleName = "", maleNickname = "Грубый", femaleNickname = "Грубая" },
    new MushroomNamePair { maleName = "Подбетонник", femaleName = "", maleNickname = "Длинноногий", femaleNickname = "Длинноногая" },
    new MushroomNamePair { maleName = "Подгруздок", femaleName = "", maleNickname = "Домашний", femaleNickname = "Домашняя" },
    new MushroomNamePair { maleName = "Подосиновик", femaleName = "", maleNickname = "Досадный", femaleNickname = "Досадная" },
    new MushroomNamePair { maleName = "Портобелло", femaleName = "", maleNickname = "Жёлтый", femaleNickname = "Жёлтая" },
    new MushroomNamePair { maleName = "Притоптальник", femaleName = "", maleNickname = "Жестокий", femaleNickname = "Жестокая" },
    new MushroomNamePair { maleName = "Протуберанчик", femaleName = "", maleNickname = "Жизнерадостный", femaleNickname = "Жизнерадостная" },
    new MushroomNamePair { maleName = "Псилоцибе", femaleName = "", maleNickname = "Жужжжавный", femaleNickname = "Жужжжавная" },
    new MushroomNamePair { maleName = "Пухлик", femaleName = "", maleNickname = "Заурядный", femaleNickname = "Заурядная" },
    new MushroomNamePair { maleName = "Рыжик", femaleName = "", maleNickname = "Изменчивый", femaleNickname = "Изменчивая" },
    new MushroomNamePair { maleName = "Синяк", femaleName = "", maleNickname = "Истинный", femaleNickname = "Истинная" },
    new MushroomNamePair { maleName = "Смаковик", femaleName = "", maleNickname = "Костноязычный", femaleNickname = "Костноязычная" },
    new MushroomNamePair { maleName = "Смертоед", femaleName = "", maleNickname = "Красноречивый", femaleNickname = "Красноречивая" },
    new MushroomNamePair { maleName = "Смешнявик", femaleName = "", maleNickname = "Красный", femaleNickname = "Красная" },
    new MushroomNamePair { maleName = "Сморчок", femaleName = "", maleNickname = "Крепкий", femaleNickname = "Крепкая" },
    new MushroomNamePair { maleName = "Споровиктор", femaleName = "", maleNickname = "Крохотный", femaleNickname = "Крохотная" },
    new MushroomNamePair { maleName = "Темнорослик", femaleName = "", maleNickname = "Лёгкий", femaleNickname = "Лёгкая" },
    new MushroomNamePair { maleName = "Толстоногий", femaleName = "", maleNickname = "Ложный", femaleNickname = "Ложная" },
    new MushroomNamePair { maleName = "Трутовик", femaleName = "", maleNickname = "Маленький", femaleNickname = "Маленькая" },
    new MushroomNamePair { maleName = "Трюфель", femaleName = "", maleNickname = "Малоспорный", femaleNickname = "Малоспорная" },
    new MushroomNamePair { maleName = "Чисночок", femaleName = "", maleNickname = "Маслянистый", femaleNickname = "Маслянистая" },
    new MushroomNamePair { maleName = "Шампиньон", femaleName = "", maleNickname = "Массивный", femaleNickname = "Массивная" },
    new MushroomNamePair { maleName = "Огузок", femaleName = "", maleNickname = "Милосердный", femaleNickname = "Милосердная" },
};

private static readonly List<string> _maleOnlyNicknames = new List<string>
{
    "Могучий", "Мокрый", "Мудрый", "Мягкий", "Мятежный", "Надёжный", "Надменный",
    "Наисмешнявейший", "Невоспитанный", "Недалёкий", "Неистовый", "Немой",
    "Необыкновенный", "Низкий", "Обученный", "Обыкновенный", "Однобокий",
    "Окрепший", "Омерзительный", "Осмотрительный", "Осуждаемый", "Отмерший",
    "Пантерный", "Пересредний", "Подглядывающий", "Подменный", "Подпольный",
    "Польский", "Прелестный", "Просохший", "Псевдоголубой", "Раздражительный",
    "Размеренный", "Размякший", "Разноцветный", "Рыжий", "С большой шляпкой",
    "С маленькой шляпкой", "Светлый", "Свободный", "Сиреневый", "Склизкий",
    "Слепой", "Смачный", "Смелый", "Смердящий", "Смешнявый", "Сознательный",
    "Спорный", "Стабильный", "Степной", "Тактильный", "Тёмный", "Тонкий",
    "Тополёвый", "Тоскливый", "Трогательный", "Трусливый", "Удивительный",
    "Узкий", "Упругий", "Уродливый", "Фиолетовый", "Шаровидный", "Широкий", "Щедрый"
};

private static readonly List<string> _femaleOnlyNicknames = new List<string>
{
    "Могучая", "Мокрая", "Мудрая", "Мягкая", "Мятежная", "Надёжная", "Надменная",
    "Наисмешнявейшая", "Невоспитанная", "Недалёкая", "Неистовая", "Немая",
    "Необыкновенная", "Низкая", "Обученная", "Обыкновенная", "Однобокая",
    "Окрепшая", "Омерзительная", "Осмотрительная", "Осуждаемая", "Отмершая",
    "Пантерная", "Пересредняя", "Подглядывающая", "Подменная", "Подпольная",
    "Польская", "Прелестная", "Просохшая", "Псевдоголубая", "Раздражительная",
    "Размеренная", "Размякшая", "Разноцветная", "Рыжая", "С большой шляпкая",
    "С маленькой шляпкая", "Светлая", "Свободная", "Сиреневая", "Склизкая",
    "Слепая", "Смачная", "Смелая", "Смердящая", "Смешнявая", "Сознательная",
    "Спорная", "Стабильная", "Степная", "Тактильная", "Тёмная", "Тонкая",
    "Тополёвая", "Тоскливая", "Трогательная", "Трусливая", "Удивительная",
    "Узкая", "Упругая", "Уродливая", "Фиолетовая", "Шаровидная", "Широкая", "Щедрая"
};
        
public static string GenerateName(int seed)
{
    System.Random random = new System.Random(seed);
    

        int nameIndex = random.Next(_names.Count);
        MushroomNamePair pair = _names[nameIndex];
        
        bool isMale = random.Next(2) == 0;
        string baseName = isMale ? pair.maleName : pair.femaleName;
        
        string nickname = isMale ? pair.maleNickname : pair.femaleNickname;
        return $"{baseName} \"{nickname}\"";

    
}


        public static string GenerateBaseName(int seed)
        {
            System.Random random = new System.Random(seed);
            int nameIndex = random.Next(_names.Count);
            MushroomNamePair pair = _names[nameIndex];
            
            bool isMale = random.Next(2) == 0;
            return isMale ? pair.maleName : pair.femaleName;
        }

        public static string GenerateNickname(int seed)
        {
            System.Random random = new System.Random(seed);
            int nameIndex = random.Next(_names.Count);
            MushroomNamePair pair = _names[nameIndex];
            
            bool isMale = random.Next(2) == 0;
            return isMale ? pair.maleNickname : pair.femaleNickname;
        }
        

        public static (string name, string nickname) GenerateNameAndNickname(int seed)
        {
            System.Random random = new System.Random(seed);
            int nameIndex = random.Next(_names.Count);
            MushroomNamePair pair = _names[nameIndex];
            
            bool isMale = random.Next(2) == 0;
            
            string name = isMale ? pair.maleName : pair.femaleName;
            string nickname = isMale ? pair.maleNickname : pair.femaleNickname;
            
            return (name, nickname);
        }
    }
}