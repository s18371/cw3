using Cciczenia3.Models2;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cciczenia3.Services2
{
    
    public class EfDbService2 : IStudentsDbService2
    {
        public s18371Context _context { get; set; }
        public EfDbService2(s18371Context context)
        {
            _context = context;
        }

        public List<Student> GetStudents()
        {
            var list = _context.Student.ToList();
            return list;
        }
        public string DeleteStudent(string id)
        {
            var student =_context.Student.Where(stu => stu.IndexNumber == id).ToList();
            if (student.Count != 0)
            {

                var stu = student[0];
                _context.Remove(stu);
                _context.SaveChanges();

            }
            else
            {
                return "Nie ma takiego studenta";
            }
            return "usunieto " + id;
        }

        public string ModStudent(Models2.Student s)
        {
            string odp = "";
            
            try
            {
                var student = _context.Student.Where(stu => stu.IndexNumber == s.IndexNumber).ToList();
                if (student.Count == 0)
                {
                    //eskaUnik = 0;
                   return "bad req";
                }
                var stu = student[0];

                _context.Entry(stu).CurrentValues.SetValues(s);//połowocznie działa ale popraw
                _context.Entry(stu).State = EntityState.Modified;
                _context.SaveChanges();
                odp = "zmodyfikowano studenta o idndeksie =" + s.IndexNumber;
            }
            catch(Exception exc)
            {
                odp = "Wystapil blad transakcji";
            }
            return odp;
        }

        public string EnrollStudent(Models.Student s)
        {
            int eskaUnik = 1;
            var student = new Student();
            
            var studia = _context.Studies.Where(stu => stu.Name == s.Studies).ToList();
            if (studia.Count==0)
            {
                return ("nie ma takiego kierunku");
            }
            var unikId = _context.Student.Where(stu => stu.IndexNumber == s.IndexNumber).ToList();
            if (unikId.Count!= 0)
            {
                eskaUnik = 0;
                student = unikId[0];
            }
            var stud = new Models2.Student()
            {
                IndexNumber = s.IndexNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                BirthDate = DateTime.Parse(s.BirthDate)
                
            };
            
            var study = _context.Studies.Where(stu => stu.Name == s.Studies).FirstOrDefault();
            int idStudy = study.IdStudy;
            if ((!(_context.Enrollment.Any(enrl => enrl.IdStudy == idStudy && enrl.Semester == 1 && enrl.StartDate.Date == DateTime.Now.Date))))
            {
                if (eskaUnik == 1)
                {
                    var setStudent = new HashSet<Student> { stud };
                    var enroll = new Models2.Enrollment()
                    {
                        IdEnrollment = _context.Enrollment.Select(en => en.IdEnrollment).Max() + 1,
                        Semester = 1,
                        StartDate = DateTime.Now.Date,
                        Student = setStudent,
                    };
                    study.Enrollment.Add(enroll);
                    _context.Add(enroll);
                    _context.Attach(study);
                    _context.Entry(study).State = EntityState.Modified;

                    _context.SaveChanges();
                    return "dodano nowego studenta i nowy enroll";
                }
                else
                {
                    var setStudent = new HashSet<Student> { student };
                    var enroll = new Models2.Enrollment()
                    {
                        IdEnrollment = _context.Enrollment.Select(en => en.IdEnrollment).Max() + 1,
                        Semester = 1,
                        StartDate = DateTime.Now.Date,
                        Student = setStudent,
                    };
                    study.Enrollment.Add(enroll);
                    _context.Add(enroll);
                    _context.Attach(study);
                    _context.Entry(study).State = EntityState.Modified;

                    _context.SaveChanges();
                    return "zapisano studenta na nowy enroll";
                }
            }
            else
            {
                if (eskaUnik == 1)
                {
                    var enroll = _context.Enrollment.Where(en => en.IdStudy == idStudy && en.Semester == 1 && en.StartDate.Date == DateTime.Now.Date).FirstOrDefault();
                    enroll.Student.Add(stud);
                    _context.Attach(study);
                    _context.Entry(study).State = EntityState.Modified;
                    _context.SaveChanges();
                    return "dodano nowego studenta na instniejacy enroll";
                }
                else
                {
                    var enroll = _context.Enrollment.Where(en => en.IdStudy == idStudy && en.Semester == 1 && en.StartDate.Date == DateTime.Now.Date).FirstOrDefault();
                    enroll.Student.Add(student);
                    _context.Attach(study);
                    _context.Entry(study).State = EntityState.Modified;
                    _context.SaveChanges();
                    return "dodano studenta na instniejacy enroll";
                }
                
               
                
                
            }


        }

        public string PostProm(PostProm prom)
        {
            
            if (prom.Studies.Length == 0 || prom.Semester.ToString().Length == 0)
            {
                return null;
            }
            var study = _context.Studies.Where(stu => stu.Name == prom.Studies).FirstOrDefault();
            int idStudy = study.IdStudy;
            if (!(_context.Enrollment.Any(st => st.Semester == prom.Semester && st.IdStudy == idStudy)))
            {
                return null;
            }
            if(_context.Enrollment.Any(enrl => enrl.IdStudy == idStudy && enrl.Semester == prom.Semester + 1))
            {
                var Enroll = _context.Enrollment.Where(enrl => enrl.IdStudy == idStudy && enrl.Semester == prom.Semester+1).FirstOrDefault();
                var Enroll_old = _context.Enrollment.Where(enrl => enrl.IdStudy == idStudy && enrl.Semester == prom.Semester).OrderBy(enrl => enrl.StartDate).FirstOrDefault();
                var studt = _context.Student.Where(stu => stu.IdEnrollment == Enroll_old.IdEnrollment);
                var nstudenci = _context.Student.Where(stu => stu.IdEnrollment == Enroll.IdEnrollment);
                var set = nstudenci.Union(studt);
                Enroll.Student = set.ToHashSet();
                _context.Attach(Enroll);
                _context.Entry(Enroll).State = EntityState.Modified;
                _context.SaveChanges();
                return "przepisano " + nstudenci.Count() + " studentów na wyższy sem o id=" + Enroll.IdEnrollment.ToString();
            }
            else
            {
                var Enroll = new Enrollment();
                var oldEnroll = _context.Enrollment.Where(enrl => enrl.IdStudy == idStudy && enrl.Semester == prom.Semester).OrderBy(enrl => enrl.StartDate).FirstOrDefault();
                var studenci = _context.Student.Where(stu => stu.IdEnrollment == oldEnroll.IdEnrollment);
                Enroll.IdEnrollment = _context.Enrollment.Select(enrl => enrl.IdEnrollment).Max() + 1;
                Enroll.Semester = prom.Semester + 1;
                Enroll.StartDate = DateTime.Now.Date;
                Enroll.Student = studenci.ToHashSet();
                study.Enrollment.Add(Enroll);
                _context.Add(Enroll);
                _context.Attach(study);
                _context.Entry(study).State = EntityState.Modified;
                _context.SaveChanges();
                return "przepisano " + studenci.Count() + " studentów na wyższy sem o id=" + Enroll.IdEnrollment.ToString();
            }
            
        }
    }
}

            
        
