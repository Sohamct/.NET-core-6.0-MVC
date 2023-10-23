using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Controllers;
using MVC.Data;
using MVC.Models;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class PostsModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public static string _username = "";
        public static byte[]? ProfilePhoto = null;

        public PostsModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Feed()
        {
            var user = _context.AccountsModel!.FirstOrDefault(u => u.username == AccountsModelsController._username);

            if (user != null)
            {
                _username = user!.username;
                ProfilePhoto = user!.ProfilePhoto;
            }

            return View();
        }


        [HttpPost]
        public IActionResult CreatePost(string content)
        {
            var p = new PostsModel
            {
                content = content,
                date = DateTime.Now.ToString(),
                postedBy = AccountsModelsController._username
            };
            _context.Add(p);

            try
            {
                _context.SaveChanges();
                return Json(new { Data = p });
            }
            catch (Exception ex)
            {
                return Json(new { Error = "An error occurred while saving the post." });
            }
        }




        [HttpGet]

        public ActionResult GetFeed()
        {
            Debug.WriteLine("Getting the all posts..................................");
            // Fetch posts and users
            var posts = _context.PostsModel!.ToList();

            var users = _context.AccountsModel!.ToList();

            // Create a list to hold FeedViewModels
            List<FeedViewModel> feedViewModels = new List<FeedViewModel>();

            foreach (var user in users)
            {
                foreach (var post in posts)
                {
                    if (user.username == post.postedBy)
                    {
                        FeedViewModel fvm = new FeedViewModel
                        {
                            id = post.ID,
                            date = post.date,
                            postedBy = post.postedBy,
                            content = post.content,
                            ProfilePhoto = user.ProfilePhoto
                        };
                        feedViewModels.Add(fvm);
                    }
                }
            }
            // Fetch the ProfilePhoto for the current user (if authenticated)
            var currentUserProfilePhoto = _context.AccountsModel!.FirstOrDefault(account => account.username == AccountsModelsController._username)?.ProfilePhoto;

            // Print the items to the console
            Debug.WriteLine("FeedViewModels:");
            foreach (var item in feedViewModels)
            {
                Debug.WriteLine($"ID: {item.id}, Date: {item.date}, PostedBy: {item.postedBy}, Content: {item.content}");
            }

            Debug.WriteLine("ProfilePhoto: " + (currentUserProfilePhoto != null ? "Available" : "Not Available"));

            Debug.WriteLine("Username: " + AccountsModelsController._username);

            return Json(new { Data = feedViewModels });
        }


        [HttpGet]
        public ActionResult GetPost(int id)
        {
            var q = from p in _context.PostsModel! where p.ID == id select p;
            return Json(new { Data = q });
        }

        [HttpPost]
        public ActionResult EditPost(int id, string content)
        {
            var post = _context.PostsModel!.Find(id);

            if (post == null)
            {
                return Json(new { Error = "Post not found." });
            }

            post.content = content;
            post.date = "Edited " + DateTime.Now.ToString();

            try
            {
                _context.Update(post);
                _context.SaveChanges();
                return Json(new { Data = post });
            }
            catch (Exception ex)
            {
                return Json(new { Error = "An error occurred while editing the post: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeletePost(int id)
        {
            var post = _context.PostsModel.Find(id);

            if (post == null)
            {
                return Json(new { Error = "Post not found." });
            }

            try
            {
                _context.PostsModel.Remove(post);
                _context.SaveChanges();
                return Json(new { Data = post });
            }
            catch (Exception ex)
            {
                return Json(new { Error = "An error occurred while deleting the post." });
            }
        }

        public IActionResult Settings()
        {
            // Retrieve the user's profile data, including the profile photo, from the database.
            var userProfile = _context.AccountsModel.FirstOrDefault(u => u.username == AccountsModelsController._username); // Replace with your query criteria

            if (userProfile == null)
            {
                return NotFound(); // Handle the case where the user's profile is not found
            }

            Debug.WriteLine("ProfilePhoto length: " + (userProfile.ProfilePhoto?.Length ?? 0));
            Debug.WriteLine("Username: " + AccountsModelsController._username);

            // Create an instance of AccountsModels and populate the ProfilePhoto property.
            var userModel = new AccountsModel
            {
                ProfilePhoto = userProfile.ProfilePhoto
                // Populate other properties as needed
            };

            // Render the "Settings" view and pass the userModel as the model.
            return View("Settings", userModel);
        }
    }

}