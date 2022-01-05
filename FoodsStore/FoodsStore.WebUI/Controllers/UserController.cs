using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FoodsStore.WebUI.Controllers {
    [Authorize]
    public class UserController : Controller {
        IUserRepository userRepo;
        public UserController(IUserRepository userRepo) {
            this.userRepo = userRepo;
        }
        
        public ActionResult EditProfile(string username, string returnUrl) {
            ViewBag.returnUrl = returnUrl;

            if (User.Identity.Name.Equals(username)) {
                User user = userRepo.Users.FirstOrDefault(u => u.Username.Equals(username));
                return View(user);
            } else if (!string.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Product");
            }
        }

        [HttpPost]
        public ActionResult SaveProfile(User user, HttpPostedFileBase avatar, string returnUrl) {
            if (!User.Identity.Name.Equals(user.Username)) { // If current user is not the owner of account, he cannot SaveProfile
                if (!string.IsNullOrEmpty(returnUrl)) {
                    return Redirect(returnUrl);
                } else {
                    return View("Index", "Product");
                }
            }

            bool isDuplicatedEmail = false;
            if (!string.IsNullOrEmpty(user.Email)) {
                isDuplicatedEmail = userRepo.Users
                    .Where(u => u.Email != null)
                    .FirstOrDefault(u => u.Email.Equals(user.Email) && !u.Username.Equals(user.Username)) != null;
            }
            if (isDuplicatedEmail) {
                ModelState.AddModelError("duplicatedEmail", "Email is existed!");
            }

            if (avatar != null) {
                user.ImageData = new byte[avatar.ContentLength];
                avatar.InputStream.Read(user.ImageData, 0, avatar.ContentLength);
                user.ImageMimeType = avatar.ContentType;
            }
            if (ModelState.IsValidField("FullName") && ModelState.IsValidField("Description") && ModelState.IsValidField("Email") && !isDuplicatedEmail) {
                userRepo.UpdateUser(user);
                ViewBag.StatusUpdate = "Successful!"; // ViewBag.StatusUpdate for indicate update user is success or fail in View page.
            }
            ViewBag.returnUrl = returnUrl;
            return View("EditProfile", user);
        }

        public FileContentResult GetImageUser(string username) {
            User user = userRepo.Users.FirstOrDefault(u => u.Username.Equals(username));
            if (user != null) {
                if (user.ImageData != null && !string.IsNullOrEmpty(user.ImageMimeType)) {
                    return File(user.ImageData, user.ImageMimeType);
                }
            }
            return null;
        }

        public string GetRoleName(string username) => userRepo.GetRoleName(username);
    }
}
