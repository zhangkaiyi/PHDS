using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PHDS.Identity.BLL;
using PHDS.Web.Models;
using PHDS.Identity.DAL;
using System.ComponentModel.DataAnnotations;

namespace PHDS.Web.Controllers
{
    [Authorize(Roles = "管理员")]
    public class RolesManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public RolesManageController()
        {
        }

        public RolesManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Account/Register
        public ActionResult Roles()
        {
            return View(RoleManager.Roles.ToList());
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateRole(string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result
                   = await RoleManager.CreateAsync(new ApplicationRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(name as object);
        }

        //
        // GET: /Account/Register
        public ActionResult EditRole(string Id)
        {
            if (Id == null)
            {
                return View("Error");
            }
            return View(RoleManager.FindById(Id));
        }

        [HttpPost]
        public async Task<ActionResult> EditRole(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(userId,
                       model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                return RedirectToAction("Roles");
            }
            return View("Error", new string[] { "Role Not Found" });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteRole(string id)
        {
            ApplicationRole role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "Role Not Found" });
            }
        }


        //
        // GET: /Account/Register
        public ActionResult Users()
        {
            return View(UserManager.Users.ToList());
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result
                   = await RoleManager.CreateAsync(new ApplicationRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(name as object);
        }

        //
        // GET: /Account/Register
        public ActionResult EditUser(string Id)
        {
            if (Id == null)
            {
                return View("Error");
            }
            return View(UserManager.FindById(Id));
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(UserModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await UserManager.AddToRoleAsync(model.UserId, RoleManager.FindById(userId).Name);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await UserManager.RemoveFromRoleAsync(model.UserId, RoleManager.FindById(userId).Name);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                return RedirectToAction("Users");
            }
            return View("Error", new string[] { "User Not Found" });
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User Not Found" });
            }
        }


        public class RoleModificationModel
        {
            public string RoleName { get; set; }
            public string[] IdsToAdd { get; set; }
            public string[] IdsToDelete { get; set; }
        }

        public class UserModificationModel
        {
            public string UserId { get; set; }
            public string[] IdsToAdd { get; set; }
            public string[] IdsToDelete { get; set; }
        }
    }
}