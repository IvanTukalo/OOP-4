using OOP_laba_4.Models;
using System;
using System.Windows;

namespace OOP_laba_4.Views
{
    public partial class ExamWindow : Window
    {
        public Exam CurrentExam { get; private set; }

        public ExamWindow(Exam exam)
        {
            InitializeComponent();
            CurrentExam = exam.Clone(); // Працюємо з копією
            LoadExamData();
        }

        private void LoadExamData()
        {
            SubjectNameTextBox.Text = CurrentExam.SubjectName;
            GradeTextBox.Text = CurrentExam.Grade.ToString();
            ExamDatePicker.SelectedDate = CurrentExam.ExamDate;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(GradeTextBox.Text, out int grade))
                {
                    throw new FormatException("Оцінка повинна бути цілим числом.");
                }

                CurrentExam.SubjectName = SubjectNameTextBox.Text;
                CurrentExam.Grade = grade;
                CurrentExam.ExamDate = ExamDatePicker.SelectedDate ?? DateTime.Now;

                // Валідація на рівні бізнес-об'єкта спрацює автоматично в сеттерах
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SubjectNameTextBox.Focus();
        }
    }
}