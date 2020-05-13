using System;
using System.Collections.Generic;

namespace Cciczenia3.Models2
{
    public partial class Patient
    {
        public Patient()
        {
            Prescription = new HashSet<Prescription>();
        }

        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }

        public virtual ICollection<Prescription> Prescription { get; set; }
    }
}
