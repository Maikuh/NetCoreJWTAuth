using JwtExample.Data;

namespace JwtExample.DTOs
{
    public class RegisterDTO
    {
        public ApplicationUser User { get; set; }
        public string Password { get; set; }
    }
}
