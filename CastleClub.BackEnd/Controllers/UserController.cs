using CastleClub.BusinessLogic.Managers;
using CastleClub.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using CastleClub.BackEnd.Models;
using CastleClub.DataTypes.Enums;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web.Security;

namespace CastleClub.BackEnd.Controllers
{
    [Authorize(Roles="Super Admin, Admin")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult ManageUsers()
        {
            List<UserDT> users = UsersManager.GetUsers();
            ViewBag.Users = users;
            NewUserVM model = new NewUserVM();
            model.UserLevel = UserRole.NOUSER;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageUsers(NewUserVM model)
        {
            List<UserDT> users = UsersManager.GetUsers();

            if (!ModelState.IsValid)
            {
                ViewBag.Users = users;
                ViewBag.Valid = false;
                return View(model);
            }

            ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!System.Web.Security.Roles.RoleExists(UserRoleHelper.GetString(model.UserLevel)))
                {
                    System.Web.Security.Roles.CreateRole(UserRoleHelper.GetString(model.UserLevel));
                }
                System.Web.Security.Roles.AddUserToRole(user.UserName, UserRoleHelper.GetString(model.UserLevel));

                UserDT newUser = new UserDT();
                newUser.FirstName = model.FirstName;
                newUser.LastName = model.LastName;
                newUser.Email = model.Email;
                newUser.Role = model.UserLevel;
                newUser.AspNetId = user.Id;

                newUser = UsersManager.CreateUser(newUser);

                users.Add(newUser);
            }

            ViewBag.Users = users;

            return View(model);
        }

        public void DeleteUser(string userId)
        {
            int id = 0;
            Int32.TryParse(userId, out id);
            UserDT user = UsersManager.GetUser(id);

            ApplicationUser toDelte =  UserManager.FindById(user.AspNetId);
            bool result = System.Web.Security.Membership.DeleteUser(toDelte.UserName);
            if (result)
            {
                UsersManager.DeleteUser(id);
            }
        }

        public ActionResult ChangePassword()
        {
            ResetPasswordVM resetPassword = new ResetPasswordVM();
            return View(resetPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool ChangePassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = UserManager.ChangePassword(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);

                return true;
                //return result.Succeeded;
            }

            return false;
        }
    }
}