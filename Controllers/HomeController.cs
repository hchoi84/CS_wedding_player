using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using _06_WEDDING.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace _06_WEDDING.Controllers
{
  public class HomeController : Controller
  {
		private MyContext dbContext;
    public HomeController(MyContext context) => dbContext = context;
    private int? _uid 
    { 
      get{ return HttpContext.Session.GetInt32("uid"); }
      set{ HttpContext.Session.SetInt32("uid", (int)value); }
    }
		private string _tempMsg
    {
      get{ return HttpContext.Session.GetString("TempMsg"); }
      set{ HttpContext.Session.SetString("TempMsg", value); }
    }

    #region ***** HttpGets *****
    [HttpGet("")]
    public IActionResult Index()
    {
      if(_uid == null)
        ViewBag.LogoutBtn = "disable";
      ViewBag.TempMsg = _tempMsg;
      return View();
    }

    [HttpGet("wedding/{id}")]
    public IActionResult Info(int id)
    {
      if(_uid == null)
      {
        _tempMsg = "Please login or register";
        return RedirectToAction("Index");
      }
      WeddingModel WeddingInfo = dbContext.Weddings
        .Include(w => w.Attendees)
        .ThenInclude(a => a.User)
        .FirstOrDefault(w => w.WeddingId == id);
      return View(WeddingInfo);
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
      if(_uid == null)
      {
        _tempMsg = "Please login or register";
        return RedirectToAction("Index");
      }
      List<WeddingModel> AllWeddings = dbContext.Weddings
        .Include(w => w.Attendees)
        .ThenInclude(a => a.User)
        .ToList();
      foreach(var wedding in AllWeddings)
      {
        if(wedding.Date < DateTime.Now)
        {
          dbContext.Remove(wedding);
        }
        wedding.IsCreator = wedding.CreatorId == _uid;
        wedding.IsAttending = wedding.Attendees.Any(a => a.UserId == _uid);
      }
      AllWeddings.RemoveAll(aw => aw.Date < DateTime.Now);
      dbContext.SaveChanges();
      return View(AllWeddings);
    }

    [HttpGet("wedding/create")]
    public IActionResult Create()
    {
      if(_uid == null)
      {
        _tempMsg = "Please login or register";
        return RedirectToAction("Index");
      }
      return View();
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
      HttpContext.Session.Clear();
      _tempMsg = "Goodbye!";
      return RedirectToAction("Index");
    }
    
    [HttpGet("wedding/delete/{id}")]
    public IActionResult DeleteWedding(int id)
    {
      WeddingModel w = dbContext.Weddings.FirstOrDefault(wed => wed.WeddingId == id);
      dbContext.Weddings.Remove(w);
      dbContext.SaveChanges();
      return RedirectToAction("Dashboard");
    }
    
    [HttpGet("wedding/rsvp/{id}")]
    public IActionResult Rsvp(int id)
    {
      RsvpModel rsvp = new RsvpModel();
      rsvp.UserId = (int)_uid;
      rsvp.WeddingId = id;
      dbContext.Rsvps.Add(rsvp);
      dbContext.SaveChanges();
      return RedirectToAction("Dashboard");
    }

    [HttpGet("wedding/unrsvp/{id}")]
    public IActionResult UnRsvp(int id)
    {
      RsvpModel rsvp = dbContext.Rsvps.FirstOrDefault(r => r.UserId == _uid);
      dbContext.Remove(rsvp);
      dbContext.SaveChanges();
      return RedirectToAction("Dashboard");
    }
    #endregion

    #region ***** HttpPosts *****
    [HttpPost("user/create")]
    public IActionResult CreateUser(LogRegModel newUser)
    {
      if (ModelState.IsValid)
      {
        UserModel emailCheck = dbContext.Users.FirstOrDefault(user => user.Email == newUser.User.Password);
        if (emailCheck == null)
        {
          PasswordHasher<UserModel> Hasher = new PasswordHasher<UserModel>();
          newUser.User.Password = Hasher.HashPassword(newUser.User, newUser.User.Password);
          var nu = dbContext.Users.Add(newUser.User);
          dbContext.SaveChanges();
          _uid = nu.Entity.UserId;
          HttpContext.Session.SetString("UserName", newUser.User.FirstName);
          return RedirectToAction("Dashboard");
        }
        ModelState.AddModelError("User.Email", "Email already exists");
        return View("Index");
      }
      else
      {
        return View("Index");
      }
    }
    
    [HttpPost("user/login")]
    public IActionResult LoginUser(LogRegModel loginUser)
    {
      if (ModelState.IsValid)
      {
        UserModel emailCheck = dbContext.Users.FirstOrDefault(user => user.Email == loginUser.Login.Email);
        if (emailCheck != null)
        {
          PasswordHasher<LoginModel> Hasher = new PasswordHasher<LoginModel>();
          var result = Hasher.VerifyHashedPassword(loginUser.Login, emailCheck.Password, loginUser.Login.Password);
          if (result != 0)
          {
            _uid = emailCheck.UserId;
            HttpContext.Session.SetString("UserName", emailCheck.FirstName);
            return RedirectToAction("Dashboard");
          }
          ModelState.AddModelError("Login.Password", "Incorrect password");
          return View("Index");
        }
        else
        {
          ModelState.AddModelError("Login.Email", "Email does not exist");
          return View("Index");
        }
      }
      return View("Index");
    }
    
    [HttpPost("wedding/create")]
    public IActionResult CreateWedding(WeddingModel newWedding)
    {
      if(ModelState.IsValid)
      {
        var nw = dbContext.Weddings.Add(newWedding);
        nw.Entity.CreatorId = (int)_uid;
        dbContext.SaveChanges();
        return RedirectToAction("Info", new { id = nw.Entity.WeddingId });
      }
      return View("Create");
    }
    #endregion

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }

  public static class SessionExtensions
  {
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
      session.SetString(key, JsonConvert.SerializeObject(value));
    }
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
      string value = session.GetString(key);
      return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
  }
}
