using Cciczenia3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cciczenia3.DAL
{
    public class MockBdService : IDbService
    {
        public static IEnumerable<Student> _students;

        static MockBdService()
        {
            _students = new List<Student>
            {
                new Student{idStudent=1,FirstName="Jan",LastName="Kowalski"},
                new Student{idStudent=1,FirstName="Anna",LastName="Malewski"},
                new Student{idStudent=1,FirstName="Andrzej",LastName="Andrzejewicz"}


            };
        }
        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

    }
}
