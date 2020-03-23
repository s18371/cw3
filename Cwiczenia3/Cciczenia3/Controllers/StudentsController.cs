using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cciczenia3.DAL;
using Cciczenia3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cciczenia3.Controllers
{
    [ApiController]
    [Route("api/students")]
    
    public class StudentsController : ControllerBase
    {
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        /*public string GetStudents()
        {
            return "Jan, Anna, Katarzyna";
            
        }*/
        /*public IActionResult GetStudents(string orderby)
        {

            return Ok(_dbService.GetStudents());
        }*/
        [HttpGet("{id}")]
        public IActionResult GetStudents(int id)
        {
            var result = new List<Student>();
            var resultEnr = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from enrollment where IdEnrollment = (select IdEnrollment from student where indexnumber = " + id + ")";// select * from students;
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    /*var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    result.Add(st);*/
                    var enrl = new Enrollment();
                    enrl.IdEnrollment = (int)dr["IdEmrollment"];
                    enrl.Semester = (int)dr["Semester"];
                    enrl.IdStudy = (int)dr["IdStudy"];
                    enrl.date = dr["StartDate"].ToString();
                    resultEnr.Add(enrl);
                }
            }


            return Ok(resultEnr);
        }
        /*[HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }
            return NotFound("Nie znaleziono studenta");
        }*/
        [HttpGet]
        public string GetStudents1(string orderBy)
        {
            return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
            

        }
        [HttpPost]
        public IActionResult CreateStudent(Student Student)
        {
            Student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(Student);
        }
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            return Ok("aktualizacja zakonczona");
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie zakonczone");
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDbService, MockBdService>();
            services.AddControllers();
        }
    }
    
}