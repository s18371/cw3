using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cciczenia3.Models2;
using Cciczenia3.Services2;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cciczenia3.Controllers2
{
    [ApiController]
    [Route("api/cw10")]
    public class StudentsController2 : ControllerBase
    {
        private IStudentsDbService2 _context; 
        // GET: api/<controller>
        public StudentsController2(IStudentsDbService2 context)
        {
            _context = context;

        }
        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> lista = _context.GetStudents();
            return Ok(lista);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        /*[HttpPost]
        public void Post([FromBody]string value)
        {
        }*/

        // PUT api/<controller>/5
        [HttpPost]
        public IActionResult ModStudent(Models2.Student s)
        {
            Student stu = s;
            var operacja = _context.ModStudent(stu);
            if(operacja=="bad req")
            {
                return BadRequest("nie ma takiej eski");
            }else if (operacja== "Wystapil blad transakcji")
            {
                return BadRequest(operacja);
            }
            return Ok(operacja);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStudents(string id)
        {
            var operacja = _context.DeleteStudent(id);
            return Ok(operacja);
        }
    }
}
