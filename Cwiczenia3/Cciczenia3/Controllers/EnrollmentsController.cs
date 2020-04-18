using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cciczenia3.DAL;
using Cciczenia3.Models;
using Cciczenia3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cciczenia3.Controllers
{
    [ApiController]
    
    public class EnrollmentsController : ControllerBase
    {
        /*Trochę pomieszalem pojecia i w mojej bazie w tabeli studies sa skroty przedmiotow
         * np PPJ zamiast nazw studiow IT itp
         */
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";
        //private readonly IDbService _dbService;
        private readonly IStudentsDbService _IsDbService;

        public EnrollmentsController(IStudentsDbService IsDbService)
        {
            _IsDbService = IsDbService;
        }
        [HttpPost]
        [Route("api/enrollments")]
        [Authorize(Roles = "employee")]
        public IActionResult NewStudent(Student student)
        {
            Enrollment enrl = _IsDbService.NewStudent(student);
            if (enrl != null)
            {
                return Created("", enrl);
            }
            return BadRequest("Nie podano wszystkich danych studenta lub nie ma tego na liscie studies");
            /*if (!ModelState.IsValid)
            {
                var d = ModelState;
                return BadRequest("!!!");
            }
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
                con.Open();
                //SqlTransaction tran = con.BeginTransaction();
                //var tran = con.BeginTransaction();


                st.IdStudy = 0;

                com.CommandText = "select * from studies where name = '" + student.Studies + "'";
                com.Parameters.AddWithValue("name", student.Studies);
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    st.IdStudy = (int)dr["idStudy"];

                }
                else if (st.IdStudy == 0)
                {

                    //tran.Rollback();
                    return BadRequest("Nie ma tego na liscie Studies");
                }
                dr.Close();
                st.IdEnrollment = 0;
                com.CommandText = "select idEnrollment from enrollment where semester=1 and idstudy =  (select idstudy from Studies where name ='" + student.Studies + "') and StartDate = (select max(StartDate) from enrollment where Semester = 1 and Idstudy = (select idStudy from studies where Name ='" + student.Studies + "'))";

                //com.Parameters.AddWithValue("name", student.Studies);
                var dr2 = com.ExecuteReader();
                if (dr2.Read())
                {
                    st.IdEnrollment = (int)dr2["IdEnrollment"];
                    
                }
                else if (st.IdEnrollment == 0)
                {
                    dr2.Close();
                    var sprawdzenieEski = new List<string>();
                    com.CommandText = "select indexNumber from students";
                    var dr3 = com.ExecuteReader();
                    while (dr3.Read())
                    {
                        string eska = dr3["indexNumber"].ToString();
                        sprawdzenieEski.Add(eska);
                    }
                    
                    int eskaUnik = 1;
                    foreach (string es in sprawdzenieEski)
                    {
                        if (student.IndexNumber.Equals(es))
                        {
                            eskaUnik++;
                        }
                    }
                    //SqlTransaction trans = con.BeginTransaction();
                    //com.Transaction = trans;
                    if (eskaUnik == 1)
                    {
                        SqlTransaction trans = con.BeginTransaction();
                        com.Transaction = trans;
                        try
                        {

                            st.StartDate = DateTime.Now;
                            com.CommandText = "insert into Enrollment(idEnrollment,Semester,IdStudy,StartDate)" +
                                                "values(" + @st.IdEnrollment + "," + 1 + "," + st.IdStudy + "," + st.StartDate + ")";
                            com.ExecuteNonQuery();
                            com.CommandText = "insert into student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) values ('" + student.IndexNumber + "', '"
                                        + student.FirstName + "', '" + student.LastName + "', '" + student.BirthDate.Split(".")[2] + "-" + student.BirthDate.Split(".")[1] + "-" + student.BirthDate.Split(".")[0] + "', (select max(IdEnrollment) from Enrollment));";
                            com.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            return BadRequest("Blad podczas transakcji");
                        }
                    }
                    else
                    {
                        SqlTransaction trans = con.BeginTransaction();
                        com.Transaction = trans;
                        try
                        {
                            st.StartDate = DateTime.Now;
                            com.CommandText = "insert into Enrollment(idEnrollment,Semester,IdStudy,StartDate)" +
                                                "values(" + @st.IdEnrollment + "," + 1 + "," + st.IdStudy + "," + st.StartDate + ")";
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            return BadRequest("Blad podczas transakcji");
                        }
                    }
                }

            }


            //st.StartDate = DateTime.Now;
            st.Semester = 1;
            result.Add(st);
            return Created("", st);*/
        }
        [Route("api/enrollments/promotions")]
        [Authorize(Roles = "employee")]
        [HttpPost]
        public IActionResult PostProm(PostProm request)
        {
            Enrollment enrl = _IsDbService.PostProm(request);
            if (enrl != null)
            {
                return Created("", enrl);
            }
            return NotFound("nie znaleziono danycjh w tabeli enrolments");
            /*if (!ModelState.IsValid)
            {
                var d = ModelState;
                return BadRequest("!!!");
            }
            var result = new List<Enrollment>();

            if (request.Studies.Length==0 || request.Semester.ToString().Length==0 )
            {
                return BadRequest("Nie podano wszystkich danych");
            }
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();
                //SqlTransaction tran = con.BeginTransaction();
                //var tran = con.BeginTransaction();
                com.CommandText = "select 1 kolumna from Enrollment where semester ="+request.Semester+" and idstudy = (select idstudy from studies where name ='"+request.Studies+"')";
                com.Parameters.AddWithValue("semester", request.Semester);
                com.Parameters.AddWithValue("name", request.Studies);
                var dr = com.ExecuteReader();
                int jest = 0;
                if (dr.Read())
                {
                    jest = (int)dr["kolumna"];
                }
                if (jest == 0)
                {
                    return NotFound("nie znaleziono danycjh w tabeli enrolments");
                }
                dr.Close();
                var trans = con.BeginTransaction();
                com.Transaction = trans;

                com.CommandText = "exec promotion @name2, @semester2";
                com.Parameters.AddWithValue("semester2", request.Semester);
                com.Parameters.AddWithValue("name2", request.Studies);
                com.ExecuteNonQuery();
                var enrol = new Enrollment();
                com.CommandText = "select * from Enrollment join Studies on Enrollment.IdStudy = Studies.IdStudy where Semester = @semester3 +1 and Name = @name3";
                com.Parameters.AddWithValue("semester3", request.Semester);
                com.Parameters.AddWithValue("name3", request.Studies);

                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    enrol.IdEnrollment= (int)dr["IdEnrollment"];
                    enrol.IdStudy = (int)dr["IdStudy"];
                    enrol.Semester = (int)dr["Semester"];
                    enrol.StartDate = (DateTime)dr["StartDate"];
                }
                dr.Close();
                trans.Commit();
                return Created("",enrol);
                */
        }

                 
    }



}
    



