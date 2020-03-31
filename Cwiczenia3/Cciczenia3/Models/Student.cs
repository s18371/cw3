using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cciczenia3.Models
{
    public class Student
    {
        public int idStudent { get; set; }
        [Required(ErrorMessage ="Musisz podac imie")]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Musisz podac nazwisko")]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Musisz podac index")]
        public string IndexNumber { get; set; }
        [Required(ErrorMessage = "Musisz podac kierunek")]
        public string Studies { get; set; }
        [Required(ErrorMessage = "Musisz podac date urodzenia")]
        public string BirthDate { get; set; }
    }
}
