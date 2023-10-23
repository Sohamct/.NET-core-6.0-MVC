using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Data;
using MVC.Models;

namespace MVC.Controllers
{
    public class AccountsModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public static bool onAdminMode = false;
        public static bool incorrectInput = false;

        private static int _id = 0;
        public static string _username = "";
        public AccountsModelsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //GET:AccountsModels/Login
        public IActionResult  Login(string username, string password)
        {
            //query for username and password
            var query = from l in _context.AccountsModel select l;

            //case sensitive query, returns data if username and password from the input matches with the username and password from accounts db, otherwise null.
            query = query.Where(l => EF.Functions.Collate(l.username, "SQL_Latin1_General_CP1_CS_AS") == username 
                                && EF.Functions.Collate(l.password, "SQL_Latin1_General_CP1_CS_AS") == password);

            //check if it returns data from query
            if(query.FirstOrDefault() != null)
            {
                _id = query.First().ID;
                _username = username;
                _httpContextAccessor.HttpContext!
                    .Session.SetString("Username", username);

                return RedirectToAction("Feed", "PostsModels");
            }
            else
            {
                incorrectInput = true;
                return RedirectToAction("Login", "LandingPage");
            }
        }
        public IActionResult Logout()
        {
            _id = 0;
            _username = "";

            return RedirectToAction("Login", "LandingPage");
        }
        public ActionResult ValidatePassword(string pwd, string cpwd)
        {
            if (pwd.Equals(cpwd))
            {
                return Json(new { Data = "1" });
            }
            else
            {
                return Json(new { Data = "0" });
            }
        }
        public ActionResult GetPassword()
        {
            Console.WriteLine("Edit Password");

            var q = from p in _context.AccountsModel where _id == p.ID select p;

            return Json(new { Data = q.First().password });
        }
        [HttpPost]
        public ActionResult EditPassword(string newpass)
        {
            var accountsModel = new AccountsModel {
                ID = _id,
                username = _username,
                password = newpass
            };

            if (ModelState.IsValid)
            {
                _context.Update(accountsModel);
                _context.SaveChanges();
            }

            return Json(new { Data = accountsModel });
        }
        public ActionResult GetUserNames()
        {
            var q = from u in _context.AccountsModel select u.username;
            return Json(new { Data = q });
        }
        public ActionResult GetUsername()
        {
            return Json(new { Data = _username });
        }
        [HttpPost]
        public IActionResult CreateAccount(string username, string password)
        {
            Console.WriteLine(username + password);
            var accountsModel = new AccountsModel
            {
                username = username,
                password = password
            };

            _context.Add(accountsModel);
            _context.SaveChanges();

            _id = accountsModel.ID;
            _username = accountsModel.username!;

            return Json(new { });
        }
        [HttpPost]
        public ActionResult DeleteAccount()
        {
            var accountsModel = _context.AccountsModel!.Find(_id);
            if(accountsModel != null)
            {
                _context.AccountsModel.Remove(accountsModel);
                _context.SaveChanges();
            }
            return Json(new { Data = "\"status\" : \"success\""});
        }


        [HttpPost]
        public IActionResult UpdateProfile(IFormFile profilePhoto)
        {
            var accountModel = _context.AccountsModel!.Find(_id);
            if (accountModel == null)
            {
                return NotFound();
            }

            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    profilePhoto.CopyTo(memoryStream);
                    accountModel.ProfilePhoto = memoryStream.ToArray();

                    // Update the profile photo in the database
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Profile updated successfully." });
                }
            }

            return Json(new { success = false, message = "No file uploaded." });
        }



    }
}
