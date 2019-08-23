using System.ComponentModel.DataAnnotations;

namespace _06_WEDDING.Models
{
  public class RsvpModel
  {
    [Key]
    public int RsvpId {get;set;}

    public int UserId {get;set;}
    public int WeddingId {get;set;}

    public UserModel User {get;set;}
    public WeddingModel Wedding {get;set;}
  }
}