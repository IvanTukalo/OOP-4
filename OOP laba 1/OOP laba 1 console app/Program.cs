using System;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


class Program
{
    static void Main(string[] args)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.White;
        Console.Clear();

        for (int i = 0; i == 0;)
        {
            Console.WriteLine("Оберіть блок:");
            Console.WriteLine("1 - Timer.");
            Console.WriteLine("2 - Масив int().");
            Console.WriteLine("3 - Обчислення суми нескінченного ряду.");
            Console.WriteLine("4 - Делегати.");
            Console.WriteLine("6 - Перевірка правильності сортуваання.");
            int choise = Convert.ToInt32(Console.ReadLine());

            switch (choise)
            {
                case 1:
                    Block1_Timer();
                    break;
                case 2:
                    Block2_Array();
                    break;
                case 3:
                    Block3_Sum();
                    break;
                case 4:
                    Block4_Delegate();
                    break;
                case 6:
                    Block6_CheckSort();
                    break;
                default:
                    Console.WriteLine("Такого блоку не існує, людино.");
                    break;
            }
            Console.WriteLine();
        }
    }

    static void Block1_Timer()
    {
        Console.WriteLine("Запуск");

        string[] messages = { "Абрикос", "Груша", "Персик", "Яблуко" };
        Random random = new Random();
        Timer timer1 = new Timer(() => Console.WriteLine($"[{DateTime.Now:T}] {messages[new Random().Next(messages.Length)]}"), 1);
        Timer timer2 = new Timer(() => Console.WriteLine($"[{DateTime.Now:T}] {messages[new Random().Next(messages.Length)]}"), 4);
        // Створення двох таймерів з делегатами (лямбда-функціями)

        // Запуск у фоновому потоці
        timer1.StartAsync();
        timer2.StartAsync();

        Console.WriteLine("Натисніть Enter для зупинки.");
        Console.ReadLine();

        // Зупинка обох таймерів
        timer1.Stop();
        timer2.Stop();
    }

    class Timer
    {
        private readonly Action _action; // Делегат — метод, який треба викликати
        private readonly int _interval;  // Інтервал у мілісекундах
        private volatile bool _running;  // Статус: чи працює таймер
        private Task _task;              // Збереження потоку, у якому виконується таймер
        private readonly object _lock = new(); // Захист від одночасного запуску

        // Конструктор — передається метод і інтервал у секундах
        public Timer(Action action, int intervalInSeconds)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _interval = intervalInSeconds * 1000;
        }

        // Синхронний таймер (блокує головний потік)
        public void StartBlocking()
        {
            _running = true;
            while (_running)
            {
                try
                {
                    _action.Invoke(); // виклик дії
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка у виконанні таймера: {ex.Message}");
                }
                Thread.Sleep(_interval); // пауза
            }
        }

        // Асинхронний таймер (не блокує головний потік)
        public void StartAsync()
        {
            lock (_lock)
            {
                if (_running) return; // Не дозволяє запускати повторно

                _running = true;
                _task = Task.Run
                (async () =>
                {
                    while (_running)
                    {
                        try
                        {
                            _action.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Помилка у виконанні таймера: {ex.Message}");
                        }
                        await Task.Delay(_interval); // пауза між викликами
                    }
                }
                );
            }
        }

        // Зупинка таймера
        public void Stop()
        {
            _running = false;
        }
    }
    public delegate bool Isdiv(int number);
    static void Block2_Array()
    {
        Console.WriteLine("Введіть k (число, на яке перевіряється кратність):");
        int k = int.Parse(Console.ReadLine());

        int[] arr;

        // Запитуємо користувача, чи хоче він ввести свій масив
        Console.WriteLine("Бажаєте ввести свій масив? (1/будь-шо):");
        string input = Console.ReadLine()?.Trim().ToLower();

        if (input == "1")
        {
            Console.WriteLine("Введіть цілі числа через пробіл:");
            string userInput = Console.ReadLine();

            // Розбиваємо рядок по пробілах і перетворюємо на масив int
            arr = userInput.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        }
        else
        {
            // Якщо користувач не хоче вводити сам — використовуємо готовий масив
            arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            Console.WriteLine("Початковий масив: " + string.Join(", ", arr));
        }

        // Делегат-фільтр: перевірка на кратність
        Isdiv IsDiv = num => num % k == 0;

        // Варіант з LINQ
        int[] FiltredByLinq = arr.Where(num => IsDiv(num)).ToArray();

        // Варіант з ручною реалізацією
        int[] FiltredManualy = Filter(arr, IsDiv);

        Console.WriteLine("Початковий масив: " + string.Join(", ", arr));
        Console.WriteLine("Фільтрація LINQ: " + string.Join(", ", FiltredByLinq));
        Console.WriteLine("Фільтрація вручну: " + string.Join(", ", FiltredManualy));
    }


    static int[] Filter(int[] arr, Isdiv predicate)
    {
        int Lichilnik = 0; // рахуємо кількість елементів, що проходять фільтр

        // перший прохід — рахуємо розмір нового масиву
        for (int i = 0; i < arr.Length; i++)
        {
            if (predicate(arr[i]))
            {
                Lichilnik++;
            }
        }

        // створюємо новий масив необхідного розміру
        int[] result = new int[Lichilnik];
        int j = 0;

        // другий прохід — додаємо елементи
        for (int i = 0; i < arr.Length; i++)
        {
            if (predicate(arr[i]))
            {
                result[j++] = arr[i];
            }
        }

        return result;
    }

    // Делегат для обчислення n-го члена ряду
    delegate double SeriesTerm(int n);

    static void Block3_Sum()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== Обчислення суми нескінченних рядів з заданою точністю ===");

        // Вибір ряду
        Console.WriteLine("Оберіть ряд, який хочете обчислити:");
        Console.WriteLine("1 - 1 + 1/2 + 1/4 + 1/8 + ... (очікується 2)");
        Console.WriteLine("2 - 1 + 1/2! + 1/3! + 1/4! + ... (очікується Math.E - 1)");
        Console.WriteLine("3 - -1 + 1/2 - 1/4 + 1/8 - ... (очікується -2/3)");
        Console.Write("Ваш вибір (1/2/3): ");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
        {
            Console.WriteLine("Невірний вибір. Будь ласка, введіть число від 1 до 3.");
        }

        // Введення точності
        Console.Write("Введіть точність ε (наприклад, 0,0001): ");
        double epsilon;
        while (!double.TryParse(Console.ReadLine().Replace(',', '.'), out epsilon) || epsilon <= 0)
        {
            Console.WriteLine("Невірне значення точності. Будь ласка, введіть додатнє число.");
        }

        // Перевірка на коректність введення
        SeriesTerm term = null;
        string expected = "";

        // Вибір формули n-го члена відповідно до вибраного ряду
        switch (choice)
        {
            case 1:
                term = n => 1.0 / Math.Pow(2, n); // 1/2^n
                expected = "2";
                break;
            case 2:
                term = n => 1.0 / Factorial(n + 1); // 1/(n+1)! (щоб починати з 1/2!)
                expected = "Math.E - 1 ≈ 1,7182818...";
                break;
            case 3:
                term = n => Math.Pow(-1, n) / Math.Pow(2, n); // (-1)^n / 2^n
                expected = "-2/3 ≈ -0,66666...";
                break;
        }

        // Обчислення суми ряду
        double sum = SumSeries(term, epsilon);

        Console.WriteLine($"\nОбчислена сума ряду: {sum:F8}"); // Форматування до 8 знаків після коми
        Console.WriteLine($"Очікуване значення: {expected}");
    }

    /// Універсальний метод для обчислення суми ряду з заданою точністю
    static double SumSeries(SeriesTerm term, double epsilon)
    {
        double sum = 0;
        int n = 0;
        double currentTerm;

        // Додаємо члени ряду, поки вони більші або рівні точності
        do
        {
            currentTerm = term(n); // n-й член ряду
            sum += currentTerm;    // додаємо до суми
            n++;
        }
        while (Math.Abs(currentTerm) >= epsilon);

        return sum;
    }

    /// Метод для обчислення факторіалу числа
    static double Factorial(int n)
    {
        double result = 1;
        for (int i = 2; i <= n; i++)
            result *= i;
        return result;
    }

    static void Block4_Delegate()
    {
        Console.WriteLine("Введіть рядки у форматі: <номер операції> <число>");
        Console.WriteLine("Номер операції:");
        Console.WriteLine("  0 -- sqrt(abs(x))");
        Console.WriteLine("  1 -- x^3 (або x*x*x)");
        Console.WriteLine("  2 -- x + 3.5");
        Console.WriteLine("Приклад: 0 4");
        Console.WriteLine("Для завершення введіть неправильний формат.");

        // Масив делегатів для трьох функцій
        Func<double, double>[] operations =
        {
            x => Math.Sqrt(Math.Abs(x)), // sqrt(abs(x))
            x => Math.Pow(x, 3),         // x^3
            x => x + 3.5                 // x + 3.5
        };

        while (true)
        {
            try
            {
                // Зчитуємо вхідний рядок
                string input = Console.ReadLine();

                // Розбиваємо рядок на частини
                string[] parts = input.Split(' ');

                // Перевіряємо, чи є дві частини (індекс та число)
                int operationIndex = int.Parse(parts[0]); // Індекс операції
                double number = double.Parse(parts[1]);   // Число для обчислення

                // Обчислюємо результат за допомогою масиву делегатів
                double result = operations[operationIndex](number);

                // Виводимо результат
                Console.WriteLine($"Результат: {result}");
                Console.WriteLine("Введіть наступний вираз:");
            }
            catch
            {
                // Якщо трапилася помилка, завершуємо програму
                Console.WriteLine("Помилка: Невірний формат вводу.");
                Console.WriteLine("Дякуємо за користування програмою! До побачення!");
                break;
            }
        }
    }
    public delegate void SortMethod<T>(T[] array) where T : IComparable<T>;
    public delegate bool TimeValidator(long etalonTime, long studentTime);
    static void Block6_CheckSort()
    {
        Console.WriteLine("=== СИСТЕМА ПЕРЕВІРКИ СОРТУВАННЯ ===");

        Dictionary<string, int[]> testData = LoadTestCases();

        Console.WriteLine("\n[ПЕРЕВІРКА: ВИБІРКОВЕ СОРТУВАННЯ]");
        RunVerification("Selection Sort", EtalonSelectionSort, StudentSelectionSort, testData, AcceptableExecutionTime);

        Console.WriteLine("\n[ПЕРЕВІРКА: ШЕЙКЕРНЕ СОРТУВАННЯ]");
        RunVerification("Shaker Sort", EtalonShakerSort, StudentShakerSort, testData, AcceptableExecutionTime);

        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
    static Dictionary<string, int[]> LoadTestCases()
    {
        var data = new Dictionary<string, int[]>();
        string[] files = Directory.GetFiles("TestArrays", "*.txt");

        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            int[] array = File.ReadAllText(file)
                              .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(int.Parse)
                              .ToArray();
            data[name] = array;
        }

        return data;
    }

    static void RunVerification<T>(
        string sortName,
        SortMethod<T> etalon,
        SortMethod<T> student,
        Dictionary<string, T[]> datasets,
        TimeValidator timeCheck
    ) where T : IComparable<T>
    {
        foreach (var kvp in datasets)
        {
            string testName = kvp.Key;
            T[] array = kvp.Value;

            Console.WriteLine($"\n--- Тест: {testName} ---");

            long etalonTime = Measure(etalon, array, out var etalonResult);
            if (etalonTime < 0)
            {
                Console.WriteLine("Помилка в еталонному рішенні. Тест пропущено.");
                continue;
            }

            long studentTime = Measure(student, array, out var studentResult);
            if (studentTime < 0)
            {
                Console.WriteLine("Результат: - Runtime error у студентському рішенні");
                continue;
            }

            bool isCorrect = CompareArrays(etalonResult, studentResult);
            bool timeOK = timeCheck(etalonTime, studentTime);

            Console.WriteLine($"Час еталону: {etalonTime} мс | Час студента: {studentTime} мс");

            if (!isCorrect)
                Console.WriteLine("Результат: - Некоректне сортування");
            else if (!timeOK)
                Console.WriteLine("Результат: ! Перевищено ліміт часу");
            else
                Console.WriteLine("Результат: <> Прийнято");
        }
    }

    static long Measure<T>(SortMethod<T> method, T[] input, out T[] result) where T : IComparable<T>
    {
        result = (T[])input.Clone();
        var sw = Stopwatch.StartNew();
        try
        {
            method(result);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Помилка]: {ex.Message}");
            sw.Stop();
            result = null;
            return -1;
        }
    }

    static bool CompareArrays<T>(T[] a, T[] b) where T : IComparable<T>
    {
        if (a == null || b == null || a.Length != b.Length)
            return false;
        for (int i = 0; i < a.Length; i++)
            if (!a[i].Equals(b[i])) return false;
        return true;
    }

    static bool AcceptableExecutionTime(long etalon, long student)
    {
        if (etalon < 5)
        {
            return student <= 50; // жорсткий ліміт на дуже швидкі тести
        }

        long absMax = etalon + 100;       // не більше ніж на 100 мс повільніше
        long relMax = etalon * 3;         // і не більше ніж у 3 рази повільніше

        long limit = Math.Min(absMax, relMax);

        return student <= limit;
    }



    // === Еталонні сортування ===

    static void EtalonSelectionSort<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int minIdx = i;
            for (int j = i + 1; j < array.Length; j++)
                if (array[j].CompareTo(array[minIdx]) < 0)
                    minIdx = j;

            if (minIdx != i)
                (array[i], array[minIdx]) = (array[minIdx], array[i]);
        }
    }

    static void EtalonShakerSort<T>(T[] array) where T : IComparable<T>
    {
        int left = 0, right = array.Length - 1;
        while (left < right)
        {
            for (int i = left; i < right; i++)
                if (array[i].CompareTo(array[i + 1]) > 0)
                    (array[i], array[i + 1]) = (array[i + 1], array[i]);

            right--;

            for (int i = right; i > left; i--)
                if (array[i].CompareTo(array[i - 1]) < 0)
                    (array[i], array[i - 1]) = (array[i - 1], array[i]);

            left++;
        }
    }

    // === Студентські сортування ===

    static void StudentSelectionSort<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < array.Length; j++)
                if (array[j].CompareTo(array[minIndex]) < 0)
                    minIndex = j;

            if (minIndex != i)
                (array[i], array[minIndex]) = (array[minIndex], array[i]);
        }

        // Симуляція повільної реалізації
        System.Threading.Thread.Sleep(50);
    }

    static void StudentShakerSort<T>(T[] array) where T : IComparable<T>
    {
        int left = 0, right = array.Length - 1;
        while (left < right)
        {
            for (int i = left; i < right; i++)
                if (array[i].CompareTo(array[i + 1]) < 0) // ❌ некоректно
                    (array[i], array[i + 1]) = (array[i + 1], array[i]);

            right--;

            for (int i = right; i > left; i--)
                if (array[i].CompareTo(array[i - 1]) > 0) // ❌ некоректно
                    (array[i], array[i - 1]) = (array[i - 1], array[i]);

            left++;
        }
    }
}