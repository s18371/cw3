using Cciczenia3.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace Cciczenia3.Services
{
    class SqlServerDbService : IStudentsDbService
    {
        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";

        public List<Enrollment> GetStudent(string indexnumber)
        {
            var resultEnr = new List<Enrollment>();

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
            return(resultEnr);
        }

        public List<Student> GetStudents()
        {
            var result = new List<Student>();
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
                    /*var enrl = new Enrollment();
                    enrl.IdEnrollment = (int)dr["IdEnrollment"];
                    enrl.Semester = (int)dr["Semester"];
                    enrl.IdStudy = (int)dr["IdStudy"];
                    enrl.date = dr["StartDate"].ToString();
                    resultEnr.Add(enrl);*/
                }
            }
            return (result);
        }

        public Enrollment NewStudent(Student student)
        {
            /*if (!ModelState.IsValid)
            {
                var d = ModelState;
                return BadRequest("!!!");
            }*/
            var result = new List<Enrollment>();

            if (student.FirstName == null || student.IndexNumber == null || student.LastName == null || student.BirthDate == null || student.Studies == null)
            {
                return null;// BadRequest("Nie podano wszystkich danych studenta");
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
                    return null;// BadRequest("Nie ma tego na liscie Studies");
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
                    /*SqlTransaction trans = con.BeginTransaction();
                    com.Transaction = trans;*/
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
                            return null;// BadRequest("Blad podczas transakcji");
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
                            return null;// BadRequest("Blad podczas transakcji");
                        }
                    }
                }

            }


            //st.StartDate = DateTime.Now;
            st.Semester = 1;
            result.Add(st);
            return st;
        }

        public Enrollment PostProm(PostProm request)
        {
            /*if (!ModelState.IsValid)
            {
                var d = ModelState;
                return BadRequest("!!!");
            }*/
            var result = new List<Enrollment>();

            if (request.Studies.Length == 0 || request.Semester.ToString().Length == 0)
            {
                return null;//BadRequest("Nie podano wszystkich danych");
            }
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {

                com.Connection = con;
                con.Open();
                //SqlTransaction tran = con.BeginTransaction();
                //var tran = con.BeginTransaction();
                com.CommandText = "select 1 kolumna from Enrollment where semester =" + request.Semester + " and idstudy = (select idstudy from studies where name ='" + request.Studies + "')";
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
                    return null;//NotFound("nie znaleziono danycjh w tabeli enrolments");
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
                    enrol.IdEnrollment = (int)dr["IdEnrollment"];
                    enrol.IdStudy = (int)dr["IdStudy"];
                    enrol.Semester = (int)dr["Semester"];
                    enrol.StartDate = (DateTime)dr["StartDate"];
                }
                dr.Close();
                trans.Commit();
                return enrol;//Created("", enrol);

            }

        }

        /*public IActionResult PutStudent(int id)
        {
            throw new NotImplementedException();
        }*/
    }
}
