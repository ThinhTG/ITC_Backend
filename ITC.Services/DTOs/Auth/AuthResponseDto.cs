using ITC.BusinessObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.Auth
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public UserResponse User { get; set; }
        public int Priority { get; set; } // 0: No, 1: PartnerShip, 2: Premium, 3: Advance
    }
}
