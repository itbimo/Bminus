using Microsoft.AspNetCore.Identity;

namespace Bminus.Models
{
    public class User
    {
        public string Nickname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsPremium { get; set; }
    }
}