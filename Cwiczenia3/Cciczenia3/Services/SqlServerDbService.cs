using Cciczenia3.DTOs;
using Cciczenia3.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Cciczenia3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        public IConfiguration Configuration { get; set; }

        private string ConnString = "Data Source=db-mssql;Initial Catalog=s18371;Integrated Security=True";

        public SqlServerDbService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string CheckStudent(string index)
        {
            string indexStu = "";
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {

                com.Connection = con;
                com.CommandText = "select * from student where indexnumber = @indexnumber";
                com.Parameters.AddWithValue("indexnumber", index);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    indexStu = dr["IndexNumber"].ToString();
                }
            }
            return indexStu;
        }

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

        public TokenResp Login(LoginRequestDto req)
        {
            string salt = "";
            string hash = "";
            string newSalt;
            byte[] randomBytes = new byte[128 / 8];
            using (var generator =RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                newSalt = Convert.ToBase64String(randomBytes);
            }
            string login = "";
            string haslo = "";
            //string salt = "";
            
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: req.Haslo,
                                salt: Encoding.UTF8.GetBytes(newSalt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 40000,
                                numBytesRequested: 256 / 8
                                );
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                com.CommandText = "select * from student where IndexNumber = @IndexNumber";
                com.Parameters.AddWithValue("IndexNumber", req.Login);
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    login = dr["IndexNumber"].ToString();
                    haslo = dr["password"].ToString();
                    salt = dr["salt"].ToString();
                }
                dr.Close();
                var good = IStudentsDbService.Validate(req.Haslo, salt, haslo);
                if (good)
                {
                    var claims = new Claim[2];
                    if (req.Login.Equals("s18371"))
                    {
                        claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, req.Login),
                            new Claim(ClaimTypes.Role, "employee")
                        };
                    }
                    else
                    {
                        claims = new[]
                        {
                        new Claim(ClaimTypes.NameIdentifier, req.Login),
                        new Claim(ClaimTypes.Role, "student")
                        };
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken
                    (
                        issuer: "Gakko",
                        audience: "Students",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                    );
                    var token2 = new JwtSecurityTokenHandler().WriteToken(token);
                    var refreshToken = Guid.NewGuid();
                    var trok = new TokenResp();
                    //using (SqlConnection con = new SqlConnection(ConnString))
                    //using (SqlCommand com = new SqlCommand())
                    com.CommandText = "update student set refreshToekn = @refreshToken where IndexNumber = @IndexNumber2";
                    com.Parameters.AddWithValue("IndexNumber2", req.Login);
                    com.Parameters.AddWithValue("refreshToken", refreshToken);
                    com.ExecuteNonQuery();
                    trok.JWTtoken = token2;
                    trok.RefreshToken = refreshToken;
                    return trok;

                }
                else
                {
                    return null;
                }
            }
                throw new NotImplementedException();
        }

        public RefreshTK RefreshTk(string token)
        {
            string indexnumber = "";
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;

                con.Open();

                com.CommandText = "select IndexNumber from student where refreshToekn = @rt;";
                com.Parameters.AddWithValue("rt", token);
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    indexnumber = dr["IndexNumber"].ToString();
                }
                dr.Close();

                if (indexnumber != "")
                {
                    var claims = new Claim[2];
                    if (indexnumber.Equals("s18371"))
                    {
                        claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, indexnumber),
                            new Claim(ClaimTypes.Role, "employee")
                        };
                    }
                    else
                    {
                        claims = new[]
                        {
                        new Claim(ClaimTypes.NameIdentifier, indexnumber),
                        new Claim(ClaimTypes.Role, "student")
                        };
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var tokenNew = new JwtSecurityToken
                    (
                        issuer: "Gakko",
                        audience: "Students",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                    );
                    var token2 = new JwtSecurityTokenHandler().WriteToken(tokenNew);
                    var refreshToken = Guid.NewGuid();
                    com.CommandText = "update student set refreshToekn = @refreshToken where IndexNumber = @IndexNumber2";
                    com.Parameters.AddWithValue("IndexNumber2", indexnumber);
                    com.Parameters.AddWithValue("refreshToken", refreshToken);
                    com.ExecuteNonQuery();
                    return (new RefreshTK
                    {
                        JwtToken = token2,
                        RefreshToken = refreshToken
                    });
                }
                else
                {
                    return null;
                }

            }
            //throw new NotImplementedException();
        }

        public string CreatePassword(LoginRequestDto req)
        {

            string login = req.Login;
            string haslo = req.Haslo;
            string salt = IStudentsDbService.CreateSalt();
            string pass = IStudentsDbService.Create(haslo, salt);

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();
                com.Connection = con;
                com.Transaction = trans;
                try
                {
                    com.CommandText = "update student set Salt = @salt, Password = @password where IndexNumber = @login";
                    com.Parameters.AddWithValue("login", login);
                    com.Parameters.AddWithValue("salt", salt);
                    com.Parameters.AddWithValue("password", pass);
                    com.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return "blad: " + e.ToString();
                }


            }

            return "Ustawiono bezpieczne haslo";
        }
    }
}
