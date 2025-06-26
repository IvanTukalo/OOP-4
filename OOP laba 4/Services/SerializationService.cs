using OOP_laba_4.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace OOP_laba_4.Services
{
    public class SerializationService
    {
        private readonly string _filePath;

        public SerializationService(string filePath)
        {
            _filePath = filePath;
        }

        public void Save(List<StudentDto> students)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(students, options);
            File.WriteAllText(_filePath, jsonString);
        }

        public List<StudentDto> Load()
        {
            if (!File.Exists(_filePath))
            {
                return new List<StudentDto>();
            }

            string jsonString = File.ReadAllText(_filePath);
            if (string.IsNullOrEmpty(jsonString))
            {
                return new List<StudentDto>();
            }

            try
            {
                var students = JsonSerializer.Deserialize<List<StudentDto>>(jsonString);
                return students ?? new List<StudentDto>();
            }
            catch (JsonException)
            {
                // Повернути пустий список, якщо файл пошкоджено
                return new List<StudentDto>();
            }
        }
    }
}
