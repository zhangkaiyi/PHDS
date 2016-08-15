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
using System.ComponentModel;

namespace PHDS.Web.Controllers
{
    public class RolesManageController : BaseController
    {
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

        public RolesManageController(ApplicationUserManager userManager, ApplicationRoleManager roleManager) : base(userManager, roleManager)
        {

        }

        
        [Description("角色列表")]
        public ActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }

        [Description("创建角色")]
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Description("创建角色，保存")]
        public async Task<ActionResult> CreateRole(string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result
                   = await _roleManager.CreateAsync(new ApplicationRole(name));
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
        [Description("编辑角色")]
        public ActionResult EditRole(string Id)
        {
            if (Id == null)
            {
                return View("Error");
            }
            return View(_roleManager.FindById(Id));
        }

        [HttpPost]
        [Description("编辑角色，保存")]
        public async Task<ActionResult> EditRole(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await _userManager.AddToRoleAsync(userId, model.RoleName);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await _userManager.RemoveFromRoleAsync(userId,
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
        [Description("删除角色，保存")]
        public async Task<ActionResult> DeleteRole(string id)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
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
        [Description("用户列表")]
        public ActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        [Description("创建用户")]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Description("创建用户，保存")]
        public async Task<ActionResult> CreateUser(string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result
                   = await _roleManager.CreateAsync(new ApplicationRole(name));
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
        [Description("编辑用户")]
        public ActionResult EditUser(string Id)
        {
            if (Id == null)
            {
                return View("Error");
            }
            return View(_userManager.FindById(Id));
        }

        [HttpPost]
        [Description("编辑用户，保存")]
        public async Task<ActionResult> EditUser(UserModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    result = await _userManager.AddToRoleAsync(model.UserId, _roleManager.FindById(userId).Name);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    result = await _userManager.RemoveFromRoleAsync(model.UserId, _roleManager.FindById(userId).Name);
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
        [Description("删除用户，保存")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
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