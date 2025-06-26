using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_laba_4.Models
{
    public class Exam
    {
        private string _subjectName;
        private int _grade;

        public string SubjectName
        {
            get => _subjectName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва іспиту не може бути порожньою.", nameof(SubjectName));
                _subjectName = value;
            }
        }

        public int Grade
        {
            get => _grade;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(Grade), "Оцінка повинна бути в діапазоні від 0 до 100.");
                _grade = value;
            }
        }

        public DateTime ExamDate { get; set; }

        public Exam(string subjectName, int grade, DateTime examDate)
        {
            SubjectName = subjectName;
            Grade = grade;
            ExamDate = examDate;
        }

        public override string ToString()
        {
            return $"{SubjectName}: {Grade} ({ExamDate:dd.MM.yyyy})";
        }

        public Exam Clone()
        {
            return new Exam(this.SubjectName, this.Grade, this.ExamDate);
        }
    }
}
