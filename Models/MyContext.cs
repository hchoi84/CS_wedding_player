using Microsoft.EntityFrameworkCore;

namespace _06_WEDDING.Models
{
  public class MyContext : DbContext
  {
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<UserModel> Users {get;set;}
    public DbSet<WeddingModel> Weddings {get;set;}
    public DbSet<RsvpModel> Rsvps {get;set;}
  }
}