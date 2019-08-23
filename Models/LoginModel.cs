using System.ComponentModel.DataAnnotations;

namespace _06_WEDDING.Models
{
  public class LoginModel
  {
    [Required]
    [Display(Name="Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
  }
}