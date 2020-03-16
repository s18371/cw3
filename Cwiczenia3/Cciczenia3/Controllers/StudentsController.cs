using System;
using System.Collections.Generic;
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
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }
        public string GetStudents()
        {
            return "Jan, Anna, Katarzyna";
            
        }
        [HttpGet]
        public IActionResult GetStudents(string orderby)
        {
            return Ok(_dbService.GetStudents());
        }
        [HttpGet("{id}")]
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
        }
        /*[HttpGet]
        public string GetStudents1(string orderBy)
        {
            return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
            

        }*/
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
        /*public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDbService, MockBdService>();
            services.AddControllers();
        }*/
    }
    
}