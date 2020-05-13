using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cciczenia3.Models2;
using Cciczenia3.Services2;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cciczenia3.Controllers2
{
    [ApiController]
    [Route("api/cw10/enroll")]
    public class EnrollmentController2 : ControllerBase
    {
        private IStudentsDbService2 _context;

        public EnrollmentController2(IStudentsDbService2 context)
        {
            _context = context;
        }
        // GET: /<controller>/
        [HttpPost]
        [Route("enrl")]
        public IActionResult EnrollStudent(Models.Student s)
        {
            var operacja = _context.EnrollStudent(s);
            if(operacja.Equals("nie ma takiego kierunku"))
            {
                return NotFound(operacja);
            }
            return Ok(operacja);
        }
        [HttpPost]
        [Route("prom")]
        public IActionResult PostProm(PostProm request)
        {
            var enrl = _context.PostProm(request);
            if (enrl == null)
            {
                return BadRequest("nie ma takich studiów lub nie uzupełniono wszytskich danych");
            }
            return Created("",enrl);
        }
    }
}
