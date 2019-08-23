using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _06_WEDDING.Models
{
  public class UserModel
  {
    [Key]
    public int UserId { get; set; }

    [Required(ErrorMessage="Required")]
    [Display(Name="First Name")]
    [MinLength(2, ErrorMessage="Minimum 2 characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage="Required")]
    [Display(Name="Last Name")]
    [MinLength(2, ErrorMessage="Minimum 2 characters")]
    public string LastName { get; set; }

    [Required(ErrorMessage="Required")]
    [Display(Name="Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage="Required")]
    [Display(Name="Password", Prompt="8-12. at least 1 upper, lower, and number")]
    [MinLength(8, ErrorMessage="Minimum 8 characters")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,12}$", 
                      ErrorMessage="8-12. at least 1 upper, lower, and number")]
    public string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<RsvpModel> Attending {get;set;}

    [NotMapped]
    [Required(ErrorMessage="Required")]
    [DataType(DataType.Password)]
    [Compare("Password")]
    // [ConfirmPassword]
    public string Confirm { get; set; }
  }

  public class ConfirmPassword : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      UserModel user = (UserModel)validationContext.ObjectInstance;
      if (value != null)
      {
        string checkPswd = value.ToString();
        if(user.Password != checkPswd)
          return new ValidationResult("Does not match Password");
        return ValidationResult.Success;
      }
      return new ValidationResult("Confirmation is required");
    }
  }
}