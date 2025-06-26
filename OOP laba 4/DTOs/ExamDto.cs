using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_laba_4.DTOs
{
    [Serializable]
    public class ExamDto
    {
        public string SubjectName { get; set; }
        public int Grade { get; set; }
        public DateTime ExamDate { get; set; }
    }
}
