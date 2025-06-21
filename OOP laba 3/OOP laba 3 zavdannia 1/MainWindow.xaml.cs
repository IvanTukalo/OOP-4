using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ButtonGenerator
{
    public partial class MainWindow : Window
    {
        private HashSet<int> _clickedButtons = new HashSet<int>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fromText = txtFrom.Text.Trim();
                string toText = txtTo.Text.Trim();
                string stepText = txtStep.Text.Trim();

                if (string.IsNullOrEmpty(fromText) || string.IsNullOrEmpty(toText) || string.IsNullOrEmpty(stepText))
                {
                    ShowError("Будь ласка, заповніть всі поля.");
                    return;
                }

                // Перевірка формату введених значень
                if (!Regex.IsMatch(fromText, @"^-?\d+$") ||
                    !Regex.IsMatch(toText, @"^-?\d+$") ||
                    !Regex.IsMatch(stepText, @"^\d+$"))
                {
                    ShowError("Введіть коректні цілі числа.");
                    return;
                }

                if (!int.TryParse(fromText, out int from) ||
                    !int.TryParse(toText, out int to) ||
                    !int.TryParse(stepText, out int step))
                {
                    ShowError("Помилка перетворення чисел.");
                    return;
                }

                if (step <= 0)
                {
                    ShowError("Крок має бути ≥ 1.");
                    return;
                }

                List<int> numbers = GenerateNumbers(from, to, step);

                if (numbers.Count > 10000)
                {
                    ShowError("Я не дозволяю вам згенерувати більше 10000 кнопок.");
                    return;
                }

                foreach (var num in numbers)
                {
                    var btn = new Button
                    {
                        Content = num.ToString(),
                        Tag = num,
                        Margin = new Thickness(5),
                        ToolTip = "Натисніть для перевірки",
                        MinWidth = num.ToString().Length * 10 + 20,
                        Focusable = true // Важливо для клавіатурної навігації
                    };
                    btn.Click += OnButtonClicked;
                    buttonContainer.Items.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowError("Помилка: " + ex.Message);
            }
        }

        private List<int> GenerateNumbers(int from, int to, int step)
        {
            var result = new List<int>();
            int current = from;
            int direction = Math.Sign(to - from);

            while (IsWithinRange(current, from, to, direction))
            {
                result.Add(current);
                current += step * direction;
            }

            return result;
        }

        private bool IsWithinRange(int value, int from, int to, int direction)
        {
            if (direction == 0) return false; // Уникнення ділення на нуль
            if (direction > 0) return value <= to;
            else return value >= to;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            int number = (int)btn.Tag;

            if (_clickedButtons.Contains(number))
            {
                MessageBox.Show($"Ти вже нажимав на цю кнопку");
                return;
            }

            _clickedButtons.Add(number);

            string message = "";

            if (number <= 0)
            {
                message = $"Число [{number}] ніяке, ні просте, ні складне.";
            }
            else
            {
                int absNumber = Math.Abs(number);
                int divisor = FindSmallestDivisor(absNumber);

                if (divisor == absNumber)
                {
                    message = $"Число [{number}] просте бо ділиться на себе";
                }
                else
                {
                    message = $"Число [{number}] складне бо ділиться на {divisor}";
                }
            }

            MessageBox.Show(message);
        }

        private int FindSmallestDivisor(int n)
        {
            for (int i = 2; i <= n; i++)
            {
                if (n % i == 0)
                    return i;
            }
            return n;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string input = txtRemoveMultiple.Text.Trim();

                // Якщо текст порожній — не показуємо помилку
                if (string.IsNullOrEmpty(input))
                {
                    return;
                }

                // Перевіряємо, чи введено лише цифри і/або мінус на початку
                if (!Regex.IsMatch(input, @"^-?\d+$"))
                {
                    ShowError("Введіть коректне ціле число.");
                    return;
                }

                if (!int.TryParse(input, out int multiple))
                {
                    ShowError("Помилка перетворення числа.");
                    return;
                }

                if (multiple <= 0)
                {
                    ShowError("Число має бути ≥ 1.");
                    return;
                }

                var buttonsToRemove = new List<Button>();
                foreach (UIElement child in buttonContainer.Items)
                {
                    if (child is Button btn && btn.Tag is int tag)
                    {
                        if (Math.Abs(tag) % Math.Abs(multiple) == 0)
                        {
                            buttonsToRemove.Add(btn);
                        }
                    }
                }

                foreach (var btn in buttonsToRemove)
                {
                    buttonContainer.Items.Remove(btn);
                }

                if (buttonsToRemove.Count == 0)
                {
                    MessageBox.Show("Не знайдено жодної кнопки, що кратна.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Помилка: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message);
            txtFrom.Clear();
            txtTo.Clear();
            txtStep.Clear();
            txtRemoveMultiple.Clear();
        }

        private void ClearInputFields()
        {
            txtFrom.Clear();
            txtTo.Clear();
            txtStep.Clear();
            txtRemoveMultiple.Clear();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Handled) return;

            // Якщо натиснути Enter — "натискаємо" активний елемент
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var focusedElement = FocusManager.GetFocusedElement(this) as UIElement;

                if (focusedElement is Button btn)
                {
                    btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    e.Handled = true;
                }
                else if (focusedElement is TextBox txt)
                {
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }

                return;
            }

            // Якщо Tab — не обробляємо
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                return;
            }

            // Обробка стрілок ↑ ↓
            if (e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Down)
            {
                e.Handled = true;

                var allFocusableElements = new List<UIElement>
        {
            txtFrom,
            txtTo,
            txtStep,
            btnGenerate,
            txtRemoveMultiple,
            btnRemove
        };

                foreach (var item in buttonContainer.Items)
                {
                    if (item is Button btn && btn.Focusable)
                    {
                        allFocusableElements.Add(btn);
                    }
                }

                if (allFocusableElements.Count == 0) return;

                var current = FocusManager.GetFocusedElement(this) as UIElement;

                int currentIndex = -1;

                for (int i = 0; i < allFocusableElements.Count; i++)
                {
                    if (allFocusableElements[i] == current)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex == -1)
                {
                    allFocusableElements[0].Focus();
                    return;
                }

                int nextIndex = e.Key == System.Windows.Input.Key.Up
                    ? (currentIndex - 1 + allFocusableElements.Count) % allFocusableElements.Count
                    : (currentIndex + 1) % allFocusableElements.Count;

                allFocusableElements[nextIndex].Focus();
            }

            // Стрілки ← → — ігноруються
            if (e.Key == System.Windows.Input.Key.Left || e.Key == System.Windows.Input.Key.Right)
            {
                e.Handled = true;
            }
        }
    }
}