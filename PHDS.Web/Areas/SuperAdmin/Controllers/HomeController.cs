using Microsoft.AspNet.Identity;
using PHDS.Identity.DAL;
using PHDS.Web.Areas.SuperAdmin.Models;
using PHDS.Web.Controllers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PHDS.Web.Models;

namespace PHDS.Web.Areas.SuperAdmin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: SuperAdmin/Home
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Register
        public ActionResult RegisterDuiZhangYuan()
        {
            using (var db = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var result = from p in db.往来单位
                             orderby p.RANK descending
                             select new SelectListItem
                             {
                                 Text = p.单位名称,
                                 Value = p.单位编号
                             };
                ViewBag.Affiliation = result.ToList();
            }
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterDuiZhangYuan(RegisterDuiZhangYuanViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Password = model.Password, Affiliation = model.Affiliation };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var rolesForUser = _userManager.GetRoles(user.Id);
                    if (!rolesForUser.Contains("对账员"))
                    {
                        _userManager.AddToRole(user.Id, "对账员");
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                    // 发送包含此链接的电子邮件
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            using (var db = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var result = from p in db.往来单位
                             orderby p.RANK descending
                             select new SelectListItem
                             {
                                 Text = p.单位名称,
                                 Value = p.单位编号
                             };
                ViewBag.Affiliation = result.ToList();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/Register
        public ActionResult RegisterGuanLiYuan()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterGuanLiYuan(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Password = model.Password };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var rolesForUser = _userManager.GetRoles(user.Id);
                    if (!rolesForUser.Contains("管理员"))
                    {
                        _userManager.AddToRole(user.Id, "管理员");
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                    // 发送包含此链接的电子邮件
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}