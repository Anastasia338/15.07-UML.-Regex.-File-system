//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.Json;

//namespace DictionaryApp
//{
 
//    public class WordEntry
//    {
//        public string Word { get; set; }
//        public List<string> Translations { get; set; } = new List<string>();
//    }


//    public class DictionaryData
//    {
//        public string DictionaryType { get; set; } 
//        public List<WordEntry> Entries { get; set; } = new List<WordEntry>();

//        public string GetFileName() => $"{DictionaryType.Replace(' ', '_')}.json";

//        public void Save()
//        {
//            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
//            File.WriteAllText(GetFileName(), json);
//        }

//        public static DictionaryData Load(string dictType)
//        {
//            string filename = $"{dictType.Replace(' ', '_')}.json";
//            if (!File.Exists(filename))
//            {
//                return new DictionaryData { DictionaryType = dictType };
//            }
//            var json = File.ReadAllText(filename);
//            return JsonSerializer.Deserialize<DictionaryData>(json);
//        }
//    }

//    class Program
//    {
//        static List<string> AvailableDictionaryTypes = new List<string>
//        {
//            "німецько-український",
//            "українсько-німецький"
//        };

//        static void Main()
//        {
//            Console.OutputEncoding = System.Text.Encoding.UTF8;
//            while (true)
//            {
//                Console.Clear();
//                Console.WriteLine("Головне меню ");
//                Console.WriteLine("1. Створити словник");
//                Console.WriteLine("2. Відкрити словник");
//                Console.WriteLine("0. Вихід");
//                Console.Write("Виберіть пункт меню: ");
//                var choice = Console.ReadLine();

//                if (choice == "0") break;

//                switch (choice)
//                {
//                    case "1":
//                        CreateDictionary();
//                        break;
//                    case "2":
//                        OpenDictionary();
//                        break;
//                    default:
//                        Console.WriteLine("Невірний вибір. Натисніть Enter.");
//                        Console.ReadLine();
//                        break;
//                }
//            }
//        }

//        static void CreateDictionary()
//        {
//            Console.Clear();
//            Console.WriteLine("Створення словника ");
//            Console.WriteLine("Доступні типи словників:");
//            for (int i = 0; i < AvailableDictionaryTypes.Count; i++)
//            {
//                Console.WriteLine($"{i + 1}. {AvailableDictionaryTypes[i]}");
//            }
//            Console.Write("Виберіть тип словника: ");
//            if (int.TryParse(Console.ReadLine(), out int sel) && sel >= 1 && sel <= AvailableDictionaryTypes.Count)
//            {
//                string dictType = AvailableDictionaryTypes[sel - 1];
//                var dict = new DictionaryData { DictionaryType = dictType };
//                dict.Save();
//                Console.WriteLine($"Словник \"{dictType}\" створено.");
//            }
//            else
//            {
//                Console.WriteLine("Невірний вибір.");
//            }
//            Console.WriteLine("Натисніть Enter для повернення...");
//            Console.ReadLine();
//        }

//        static void OpenDictionary()
//        {
//            Console.Clear();
//            Console.WriteLine("Відкриття словника");
//            Console.WriteLine("Доступні словники:");
//            var existingDicts = AvailableDictionaryTypes
//                .Where(t => File.Exists($"{t.Replace(' ', '_')}.json"))
//                .ToList();

//            if (!existingDicts.Any())
//            {
//                Console.WriteLine("Немає створених словників.");
//                Console.WriteLine("Натисніть Enter для повернення...");
//                Console.ReadLine();
//                return;
//            }

//            for (int i = 0; i < existingDicts.Count; i++)
//            {
//                Console.WriteLine($"{i + 1}. {existingDicts[i]}");
//            }
//            Console.Write("Виберіть словник: ");
//            if (int.TryParse(Console.ReadLine(), out int sel) && sel >= 1 && sel <= existingDicts.Count)
//            {
//                var dict = DictionaryData.Load(existingDicts[sel - 1]);
//                DictionaryMenu(dict);
//            }
//            else
//            {
//                Console.WriteLine("Невірний вибір.");
//                Console.ReadLine();
//            }
//        }

//        static void DictionaryMenu(DictionaryData dict)
//        {
//            while (true)
//            {
//                Console.Clear();
//                Console.WriteLine($"Словник: {dict.DictionaryType}");
//                Console.WriteLine("1. Додати слово та переклади");
//                Console.WriteLine("2. Замінити слово або переклад");
//                Console.WriteLine("3. Видалити слово або переклад");
//                Console.WriteLine("4. Пошук перекладу слова");
//                Console.WriteLine("5. Експорт слова з перекладами у файл");
//                Console.WriteLine("0. Назад");
//                Console.Write("Виберіть пункт меню: ");

//                var choice = Console.ReadLine();
//                switch (choice)
//                {
//                    case "1":
//                        AddWord(dict);
//                        break;
//                    case "2":
//                        ReplaceWordOrTranslation(dict);
//                        break;
//                    case "3":
//                        DeleteWordOrTranslation(dict);
//                        break;
//                    case "4":
//                        SearchTranslation(dict);
//                        break;
//                    case "5":
//                        ExportWord(dict);
//                        break;
//                    case "0":
//                        dict.Save();
//                        return;
//                    default:
//                        Console.WriteLine("Невірний вибір.");
//                        break;
//                }
//                Console.WriteLine("Натисніть Enter...");
//                Console.ReadLine();
//            }
//        }

//        static void AddWord(DictionaryData dict)
//        {
//            Console.Write("Введіть слово: ");
//            string word = Console.ReadLine().Trim();
//            if (string.IsNullOrEmpty(word))
//            {
//                Console.WriteLine("Слово не може бути порожнім.");
//                return;
//            }
//            var existing = dict.Entries.FirstOrDefault(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
//            if (existing != null)
//            {
//                Console.WriteLine("Це слово вже існує. Додамо переклад.");
//                Console.Write("Введіть переклад: ");
//                string tr = Console.ReadLine().Trim();
//                if (!string.IsNullOrEmpty(tr) && !existing.Translations.Contains(tr))
//                {
//                    existing.Translations.Add(tr);
//                    Console.WriteLine("Переклад додано.");
//                }
//                else
//                {
//                    Console.WriteLine("Переклад порожній або вже існує.");
//                }
//            }
//            else
//            {
//                Console.Write("Введіть переклади через кому: ");
//                string translationsInput = Console.ReadLine();
//                var translations = translationsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
//                    .Select(t => t.Trim())
//                    .Where(t => t.Length > 0)
//                    .Distinct()
//                    .ToList();

//                if (translations.Count == 0)
//                {
//                    Console.WriteLine("Потрібен хоча б один переклад.");
//                    return;
//                }

//                dict.Entries.Add(new WordEntry
//                {
//                    Word = word,
//                    Translations = translations
//                });
//                Console.WriteLine("Слово з перекладами додано.");
//            }
//        }

//        static void ReplaceWordOrTranslation(DictionaryData dict)
//        {
//            Console.Write("Введіть слово, яке потрібно замінити або в якому замінити переклад: ");
//            string word = Console.ReadLine().Trim();
//            var entry = dict.Entries.FirstOrDefault(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
//            if (entry == null)
//            {
//                Console.WriteLine("Слово не знайдено.");
//                return;
//            }

//            Console.WriteLine("1. Замінити слово");
//            Console.WriteLine("2. Замінити переклад");
//            Console.Write("Ваш вибір: ");
//            var choice = Console.ReadLine();

//            if (choice == "1")
//            {
//                Console.Write("Введіть нове слово: ");
//                string newWord = Console.ReadLine().Trim();
//                if (string.IsNullOrEmpty(newWord))
//                {
//                    Console.WriteLine("Слово не може бути порожнім.");
//                    return;
//                }
//                if (dict.Entries.Any(e => e.Word.Equals(newWord, StringComparison.OrdinalIgnoreCase)))
//                {
//                    Console.WriteLine("Таке слово вже існує.");
//                    return;
//                }
//                entry.Word = newWord;
//                Console.WriteLine("Слово замінено.");
//            }
//            else if (choice == "2")
//            {
//                Console.WriteLine("Існуючі переклади:");
//                for (int i = 0; i < entry.Translations.Count; i++)
//                {
//                    Console.WriteLine($"{i + 1}. {entry.Translations[i]}");
//                }
//                Console.Write("Виберіть номер перекладу для заміни: ");
//                if (int.TryParse(Console.ReadLine(), out int trIndex) && trIndex >= 1 && trIndex <= entry.Translations.Count)
//                {
//                    Console.Write("Введіть новий переклад: ");
//                    string newTr = Console.ReadLine().Trim();
//                    if (string.IsNullOrEmpty(newTr))
//                    {
//                        Console.WriteLine("Переклад не може бути порожнім.");
//                        return;
//                    }
//                    if (entry.Translations.Contains(newTr))
//                    {
//                        Console.WriteLine("Такий переклад вже існує.");
//                        return;
//                    }
//                    entry.Translations[trIndex - 1] = newTr;
//                    Console.WriteLine("Переклад замінено.");
//                }
//                else
//                {
//                    Console.WriteLine("Невірний вибір перекладу.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("Невірний вибір.");
//            }
//        }

//        static void DeleteWordOrTranslation(DictionaryData dict)
//        {
//            Console.Write("Введіть слово для видалення або видалення перекладу: ");
//            string word = Console.ReadLine().Trim();
//            var entry = dict.Entries.FirstOrDefault(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
//            if (entry == null)
//            {
//                Console.WriteLine("Слово не знайдено.");
//                return;
//            }

//            Console.WriteLine("1. Видалити слово");
//            Console.WriteLine("2. Видалити переклад");
//            Console.Write("Ваш вибір: ");
//            var choice = Console.ReadLine();

//            if (choice == "1")
//            {
//                dict.Entries.Remove(entry);
//                Console.WriteLine("Слово та всі його переклади видалені.");
//            }
//            else if (choice == "2")
//            {
//                if (entry.Translations.Count == 1)
//                {
//                    Console.WriteLine("Неможливо видалити останній переклад слова.");
//                    return;
//                }
//                Console.WriteLine("Переклади:");
//                for (int i = 0; i < entry.Translations.Count; i++)
//                {
//                    Console.WriteLine($"{i + 1}. {entry.Translations[i]}");
//                }
//                Console.Write("Виберіть номер перекладу для видалення: ");
//                if (int.TryParse(Console.ReadLine(), out int trIndex) && trIndex >= 1 && trIndex <= entry.Translations.Count)
//                {
//                    entry.Translations.RemoveAt(trIndex - 1);
//                    Console.WriteLine("Переклад видалено.");
//                }
//                else
//                {
//                    Console.WriteLine("Невірний вибір перекладу.");
//                }
//            }
//            else
//            {
//                Console.WriteLine("Невірний вибір.");
//            }
//        }

//        static void SearchTranslation(DictionaryData dict)
//        {
//            Console.Write("Введіть слово для пошуку перекладу: ");
//            string word = Console.ReadLine().Trim();
//            var entry = dict.Entries.FirstOrDefault(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
//            if (entry == null)
//            {
//                Console.WriteLine("Слово не знайдено.");
//            }
//            else
//            {
//                Console.WriteLine($"Переклади для слова \"{entry.Word}\":");
//                foreach (var tr in entry.Translations)
//                {
//                    Console.WriteLine($"- {tr}");
//                }
//            }
//        }

//        static void ExportWord(DictionaryData dict)
//        {
//            Console.Write("Введіть слово для експорту: ");
//            string word = Console.ReadLine().Trim();
//            var entry = dict.Entries.FirstOrDefault(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
//            if (entry == null)
//            {
//                Console.WriteLine("Слово не знайдено.");
//                return;
//            }

//            string exportFileName = $"{dict.DictionaryType.Replace(' ', '_')}_{entry.Word}.txt";
//            using (var writer = new StreamWriter(exportFileName))
//            {
//                writer.WriteLine($"Слово: {entry.Word}");
//                writer.WriteLine("Переклади:");
//                foreach (var tr in entry.Translations)
//                {
//                    writer.WriteLine(tr);
//                }
//            }
//            Console.WriteLine($"Експортовано у файл: {exportFileName}");
//        }
//    }
//}
