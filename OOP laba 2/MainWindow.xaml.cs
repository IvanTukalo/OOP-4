using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OOP_laba_2
{
    public partial class MainWindow : Window
    {
        private readonly CalculatorEngine _calculator;      // Основна логіка обчислень
        private readonly CommandManager _commandManager;    // Управління командами (Undo/Redo)
        private bool _isScientificMode = false;             // Чи ввімкнена науковий режим

        public MainWindow()
        {
            InitializeComponent();
            _calculator = new CalculatorEngine();
            _commandManager = new CommandManager();

            // Підписка на клавіатуру
            KeyDown += MainWindow_KeyDown;

            // Перший оновлення дисплея
            UpdateDisplay();
        }


        /// Оновлює текст у полях виведення: результат і вираз

        private void UpdateDisplay()
        {
            ResultDisplay.Text = _calculator.DisplayValue;
            CalculationDisplay.Text = _calculator.CalculationExpression;
        }
        private void EnableInput(bool enable)
        {
            foreach (UIElement child in ButtonsGrid.Children)
            {
                if (child is Button button && !button.Content.ToString().Equals("="))
                {
                    button.IsEnabled = enable;
                }
            }
        }
        private bool IsInputTooLong(string text)
        {
            return text.Length > 22;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                    ProcessNumberInput("1");
                    break;
                case Key.D2:
                    ProcessNumberInput("2");
                    break;
                case Key.D3:
                    ProcessNumberInput("3");
                    break;
                case Key.D4:
                    ProcessNumberInput("4");
                    break;
                case Key.D5:
                    ProcessNumberInput("5");
                    break;
                case Key.D6:
                    ProcessNumberInput("6");
                    break;
                case Key.D7:
                    ProcessNumberInput("7");
                    break;
                case Key.D8:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        ProcessOperator("*");
                    else
                        ProcessNumberInput("8");
                    break;
                case Key.D9:
                    ProcessNumberInput("9");
                    break;
                case Key.D0:
                    ProcessNumberInput("0");
                    break;
                case Key.OemPlus:
                    ProcessOperator("+");
                    break;
                case Key.OemMinus:
                    ProcessOperator("-");
                    break;
                case Key.OemQuestion:
                    ProcessOperator("/");
                    break;
                case Key.Enter:
                    CalculateResult();
                    break;
                case Key.Escape:
                    Clear_Click(null, null);
                    break;
                case Key.Back:
                    Backspace_Click(null, null);
                    break;
                case Key.Decimal:
                case Key.OemPeriod:
                    Decimal_Click(null, null);
                    break;
                case Key.Z:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
                        Redo_Click(null, null);
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        Undo_Click(null, null);
                    break;
            }
        }


        /// Обробка вводу числа

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            string number = ((Button)sender).Content.ToString();
            ProcessNumberInput(number);
        }


        /// Обробка десяткового роздільника

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsInErrorState)
                _calculator.ResetErrorState();

            if (IsInputTooLong(_calculator.DisplayValue))
            {
                return;
            }

            var command = new DecimalCommand(_calculator);
            _commandManager.ExecuteCommand(command);
            UpdateDisplay();
        }


        /// Обробка операцій (+, -, *, /)

        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            string opSymbol = ((Button)sender).Content.ToString();
            string op;
            switch (opSymbol)
            {
                case "×": op = "*"; break;
                case "÷": op = "/"; break;
                default: op = opSymbol; break;
            }

            if (IsInputTooLong(_calculator.DisplayValue))
            {
                return;
            }

            ProcessOperator(op);
        }


        /// Обробка кнопки =

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            CalculateResult();
        }


        /// Обробка кнопки C

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            var command = new ClearCommand(_calculator);
            _commandManager.ExecuteCommand(command);

            UpdateDisplay();
        }


        /// Обробка кнопки CE — очищує всю чергу

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Clear(); // Очищуємо всю історію
            _calculator.Clear();     // Очищуємо все
            UpdateDisplay();
        }


        /// Обробка кнопки Backspace

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsInErrorState)
            {
                _calculator.ResetErrorState();
                UpdateDisplay();
                return;
            }

            if (!_calculator.IsNewOperation && _calculator.DisplayValue.Length > 0)
            {
                var command = new BackspaceCommand(_calculator);
                _commandManager.ExecuteCommand(command);
                UpdateDisplay();
            }
        }


        /// Обробка кнопки Undo

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Undo();
            UpdateDisplay();
        }


        /// Обробка кнопки Redo

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            _commandManager.Redo();
            UpdateDisplay();
        }


        /// Переключення між базовим і науковим режимом

        private void ToggleScientific_Click(object sender, RoutedEventArgs e)
        {
            _isScientificMode = !_isScientificMode;

            if (_isScientificMode)
            {
                ExtraColumn.Width = new GridLength(1, GridUnitType.Star);
                Width += 80;
                MenuButton.Visibility = Visibility.Collapsed;
                CollapseButton.Visibility = Visibility.Visible;

                foreach (UIElement element in ButtonsGrid.Children)
                {
                    if (Grid.GetColumn(element) == 4)
                        element.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ExtraColumn.Width = new GridLength(0);
                Width -= 80;
                MenuButton.Visibility = Visibility.Visible;
                CollapseButton.Visibility = Visibility.Collapsed;

                foreach (UIElement element in ButtonsGrid.Children)
                {
                    if (Grid.GetColumn(element) == 4)
                        element.Visibility = Visibility.Collapsed;
                }
            }
        }


        /// Обробка математичних функцій (π, √, x², ln, lg)

        private void Scientific_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsInErrorState)
            {
                _calculator.ResetErrorState();
                UpdateDisplay();
                return;
            }
            
            if (IsInputTooLong(_calculator.DisplayValue))
            {
                return;
            }

            string operation = ((Button)sender).Content.ToString();
            try
            {
                switch (operation)
                {
                    case "π":
                        if (!_calculator.IsNewOperation)
                            return;
                        var commandPi = new InputCommand(_calculator, Math.PI.ToString("F8"));
                        _commandManager.ExecuteCommand(commandPi);
                        break;
                    case "e":
                        if (!_calculator.IsNewOperation)
                            return;
                        var commandE = new InputCommand(_calculator, Math.E.ToString("F8"));
                        _commandManager.ExecuteCommand(commandE);
                        break;
                    default:
                        var command = new ScientificCommand(_calculator, operation);
                        _commandManager.ExecuteCommand(command);
                        break;
                }
                UpdateDisplay();
            }
            catch
            {
                _calculator.SetErrorState();
                UpdateDisplay();
            }
        }


        /// Змінює знак числа

        private void SignChange_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsInErrorState)
            {
                _calculator.ResetErrorState();
                UpdateDisplay();
                return;
            }

            if (IsInputTooLong(_calculator.DisplayValue))
            {
                return;
            }

            try
            {
                double value = double.Parse(_calculator.DisplayValue);
                var command = new SignChangeCommand(_calculator, value);
                _commandManager.ExecuteCommand(command);
                UpdateDisplay();
            }
            catch
            {
                _calculator.SetErrorState();
                UpdateDisplay();
            }
        }


        /// Піднести число до степеня x

        private void PowerX_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsInErrorState)
            {
                _calculator.ResetErrorState();
                UpdateDisplay();
                return;
            }

            try
            {
                var command = new PowerXCommand(_calculator);
                _commandManager.ExecuteCommand(command);
                UpdateDisplay();
            }
            catch
            {
                _calculator.SetErrorState();
                UpdateDisplay();
            }
        }


        /// Обчислення остачі від ділення

        private void Modulo_Click(object sender, RoutedEventArgs e)
        {
            if (_calculator.IsInErrorState)
            {
                _calculator.ResetErrorState();
                UpdateDisplay();
                return;
            }

            try
            {
                var command = new ModuloCommand(_calculator);
                _commandManager.ExecuteCommand(command);
                UpdateDisplay();
            }
            catch
            {
                _calculator.SetErrorState();
                UpdateDisplay();
            }
        }

        /// Виконує операцію

        private void ProcessNumberInput(string number)
        {
            if (_calculator.IsInErrorState)
                _calculator.ResetErrorState();

            if (IsInputTooLong(_calculator.DisplayValue))
            {
                return;
            }

            var command = new InputCommand(_calculator, number);
            _commandManager.ExecuteCommand(command);
            UpdateDisplay();
        }


        /// Встановлює операцію (+, -, * або /)

        private void ProcessOperator(string op)
        {
            if (_calculator.IsInErrorState)
                _calculator.ResetErrorState();

            try
            {
                if (!string.IsNullOrEmpty(_calculator.CalculationExpression) && !_calculator.IsNewOperation)
                    CalculateResult();

                var command = new OperatorCommand(_calculator, op);
                _commandManager.ExecuteCommand(command);

                UpdateDisplay();
            }
            catch
            {
                _calculator.SetErrorState();
                UpdateDisplay();
            }
        }


        /// Виконує обчислення

        private void CalculateResult()
        {
            if (_calculator.IsInErrorState || string.IsNullOrEmpty(_calculator.CalculationExpression) || _calculator.IsNewOperation)
                return;

            try
            {
                var command = new CalculateCommand(_calculator);
                _commandManager.ExecuteCommand(command);
                UpdateDisplay();
            }
            catch
            {
                _calculator.SetErrorState();
                UpdateDisplay();
            }
        }
    }


    /// Машинка для обчислень: додає, множить, підносить до степеня, вираховує корінь тощо

    public class CalculatorEngine
    {
        public string DisplayValue { get; private set; } = "0";
        public string CalculationExpression { get; private set; } = "";
        public bool IsNewOperation { get; private set; } = true;
        public bool IsInErrorState { get; private set; } = false;

        public void SetDisplayValue(string value) => DisplayValue = value;
        public void SetCalculationExpression(string expression) => CalculationExpression = expression;
        public void SetNewOperation(bool value) => IsNewOperation = value;

        public void ResetErrorState()
        {
            if (IsInErrorState)
            {
                DisplayValue = "0";
                CalculationExpression = "";
                IsNewOperation = true;
                IsInErrorState = false;
            }
        }

        public void SetErrorState()
        {
            DisplayValue = "Error";
            CalculationExpression = "";
            IsNewOperation = true;
            IsInErrorState = true;
        }

        public void AddDecimalPoint()
        {
            if (IsNewOperation)
            {
                DisplayValue = "0.";
                IsNewOperation = false;
            }
            else if (!DisplayValue.Contains("."))
            {
                DisplayValue += ".";
            }
        }

        public void Backspace()
        {
            if (DisplayValue.Length == 1 || (DisplayValue.Length == 2 && DisplayValue[0] == '-'))
            {
                DisplayValue = "0";
                IsNewOperation = true;
            }
            else
            {
                DisplayValue = DisplayValue.Substring(0, DisplayValue.Length - 1);
            }
        }

        public void Clear()
        {
            DisplayValue = "0";
            CalculationExpression = "";
            IsNewOperation = true;
        }

        public void ProcessNumber(string number)
        {
            if (IsNewOperation)
            {
                DisplayValue = number;
                IsNewOperation = false;
            }
            else
            {
                if (DisplayValue == "0" && number != ".")
                    DisplayValue = number;
                else
                    DisplayValue += number;
            }
        }

        public void ApplyOperator(string op)
        {
            if (!string.IsNullOrEmpty(DisplayValue))
            {
                if (!string.IsNullOrEmpty(CalculationExpression) && !IsNewOperation)
                {
                    CalculationExpression += $" ({DisplayValue})";
                }
                else
                {
                    CalculationExpression = $"{DisplayValue} {op}";
                }

                IsNewOperation = true;
            }
        }

        public void PerformScientificOperation(string operation, out string oldDisplay, out string oldExpression)
        {
            oldDisplay = DisplayValue;
            oldExpression = CalculationExpression;

            double input;
            if (!double.TryParse(DisplayValue, out input))
                throw new ArgumentException("Bad input!");

            double result = 0;
            string newExpression = "";

            switch (operation)
            {
                case "π":
                    result = Math.PI;
                    newExpression = "π";
                    break;
                case "e":
                    result = Math.E;
                    newExpression = "e";
                    break;
                case "√":
                    if (input < 0)
                        throw new ArgumentException("sqrt on negative");
                    result = Math.Sqrt(input);
                    newExpression = $"√({input})";
                    break;
                case "x²":
                    result = Math.Pow(input, 2);
                    newExpression = $"({input})²";
                    break;
                case "ln":
                    if (input <= 0)
                        throw new ArgumentException("ln on non-positive");
                    result = Math.Log(input);
                    newExpression = $"ln({input})";
                    break;
                case "lg":
                    if (input <= 0)
                        throw new ArgumentException("lg on non-positive");
                    result = Math.Log10(input);
                    newExpression = $"lg({input})";
                    break;
                default:
                    throw new ArgumentException("Unknown operation");
            }

            DisplayValue = result.ToString();
            CalculationExpression = newExpression;
            IsNewOperation = true;
        }

        public void Calculate(out string oldDisplay, out string oldExpression,
                               out double firstOperand, out double secondOperand, out string op)
        {
            oldDisplay = DisplayValue;
            oldExpression = CalculationExpression;

            string[] parts = CalculationExpression.Split(' ');

            if (parts.Length < 2 || !double.TryParse(parts[0], out firstOperand))
                throw new ArgumentException("Invalid expression");

            op = parts[1];

            if (!double.TryParse(DisplayValue, out secondOperand))
                throw new ArgumentException("Invalid operand");

            double result = 0;

            switch (op)
            {
                case "+": result = firstOperand + secondOperand; break;
                case "-": result = firstOperand - secondOperand; break;
                case "*": result = firstOperand * secondOperand; break;
                case "/":
                    if (secondOperand == 0)
                        throw new DivideByZeroException();
                    result = firstOperand / secondOperand;
                    break;
                case "%":
                    if (secondOperand == 0)
                        throw new DivideByZeroException();
                    result = firstOperand % secondOperand;
                    break;
                case "^":
                    result = Math.Pow(firstOperand, secondOperand);
                    break;
                default:
                    throw new ArgumentException("Unknown operator");
            }

            CalculationExpression = $"{CalculationExpression} ({DisplayValue}) =";
            DisplayValue = result.ToString();
            IsNewOperation = true;
        }
    }


    /// Інтерфейс команд, використовується для Undo/Redo

    public interface ICommand
    {
        void Execute();
        void Undo();
    }


    /// Управління командами (Undo/Redo)

    public class CommandManager
    {
        private readonly Stack<ICommand> _undoStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _redoStack = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                ICommand command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                ICommand command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }


    /// Базовий клас для всіх команд

    public abstract class CalculatorCommand : ICommand
    {
        protected readonly CalculatorEngine _calculator;
        protected string _oldDisplayValue;
        protected string _oldCalculationExpression;
        protected bool _oldIsNewOperation;

        protected CalculatorCommand(CalculatorEngine calculator)
        {
            _calculator = calculator;
            _oldDisplayValue = calculator.DisplayValue;
            _oldCalculationExpression = calculator.CalculationExpression;
            _oldIsNewOperation = calculator.IsNewOperation;
        }

        public abstract void Execute();

        public virtual void Undo()
        {
            _calculator.SetDisplayValue(_oldDisplayValue);
            _calculator.SetCalculationExpression(_oldCalculationExpression);
            _calculator.SetNewOperation(_oldIsNewOperation);
        }
    }


    /// Команда введення цифри

    public class InputCommand : CalculatorCommand
    {
        private readonly string _number;

        public InputCommand(CalculatorEngine calculator, string number) : base(calculator)
        {
            _number = number;
        }

        public override void Execute() => _calculator.ProcessNumber(_number);
    }


    /// Команда додавання десяткової точки

    public class DecimalCommand : CalculatorCommand
    {
        public DecimalCommand(CalculatorEngine calculator) : base(calculator) { }

        public override void Execute() => _calculator.AddDecimalPoint();
    }


    /// Команда видалення символу

    public class BackspaceCommand : CalculatorCommand
    {
        public BackspaceCommand(CalculatorEngine calculator) : base(calculator) { }

        public override void Execute() => _calculator.Backspace();
    }


    /// Команда установки арифметичної операції

    public class OperatorCommand : CalculatorCommand
    {
        private readonly string _operatorSymbol;

        public OperatorCommand(CalculatorEngine calculator, string operatorSymbol) : base(calculator)
        {
            _operatorSymbol = operatorSymbol;
        }

        public override void Execute() => _calculator.ApplyOperator(_operatorSymbol);
    }


    /// Команда виконання обчислення

    public class CalculateCommand : CalculatorCommand
    {
        private double _firstOperand;
        private double _secondOperand;
        private string _operatorSymbol;

        public CalculateCommand(CalculatorEngine calculator) : base(calculator) { }

        public override void Execute() => _calculator.Calculate(
            out _, out _, out _firstOperand, out _secondOperand, out _operatorSymbol);
    }


    /// Команда наукової операції (π, √, x², ln, lg)

    public class ScientificCommand : CalculatorCommand
    {
        private readonly string _operation;

        public ScientificCommand(CalculatorEngine calculator, string operation) : base(calculator)
        {
            _operation = operation;
        }

        public override void Execute() => _calculator.PerformScientificOperation(_operation, out _, out _);
    }


    /// Команда очищення

    public class ClearCommand : CalculatorCommand
    {
        public ClearCommand(CalculatorEngine calculator) : base(calculator) { }

        public override void Execute() => _calculator.Clear();
    }


    /// Команда зміни знаку

    public class SignChangeCommand : CalculatorCommand
    {
        private readonly double _value;

        public SignChangeCommand(CalculatorEngine calculator, double value) : base(calculator)
        {
            _value = value;
        }

        public override void Execute()
        {
            _calculator.SetDisplayValue((-_value).ToString());
        }
    }


    /// Команда піднесення до степеня x

    public class PowerXCommand : CalculatorCommand
    {
        public PowerXCommand(CalculatorEngine calculator) : base(calculator) { }

        public override void Execute()
        {
            _calculator.ApplyOperator("^");
        }
    }


    /// Команда остачі від ділення

    public class ModuloCommand : CalculatorCommand
    {
        public ModuloCommand(CalculatorEngine calculator) : base(calculator) { }

        public override void Execute()
        {
            _calculator.ApplyOperator("%");
        }
    }
}