using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_laba_4.Models
{
    public class Person
    {
        private string _firstName;
        private string _lastName;

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Ім'я не може бути порожнім.", nameof(FirstName));
                _firstName = value;
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Прізвище не може бути порожнім.", nameof(LastName));
                _lastName = value;
            }
        }

        public DateTime BirthDate { get; set; }

        public Person(string firstName, string lastName, DateTime birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public Person Clone()
        {
            return new Person(this.FirstName, this.LastName, this.BirthDate);
        }
    }
}
