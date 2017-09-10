using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TheWall.Models;

namespace TheWall.Controllers
{
    public class WallController : Controller
    {
        private readonly DbConnector _dbConnector;

        public WallController(DbConnector connect)
        {
            _dbConnector = connect;
        }
        // GET: /Home/
        [HttpGet]
        [Route("wall")]
        public IActionResult Wall()
        {
            ViewBag.ErrorMessage = TempData["msgError"];
            ViewBag.ErrorComment = TempData["comError"];
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.Messages = _dbConnector.Query("select messages.id, messages.message, messages.CreatedAt, CONCAT(users.FirstName, ' ', users.LastName) AS name FROM messages JOIN users ON messages.usersID = users.id ORDER BY CreatedAt DESC");
            ViewBag.Comments = _dbConnector.Query("select comments.messagesID, comments.comment, comments.CreatedAt,  CONCAT(users.FirstName, ' ', users.LastName) AS creator FROM comments JOIN users ON comments.usersID = users.id ORDER BY comments.CreatedAt");
            return View();
        }

        [HttpPost]
        [Route("post")]
        public IActionResult Post(string message = null)
        {
            if (message == null)
            {
                TempData["msgError"] = "No empty post please...";
                return RedirectToAction("Wall");
            }
            string msg = message;
            msg = msg.Replace("'", "\\'");
            _dbConnector.Execute("INSERT INTO messages(usersID, message, CreatedAt, UpdatedAt) VALUES ('" + HttpContext.Session.GetInt32("UserID") + "', '" + msg + "', NOW(), NOW())");
            return RedirectToAction("Wall");
        }

        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(int messageID, string comment = null)
        {
            if (comment == null)
            {
                TempData["comError"] = "No empty comments please...";
                return RedirectToAction("Wall");
            }
            string com = comment;
            com = com.Replace("'", "\\'");
            _dbConnector.Execute("INSERT INTO comments(usersID, messagesID, comment, CreatedAt, UpdatedAt) VALUES ('" + HttpContext.Session.GetInt32("UserID") + "', '" + messageID + "', '" + com + "', NOW(), NOW())");
            return RedirectToAction("Wall");
        }



        [HttpGet]
        [Route("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
