using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Dto
{
    public class LoginResponseDto
    {
        public UserDTO User { get; set; }
        public string Token { get; set; }
    }
}
