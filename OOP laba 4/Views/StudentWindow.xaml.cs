using OOP_laba_4.Models;
using OOP_laba_4.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace OOP_laba_4.Views
{
    public partial class StudentWindow : Window
    {
        public Student CurrentStudent { get; private set; }
        private bool _isSaved = false;

        public StudentWindow(Student student)
        {
            InitializeComponent();
            CurrentStudent = student.Clone(); // Завжди працюємо з копією!
            this.Loaded += StudentWindow_Loaded;
        }

        private void StudentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Заповнення ComboBox
            EducationLevelComboBox.ItemsSource = Enum.GetValues(typeof(EducationLevel));

            LoadStudentData();
        }

        private void LoadStudentData()
        {
            FirstNameTextBox.Text = CurrentStudent.PersonInfo.FirstName;
            LastNameTextBox.Text = CurrentStudent.PersonInfo.LastName;
            BirthDatePicker.SelectedDate = CurrentStudent.PersonInfo.BirthDate;
            EducationLevelComboBox.SelectedItem = CurrentStudent.EducationLevel;
            RefreshExamsList();
        }

        private void RefreshExamsList()
        {
            ExamsListBox.ItemsSource = null;
            ExamsListBox.ItemsSource = CurrentStudent.Exams;
            EditExamButton.IsEnabled = false;
            DeleteExamButton.IsEnabled = false;
        }

        private void ExamsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = ExamsListBox.SelectedItem != null;
            EditExamButton.IsEnabled = isItemSelected;
            DeleteExamButton.IsEnabled = isItemSelected;
        }

        private void AddExamButton_Click(object sender, RoutedEventArgs e)
        {
            // Створюємо новий порожній іспит для редагування
            var newExam = new Exam("Новий предмет", 60, DateTime.Now);
            var examWindow = new ExamWindow(newExam);

            if (examWindow.ShowDialog() == true)
            {
                CurrentStudent.AddExam(examWindow.CurrentExam);
                RefreshExamsList();
            }
        }

        private void EditExamButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExamsListBox.SelectedItem is Exam selectedExam)
            {
                var examWindow = new ExamWindow(selectedExam);
                if (examWindow.ShowDialog() == true)
                {
                    // Замінюємо старий об'єкт на новий, змінений
                    int index = CurrentStudent.Exams.IndexOf(selectedExam);
                    CurrentStudent.Exams[index] = examWindow.CurrentExam;
                    RefreshExamsList();
                }
            }
        }

        private void DeleteExamButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExamsListBox.SelectedItem is Exam selectedExam)
            {
                if (MessageBox.Show($"Ви впевнені, що хочете видалити іспит '{selectedExam.SubjectName}'?",
                    "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    CurrentStudent.Exams.Remove(selectedExam);
                    RefreshExamsList();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentStudent.PersonInfo.FirstName = FirstNameTextBox.Text;
                CurrentStudent.PersonInfo.LastName = LastNameTextBox.Text;
                CurrentStudent.PersonInfo.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;
                CurrentStudent.EducationLevel = (EducationLevel)EducationLevelComboBox.SelectedItem;

                _isSaved = true;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved || this.DialogResult == false)
            {
                return; // Якщо вже зберегли або натиснули "Скасувати", нічого не питаємо
            }

            var result = MessageBox.Show("Зберегти зміни перед закриттям?", "Збереження", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true; // Не закривати вікно
            }
            else if (result == MessageBoxResult.Yes)
            {
                SaveButton_Click(null, null);
                // Якщо після спроби збереження DialogResult не став true (була помилка валідації), не закриваємо
                if (this.DialogResult != true)
                {
                    e.Cancel = true;
                }
            }
            else // No
            {
                this.DialogResult = false; // Закрити без збереження
            }
        }
    }
}