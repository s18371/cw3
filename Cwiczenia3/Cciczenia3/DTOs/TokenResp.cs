using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cciczenia3.DTOs
{
    public class TokenResp
    {
        public string JWTtoken { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
