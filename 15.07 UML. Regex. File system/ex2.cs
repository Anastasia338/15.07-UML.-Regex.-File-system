using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizApp
{
    class Program
    {
    
        static List<User> users = new List<User>();

     
        static List<Quiz> quizzes = new List<Quiz>();

       
        static User currentUser = null;

        static void Main(string[] args)
        {
            SeedQuizzes();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Ласкаво просимо до Вікторини");
                Console.WriteLine("1. Вхід");
                Console.WriteLine("2. Реєстрація");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть дію: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Login();
                    if (currentUser != null)
                    {
                        UserMenu();
                    }
                }
                else if (choice == "2")
                {
                    Register();
                }
                else if (choice == "0")
                {
                    break;
                }
            }
        }

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("Вхід");
            Console.Write("Логін: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();

            currentUser = users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (currentUser == null)
            {
                Console.WriteLine("Невірний логін або пароль.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine($"Ласкаво просимо, {currentUser.Login}!");
                Console.ReadKey();
            }
        }

        static void Register()
        {
            Console.Clear();
            Console.WriteLine("Реєстрація ");
            Console.Write("Логін: ");
            string login = Console.ReadLine();

            if (users.Any(u => u.Login == login))
            {
                Console.WriteLine("Такий логін вже існує!");
                Console.ReadKey();
                return;
            }

            Console.Write("Пароль: ");
            string password = Console.ReadLine();

            Console.Write("Дата народження (дд.мм.рррр): ");
            DateTime birthDate;
            if (!DateTime.TryParse(Console.ReadLine(), out birthDate))
            {
                Console.WriteLine("Некоректна дата!");
                Console.ReadKey();
                return;
            }

            users.Add(new User { Login = login, Password = password, BirthDate = birthDate, Results = new List<Result>() });
            Console.WriteLine("Реєстрація успішна!");
            Console.ReadKey();
        }

        static void UserMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Меню користувача {currentUser.Login} ");
                Console.WriteLine("1. Почати нову вікторину");
                Console.WriteLine("2. Переглянути результати");
                Console.WriteLine("3. Топ-20 по вікторині");
                Console.WriteLine("4. Налаштування");
                Console.WriteLine("0. Вихід");

                string choice = Console.ReadLine();

                if (choice == "1")
                    StartQuiz();
                else if (choice == "2")
                    ShowUserResults();
                else if (choice == "3")
                    ShowTop20();
                else if (choice == "4")
                    UserSettings();
                else if (choice == "0")
                {
                    currentUser = null;
                    break;
                }
            }
        }

        static void StartQuiz()
        {
            Console.Clear();
            Console.WriteLine("Оберіть вікторину ");
            for (int i = 0; i < quizzes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quizzes[i].Name}");
            }
            Console.WriteLine($"{quizzes.Count + 1}. Змішана вікторина");

            Console.Write("Вибір: ");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > quizzes.Count + 1)
            {
                Console.WriteLine("Некоректний вибір");
                Console.ReadKey();
                return;
            }

            List<Question> questionsForQuiz;

            if (choice == quizzes.Count + 1)
            {
            
                var allQuestions = quizzes.SelectMany(q => q.Questions).ToList();
                questionsForQuiz = allQuestions.OrderBy(x => Guid.NewGuid()).Take(20).ToList();
            }
            else
            {
                questionsForQuiz = quizzes[choice - 1].Questions.Take(20).ToList();
            }

            int correctCount = 0;

            for (int i = 0; i < questionsForQuiz.Count; i++)
            {
                Console.Clear();
                var q = questionsForQuiz[i];
                Console.WriteLine($"Питання {i + 1}: {q.Text}");
                for (int j = 0; j < q.Answers.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {q.Answers[j].Text}");
                }
                Console.WriteLine("Введіть номери правильних відповідей через кому:");

                var input = Console.ReadLine();
                var userAnswers = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                       .Select(x => {
                                           int num;
                                           bool parsed = int.TryParse(x.Trim(), out num);
                                           return parsed ? num - 1 : -1;
                                       })
                                       .Where(x => x >= 0 && x < q.Answers.Count)
                                       .ToList();

                var correctAnswers = q.Answers.Select((a, idx) => new { a, idx })
                                             .Where(x => x.a.IsCorrect)
                                             .Select(x => x.idx)
                                             .ToList();

              
                if (userAnswers.Count == correctAnswers.Count && !userAnswers.Except(correctAnswers).Any())
                {
                    correctCount++;
                }
            }

            Console.WriteLine($"Вікторина завершена! Правильних відповідей: {correctCount} з {questionsForQuiz.Count}");

        
            currentUser.Results.Add(new Result
            {
                QuizName = choice == quizzes.Count + 1 ? "Змішана вікторина" : quizzes[choice - 1].Name,
                DateTaken = DateTime.Now,
                Score = correctCount
            });

        
            var allResultsForQuiz = users.SelectMany(u => u.Results)
                                         .Where(r => r.QuizName == currentUser.Results.Last().QuizName)
                                         .OrderByDescending(r => r.Score)
                                         .ToList();

            int place = allResultsForQuiz.IndexOf(currentUser.Results.Last()) + 1;

            Console.WriteLine($"Ваше місце в таблиці результатів: {place}");
            Console.ReadKey();
        }

        static void ShowUserResults()
        {
            Console.Clear();
            Console.WriteLine("Ваші результати");
            if (currentUser.Results.Count == 0)
            {
                Console.WriteLine("Результатів немає.");
            }
            else
            {
                foreach (var r in currentUser.Results)
                {
                    Console.WriteLine($"{r.DateTaken}: Вікторина '{r.QuizName}', бали: {r.Score}");
                }
            }
            Console.ReadKey();
        }

        static void ShowTop20()
        {
            Console.Clear();
            Console.WriteLine("Топ-20 по вікторинах ");

            var quizNames = quizzes.Select(q => q.Name).ToList();
            quizNames.Add("Змішана вікторина");

            for (int i = 0; i < quizNames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quizNames[i]}");
            }

            Console.Write("Оберіть вікторину: ");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > quizNames.Count)
            {
                Console.WriteLine("Некоректний вибір");
                Console.ReadKey();
                return;
            }

            string selectedQuiz = quizNames[choice - 1];

            var topResults = users.SelectMany(u => u.Results
                                            .Where(r => r.QuizName == selectedQuiz))
                                  .OrderByDescending(r => r.Score)
                                  .ThenBy(r => r.DateTaken)
                                  .Take(20)
                                  .ToList();

            if (topResults.Count == 0)
            {
                Console.WriteLine("Результатів поки немає.");
            }
            else
            {
                Console.WriteLine($"Топ-20 для вікторини '{selectedQuiz}':");
                int place = 1;
                foreach (var r in topResults)
                {
                    Console.WriteLine($"{place}. {r.Score} балів - {r.DateTaken} - користувач");
                    place++;
                }
            }

            Console.ReadKey();
        }

        static void UserSettings()
        {
            Console.Clear();
            Console.WriteLine("Налаштування користувача");
            Console.WriteLine("1. Змінити пароль");
            Console.WriteLine("2. Змінити дату народження");
            Console.WriteLine("0. Назад");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введіть новий пароль: ");
                string newPassword = Console.ReadLine();
                currentUser.Password = newPassword;
                Console.WriteLine("Пароль змінено.");
            }
            else if (choice == "2")
            {
                Console.Write("Введіть нову дату народження (дд.мм.рррр): ");
                DateTime newDate;
                if (DateTime.TryParse(Console.ReadLine(), out newDate))
                {
                    currentUser.BirthDate = newDate;
                    Console.WriteLine("Дата народження змінена.");
                }
                else
                {
                    Console.WriteLine("Некоректна дата.");
                }
            }
            Console.ReadKey();
        }

     
        static void SeedQuizzes()
        {
            quizzes.Add(new Quiz
            {
                Name = "Історія",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Коли почалась Друга світова війна?",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "1939", IsCorrect = true },
                            new Answer { Text = "1941", IsCorrect = false },
                            new Answer { Text = "1914", IsCorrect = false },
                            new Answer { Text = "1945", IsCorrect = false }
                        }
                    },
                   
                }
            });

            quizzes.Add(new Quiz
            {
                Name = "Географія",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Яка найдовша річка у світі?",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Амазонка", IsCorrect = true },
                            new Answer { Text = "Ніл", IsCorrect = false },
                            new Answer { Text = "Міссісіпі", IsCorrect = false }
                        }
                    },
                  
                }
            });

           
        }
    }

  

    class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Result> Results { get; set; }
    }

    class Result
    {
        public string QuizName { get; set; }
        public DateTime DateTaken { get; set; }
        public int Score { get; set; }
    }

    class Quiz
    {
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
    }

    class Question
    {
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
    }

    class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
