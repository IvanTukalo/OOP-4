using OOP_laba_4.DTOs;
using OOP_laba_4.Models;
using OOP_laba_4.Services;
using OOP_laba_4.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OOP_laba_4.Views
{
    public partial class MainWindow : Window
    {
        private List<Student> _students;
        private readonly SerializationService _serializationService;
        private const string FileName = "students.json";

        public MainWindow()
        {
            InitializeComponent();
            _serializationService = new SerializationService(FileName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var studentDtos = _serializationService.Load();
            _students = studentDtos.Select(dto =>
            {
                var person = new Person(dto.PersonInfo.FirstName, dto.PersonInfo.LastName, dto.PersonInfo.BirthDate);
                var student = new Student(person, dto.EducationLevel);
                var exams = dto.Exams
                    .Select(exDto => new Exam(exDto.SubjectName, exDto.Grade, exDto.ExamDate))
                    .ToList();
                exams.ForEach(ex => student.AddExam(ex));
                return student;
            }).ToList();

            RefreshStudentsList();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var studentDtos = _students.Select(student => new StudentDto
            {
                PersonInfo = new PersonDto
                {
                    FirstName = student.PersonInfo.FirstName,
                    LastName = student.PersonInfo.LastName,
                    BirthDate = student.PersonInfo.BirthDate
                },
                EducationLevel = student.EducationLevel,
                Exams = student.Exams.Select(exam => new ExamDto
                {
                    SubjectName = exam.SubjectName,
                    Grade = exam.Grade,
                    ExamDate = exam.ExamDate
                }).ToList()
            }).ToList();

            _serializationService.Save(studentDtos);
        }

        private void RefreshStudentsList()
        {
            StudentsListView.ItemsSource = null;
            StudentsListView.ItemsSource = _students;
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            DetailsButton.IsEnabled = false;
        }

        private void StudentsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = StudentsListView.SelectedItem != null;
            EditButton.IsEnabled = isItemSelected;
            DeleteButton.IsEnabled = isItemSelected;
            DetailsButton.IsEnabled = isItemSelected;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var defaultPerson = new Person("Ім'я", "Прізвище", System.DateTime.Now.AddYears(-18));
            var newStudent = new Student(defaultPerson, EducationLevel.Bachelor);

            var studentWindow = new StudentWindow(newStudent);
            if (studentWindow.ShowDialog() == true)
            {
                _students.Add(studentWindow.CurrentStudent);
                RefreshStudentsList();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsListView.SelectedItem is Student selectedStudent)
            {
                var studentWindow = new StudentWindow(selectedStudent);
                if (studentWindow.ShowDialog() == true)
                {
                    int index = _students.IndexOf(selectedStudent);
                    _students[index] = studentWindow.CurrentStudent;
                    RefreshStudentsList();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsListView.SelectedItem is Student selectedStudent)
            {
                if (MessageBox.Show($"Ви впевнені, що хочете видалити студента '{selectedStudent.PersonInfo.LastName}'?",
                    "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _students.Remove(selectedStudent);
                    RefreshStudentsList();
                }
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsListView.SelectedItem is Student selectedStudent)
            {
                MessageBox.Show(selectedStudent.ToString(), "Детальна інформація");
            }
        }
    }
}