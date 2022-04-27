using Contracts;
using StudentsService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker
{
    public class StudentServerProvider : IStudent
    {
        private StudentDataRepository dataRepository = new StudentDataRepository();

        public void AddStudent(string indexNo, string name, string lastName)
        {
            Student student = new Student(indexNo);
            student.Name = name;
            student.LastName = lastName;
            dataRepository.AddStudent(student);
        }

        public List<Student> RetrieveAllStudents()
        {
            return dataRepository.RetrieveAllStudents().ToList<Student>();
        }
    }
}
