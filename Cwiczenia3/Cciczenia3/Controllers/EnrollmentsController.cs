using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cciczenia3.DAL;
using Cciczenia3.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cciczenia3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";
        private readonly IDbService _dbService;

        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        [HttpPost]
        public IActionResult NewStudent(Student student)
        {
            var result = new List<Enrollment>();

            if (student.FirstName == null || student.IndexNumber == null || student.LastName == null || student.BirthDate == null || student.Studies == null)
            {
                return BadRequest("Nie podano wszystkich danych studenta");
            }
            var st = new Enrollment();
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                    com.Connection = con;
                    com.CommandText = "select * from studies where name = '" + student.Studies + "'";
                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        st.IdStudy = (int)dr["idStudy"];
                  
                    }
                    else {
                        return BadRequest("Nie ma tego na liscie Studies");
                    }
            }

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select max(idenrollment) kolumna from enrollment";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    st.IdEnrollment = (int)dr["kolumna"];

                }
            }

            st.Semester = 1;
            result.Add(st);
            return Ok(result);
        }
    }
}
