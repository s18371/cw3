using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cciczenia3.DAL;
using Cciczenia3.DTOs;
using Cciczenia3.Models;
using Cciczenia3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Cciczenia3.Controllers
{
    [ApiController]
    //[Route("api/students")]
    [Route("api/student")]
    
    public class StudentsController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }
        /*public StudentsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }*/
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";
        //private readonly IDbService _dbService;
        private readonly IStudentsDbService _IsDbService;

        public StudentsController(IStudentsDbService IsDbService)
        {
            _IsDbService = IsDbService;
        }
        /*public string GetStudents()
        {
            return "Jan, Anna, Katarzyna";
            
        }*/
        /*public IActionResult GetStudents(string orderby)
        {
            return Ok(_dbService.GetStudents());
        }*/
        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> lista = _IsDbService.GetStudents();
            return Ok(lista);
            /*var result = new List<Student>();
            var resultEnr = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student"; //"select * from Enrollment where IdEnrollment = (select IdEnrollment from student where indexnumber = '"+id+"')";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    result.Add(st);
                    //var enrl = new Enrollment();
                    //enrl.IdEnrollment = (int)dr["IdEnrollment"];
                    //enrl.Semester = (int)dr["Semester"];
                    //enrl.IdStudy = (int)dr["IdStudy"];
                    //enrl.date = dr["StartDate"].ToString();
                    //resultEnr.Add(enrl);
                }
            }
            return Ok(result);*/
        }
        [HttpGet("{indexnumber}")]
        public IActionResult GetStudent(string indexnumber)
        {
            //
            List<Enrollment> lista = _IsDbService.GetStudent(indexnumber);
            return Ok(lista);
            /*var resultEnr = new List<Enrollment>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
             
                com.CommandText = "select * from enrollment where IdEnrollment = (select IdEnrollment from student where indexnumber = @indexnumber)";
                com.Parameters.AddWithValue("indexnumber", indexnumber);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var enh = new Enrollment();
                    enh.IdEnrollment = (int)dr["IdEnrollment"];
                    enh.IdStudy = (int)dr["IdStudy"];
                    enh.Semester = (int)dr["Semester"];
                    enh.StartDate = (DateTime)dr["StartDate"];
                    resultEnr.Add(enh);
                }

            }
                return Ok(resultEnr);*/
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
        /*[HttpGet]
        public string GetStudents1(string orderBy)
        {
            return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
            

        }*/
        /*[HttpPost]
        public IActionResult CreateStudent(Student Student)
        {
            Student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(Student);
        }*/
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
        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {
            TokenResp resp = _IsDbService.Login(request);
            if (resp != null)
            {
                return Ok(new
                {
                    token = resp.JWTtoken,
                    refreshToken = resp.RefreshToken
                });
            }
            else
            {
                return Unauthorized();
            }
            
        }
        [Route("refresh/{token}")]
        [HttpPost]
        public IActionResult Refresh(string token)
        {
            RefreshTK resp = _IsDbService.RefreshTk(token);
            return Ok(new
            {
                token = resp.JwtToken,
                refreshToken = resp.RefreshToken
            }) ;
        }
        [Route("createSafePassword")]
        [HttpPost]
        public IActionResult CreatePassword(LoginRequestDto req)
        {
            string wynik = _IsDbService.CreatePassword(req);
            return Ok(wynik);
        }
    }
    
}