using Cciczenia3.Models2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cciczenia3.Services2
{
    public interface IStudentsDbService2 
    {
        public List<Student> GetStudents();

        public string DeleteStudent(string id);

        public string ModStudent(Models2.Student s);
        public string EnrollStudent(Models.Student s);
        public string PostProm(PostProm prom);
    }
}
