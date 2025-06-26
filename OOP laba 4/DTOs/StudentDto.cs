using OOP_laba_4.Models;
using System;
using System.Collections.Generic;

namespace OOP_laba_4.DTOs
{
    [Serializable]
    public class StudentDto
    {
        public PersonDto PersonInfo { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public List<ExamDto> Exams { get; set; }
    }
}
