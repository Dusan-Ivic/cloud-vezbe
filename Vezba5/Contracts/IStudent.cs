using StudentsService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IStudent
    {
        [OperationContract]
        List<Student> RetrieveAllStudents();

        [OperationContract]
        void AddStudent(string indexNo, string name, string lastName);
    }
}
