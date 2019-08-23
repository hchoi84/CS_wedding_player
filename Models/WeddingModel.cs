using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _06_WEDDING.Models
{
  public class WeddingModel
  {
    [Key]
    public int WeddingId {get;set;}

    [Required(ErrorMessage="Required")]
    [Display(Name="Wedder One")]
    [DataType(DataType.Text)]
    [MinLength(2, ErrorMessage="Minimum 2 characters")]
    public string WedderOne {get;set;}
    
    [Required(ErrorMessage="Required")]
    [Display(Name="Wedder Two")]
    [DataType(DataType.Text)]
    [MinLength(2, ErrorMessage="Minimum 2 characters")]
    public string WedderTwo {get;set;}
    
    [Required(ErrorMessage="Required")]
    [Display(Name="Wedding Date")]
    [DataType(DataType.DateTime)]
    // [DateCheck]
    public DateTime Date {get;set;}
    
    [Required(ErrorMessage="Required")]
    [Display(Name="Wedding Location")]
    [DataType(DataType.Text)]
    [MinLength(2, ErrorMessage="Minimum 2 characters")]
    public string Address {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
    public List<RsvpModel> Attendees {get;set;}
    
    public int CreatorId {get;set;}
    
    [NotMapped]
    public bool IsCreator {get;set;}
    [NotMapped]
    public bool IsAttending {get;set;}
  }

  public class DateCheck : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      DateTime convertedDate = Convert.ToDateTime(value);
      if(convertedDate > DateTime.Now)
        return ValidationResult.Success;
      return new ValidationResult("Please select future date");
    }
  }
}