using System.ComponentModel.DataAnnotations;

namespace AuthApplication.Models.PostModel
{
    public class LoginModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }

    }
}
