using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LoginRegistration.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace LoginRegistration.Controllers
{
    public class LoginRegController : Controller
    {
        private ProjectContext dbContext;
    
        // here we can "inject" our context service into the constructor
        public LoginRegController(ProjectContext context)
        {
            dbContext = context;
        }

        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        [Route("success")]
        public IActionResult success(LoginUser user)
        {
            ViewBag.userID = HttpContext.Session.GetInt32("UserID");
            return View();
        }
        
        [HttpGet]
        [Route("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(User user)
        {
            // Check initial ModelState
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    // You may consider returning to the View at this point
                    return View("Register");
                }
                // Initializing a PasswordHasher object, providing our User class as its
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                //Save your user object to the database
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                return RedirectToAction("Success");
            }
            return View("Register");

            
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
            
                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Incorrect Password");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                return RedirectToAction("Success");
            }
            return View("Login");
        }
    }
}
