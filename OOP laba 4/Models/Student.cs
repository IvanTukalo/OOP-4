using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_laba_4.Models
{
    public class Student
    {
        public Person PersonInfo { get; private set; }
        public EducationLevel EducationLevel { get; set; }
        public List<Exam> Exams { get; private set; }

        public Student(Person personInfo, EducationLevel level)
        {
            PersonInfo = personInfo ?? throw new ArgumentNullException(nameof(personInfo));
            EducationLevel = level;
            Exams = new List<Exam>();
        }

        public void AddExam(Exam exam)
        {
            Exams.Add(exam ?? throw new ArgumentNullException(nameof(exam)));
        }

        public double AverageGrade
        {
            get
            {
                if (Exams == null || !Exams.Any())
                    return 0;
                return Exams.Average(e => e.Grade);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{PersonInfo.LastName} {PersonInfo.FirstName}, {EducationLevel}");
            sb.AppendLine($"Дата народження: {PersonInfo.BirthDate:dd.MM.yyyy}");
            sb.AppendLine($"Середній бал: {AverageGrade:F2}");
            sb.AppendLine("Іспити:");
            if (Exams.Any())
            {
                Exams.ForEach(e => sb.AppendLine($"- {e}"));
            }
            else
            {
                sb.AppendLine("Немає складених іспитів.");
            }
            return sb.ToString();
        }

        public string ToStringShort()
        {
            return $"{PersonInfo.LastName} {PersonInfo.FirstName}, середній бал: {AverageGrade:F2}";
        }

        public Student Clone()
        {
            var newStudent = new Student(this.PersonInfo.Clone(), this.EducationLevel);
            this.Exams.ForEach(e => newStudent.AddExam(e.Clone()));
            return newStudent;
        }
    }
}
