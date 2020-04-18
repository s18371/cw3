using Cciczenia3.DTOs;
using Cciczenia3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cciczenia3.Services
{
    public interface IStudentsDbService
    {
        public Enrollment PostProm(PostProm request);
        public Enrollment NewStudent(Student student);
        public List<Student> GetStudents();
        public List<Enrollment> GetStudent(string idnexnumber);

        public string CheckStudent(string index);
        public TokenResp Login(LoginRequestDto req);
    }
}
