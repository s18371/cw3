using Cciczenia3.DTOs;
using Cciczenia3.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        public RefreshTK RefreshTk(string token);
        public string CreatePassword(LoginRequestDto req);

        public static string Create(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 40000,
                                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }
        public static bool Validate(string value, string salt, string hash) => Create(value, salt) == hash;
        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
