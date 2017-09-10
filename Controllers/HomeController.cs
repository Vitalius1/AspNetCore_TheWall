using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TheWall.Models;

namespace TheWall.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbConnector _dbConnector;
 
        public HomeController(DbConnector connect)
        {
            _dbConnector = connect;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
       
        //================================== Register and login automatically ==============================================
        [HttpPost]
        [Route("register")]
        public IActionResult Register(User NewUser)
        {
            if(ModelState.IsValid){
                List<Dictionary<string, object>> User = _dbConnector.Query($"SELECT * FROM users WHERE Email = '{NewUser.Email}'");
                if(User.Count > 0)
                {
                    ViewBag.ErrorRegister = "Email already in use.";
                    return View("index");
                }
                _dbConnector.Execute($"INSERT INTO users(FirstName, LastName, Email, Password, CreatedAt, UpdatedAt) VALUES ('{NewUser.FirstName}', '{NewUser.LastName}', '{NewUser.Email}', '{NewUser.Password}', NOW(), NOW())");
                List<Dictionary<string, object>> UserReg = _dbConnector.Query($"SELECT * FROM users WHERE Email = '{NewUser.Email}'");
                HttpContext.Session.SetInt32("UserID", (int)UserReg[0]["id"]);
                HttpContext.Session.SetString("UserName", (string)UserReg[0]["FirstName"]);
                return RedirectToAction("Wall", "Wall");
            }
            return View("index");
        }

        //================================== Login a registered User ==============================================
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string LEmail, string Password)
        {
            List<Dictionary<string, object>> User = _dbConnector.Query($"SELECT * FROM users WHERE Email = '{LEmail}' AND Password = '{Password}'");
            if(User.Count > 0)
            {
                HttpContext.Session.SetInt32("UserID", (int)User[0]["id"]);
                HttpContext.Session.SetString("UserName", (string)User[0]["FirstName"]);
                return RedirectToAction("Wall", "Wall");
            }
            ViewBag.ErrorLogin = "Incorrect Login Data";
            return View("Index");
        }
    }
}
