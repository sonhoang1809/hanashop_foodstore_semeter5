using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using FoodsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Windows;

namespace FoodsStore.WebUI.Controllers {
    public class AccountController : Controller {
        private IUserRepository userRepo;


        public AccountController(IUserRepository userRepo) {
            this.userRepo = userRepo;
        }

        [Authorize]
        public string GetFullnameUser(string username) => userRepo.Users.FirstOrDefault(u => u.Username.Equals(username)).FullName;

        public ViewResult Login() {
            return View();
        }

        private bool CheckExistUsernameOrEmail(string email = null, string username = null) {
            bool check = false;
            if (email != null) {
                if (userRepo.CheckExistEmail(email)) {
                    TempData["ErrorEmail"] = "This Email is already used !!";
                    check = true;
                }
            }
            if (username != null) {
                if (userRepo.CheckExistUsername(username)) {
                    TempData["ErrorUsername"] = "This username is already used !!";
                    check = true;
                }
            }
            return check;
        }
        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl) {
            if (CheckExistUsernameOrEmail(null, username)) {
                if (userRepo.CheckLoginDefault(username, password)) {
                    TempData["MessageWelcome"] = "Welcome " + userRepo.GetFullName(username) + " to Hana Shop !";
                    FormsAuthentication.SetAuthCookie(username, false);
                }
            } else {
                TempData["NotExistUserName"] = "This username is not exist!!";
            }
            if (!string.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Product");
            }
        }

 
        [HttpPost]
        public ActionResult LoginByGoogle(string ID, string fullname, string email, string avatar, string returnUrl) {        //ID = username
            if (!CheckExistUsernameOrEmail(email, ID)) {
                SignupByAnotherAccount(ID, fullname, email, avatar);
                TempData["MessageWelcome"] = "Welcome " + fullname + " to Hana Shop !";
                FormsAuthentication.SetAuthCookie(ID, false);
            } else {
                if (userRepo.CheckLoginAnotherAccount(ID, email)) {
                    TempData["MessageWelcome"] = "Welcome " + fullname + " to Hana Shop !";
                    FormsAuthentication.SetAuthCookie(ID, false);
                } else {
                    TempData["ErrorLogin"] = "Error to login by Google Account!!";
                }
            }
            if (!string.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Product");
            }
        }

        [HttpPost]
        public bool SignupByAnotherAccount(string username, string fullname, string email = null, string avatar = null) {
            bool result = true;
            if (email != null)
            {
                if (userRepo.CheckExistEmail(email))
                {
                    TempData["ErrorEmail"] = "This Email is already used !!";
                    result = false;
                }
            }          
            if (userRepo.CheckExistUsername(username)) {
                TempData["ErrorUsername"] = "This username is already used !!";
                result = false;
            }
            if (result) {
                User user = new User();
                user.Username = username;
                user.FullName = fullname;
                user.Password = username;//cho password = ID = username lun
                user.Email = email;
                user.RoleID = "US";
                user.ImageMimeType = "image/jpeg";
                user.DateCreated = DateTime.Now;
                var webClient = new WebClient();
                user.ImageData = webClient.DownloadData(avatar);
                userRepo.InsertUser(user);
                result = true;
            }
            return result;
        }

        [HttpPost]
        public ActionResult LoginByFacebook(string accessToken, string returnUrl) {
            UserReceiveFbModel userResponse = ExecuteFbProfileAPI(accessToken);
            string username = userResponse.Id;//username == ID
            string fullname = userResponse.Name;
            string email = userResponse.Email;
            string avatar = userResponse.Picture.Data.Url;
            if (!CheckExistUsernameOrEmail(email, username)) {
                SignupByAnotherAccount(username, fullname, email, avatar);
                TempData["MessageWelcome"] = "Welcome " + fullname + " to Hana Shop !";
                FormsAuthentication.SetAuthCookie(username, false);
            } else {
                if (userRepo.CheckLoginAnotherAccount(username, email)) {
                    TempData["MessageWelcome"] = "Welcome " + fullname + " to Hana Shop !";
                    FormsAuthentication.SetAuthCookie(username, false);
                } else {
                    TempData["ErrorLogin"] = "Error to login by Facebook Account!!";
                }
            }
            if (!string.IsNullOrEmpty(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Product");
            }
        }

        private UserReceiveFbModel ExecuteFbProfileAPI(string accessToken) {
            // bool result = false;
            string requestUri = @"https://graph.facebook.com/me?fields=id,name,email,picture&scope=email,public_profile&access_token=" + accessToken;
            //execute api
            string data = Support.Support.GetResponseFromUrlFacebook(requestUri);
            var json = new JavaScriptSerializer();
            var user = json.Deserialize<UserReceiveFbModel>(data);
            return user;
        }


        //[HttpPost]
        //public ActionResult Signup(string username, string fullname, string email, string password) {
        //    bool check = true;

        //    if (userRepo.CheckExistEmail(email)) {
        //        TempData["ErrorEmail"] = "This Email is already used !!";
        //        check = false;
        //    }
        //    if (userRepo.CheckExistUsername(username)) {
        //        TempData["ErrorUsername"] = "This username is already used !!";
        //        check = false;
        //    }
        //    if (check) {
        //        User user = new User();
        //        user.Username = username;
        //        user.FullName = fullname;
        //        user.Password = password;
        //        user.Email = email;
        //        user.RoleID = "US";
        //        user.DateCreated = DateTime.Now;
        //        userRepo.InsertUser(user);
        //    }
        //    return RedirectToAction("Index", "Product");
        //}

        public ViewResult SignUp(string returnUrl) {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user, string confirmPassword, string returnUrl) {
            bool isValidModel = ModelState.IsValidField("Username") && ModelState.IsValidField("Password") && ModelState.IsValidField("FullName") && ModelState.IsValidField("Email");
            bool isNotExistedUsername = userRepo.Users.FirstOrDefault(u => u.Username.Equals(user.Username)) == null;
            bool isMatchedPassword = false;
            bool isDuplicatedEmail = false;
            if (!string.IsNullOrEmpty(user.Email)) {
                isDuplicatedEmail = userRepo.Users
                    .Where(u => u.Email != null)
                    .FirstOrDefault(u => u.Email.Equals(user.Email) && !u.Username.Equals(user.Username)) != null;
            }
            if (isDuplicatedEmail) {
                ModelState.AddModelError("duplicatedEmail", "Email is existed!");
            }

            if (isValidModel) {
                isMatchedPassword = user.Password.Equals(confirmPassword);
                if (!isMatchedPassword) {
                    ModelState.AddModelError("passwordNotMatch", "Password is not match!");
                }
                if (isDuplicatedEmail) {
                    ModelState.AddModelError("duplicatedEmail", "This email address is already used!");
                }
                if (!isNotExistedUsername) {
                    ModelState.AddModelError("duplicatedUsername", "This username is already used!");
                }
            }

            if(isValidModel && isMatchedPassword && !isDuplicatedEmail && isNotExistedUsername) {
                user.DateCreated = DateTime.Now;
                user.RoleID = "US";
                userRepo.InsertUser(user);
                ViewBag.status = "Successful!";
            }
            ViewBag.returnUrl = returnUrl;
            return View(user);

        }

        public ActionResult Logout() {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Product");
        }
    }
}