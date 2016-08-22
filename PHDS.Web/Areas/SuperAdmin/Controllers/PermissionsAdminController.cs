﻿using Newtonsoft.Json;
using PHDS.Identity.DAL;
using PHDS.Web.Controllers;
using PHDS.Web.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PHDS.Web.Areas.SuperAdmin.Controllers
{
    public class PermissionsAdminController : BaseController
    {
        [Description("权限列表")]
        public async Task<ActionResult> Index(int index = 1)
        {
            var permissions = await _db.Permissions.ToListAsync();
            //创建ViewModel
            var permissionViews = new List<PermissionViewModel>();
            foreach(var t in permissions)
            {
                var view = new PermissionViewModel
                {
                    Id = t.Id,
                    Action = t.Action,
                    Controller = t.Controller,
                    Description = t.Description,
                };
                permissionViews.Add(view);
            }

            //排序
            permissionViews.Sort(new PermissionViewModelComparer());
            return View(permissionViews);
        }

        // GET: PermissionsAdmin/Details/5
        [Description("权限详情")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            if (applicationPermission == null)
            {
                return HttpNotFound();
            }
            var view = new PermissionViewModel
            {
                Id=applicationPermission.Id,
                Action=applicationPermission.Action,
                Controller=applicationPermission.Controller,
                Description=applicationPermission.Description,
            };
            return View(view);
        }

        // GET: PermissionsAdmin/Create
        [Description("新建权限，列表")]
        //[GridDataSourceAction]
        public ActionResult Create()
        {
            //创建ViewModel
            var permissionViews = new List<PermissionViewModel>();
            //取程序集中权限
            var allPermissions = _permissionsOfAssembly;
            //取数据库已有权限
            var dbPermissions = _db.Permissions.ToList();
            //取两者差集
            var permissions = allPermissions.Except(dbPermissions, new ApplicationPermissionEqualityComparer());
            foreach (var t in permissions)
            {
                var view = new PermissionViewModel
                {
                    Action = t.Action,
                    Controller = t.Controller,
                    Description = t.Description,
                    Id = t.Id,
                };
                permissionViews.Add(view);
            }
            //排序
            permissionViews.Sort(new PermissionViewModelComparer());
            return View(permissionViews);
        }

        [Description("新建权限，保存")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IEnumerable<PermissionViewModel> data)
        {
            if (data == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "参数不能为空");
            }
            foreach (var item in data)
            {
                if (item.Selected)
                {
                    //创建权限
                    var permission = new ApplicationPermission
                    {
                        Id = item.Id,
                        Action = item.Action,
                        Controller = item.Controller,
                        Description = item.Description,
                    };
                    _db.Permissions.Add(permission);
                }
            }
            //保存
            await _db.SaveChangesAsync();
            //方法2，使用Newtonsoft.Json序列化结果对象
            //格式为json字符串，客户端需要解析，即反序列化
            //var result = JsonConvert.SerializeObject(new { Success = true });
            return RedirectToAction("Index");
        }

        //// GET: PermissionsAdmin/Edit/5
        //[Description("编辑权限")]
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ApplicationPermission applicationPermission = _db.Permissions.Find(id);
        //    if (applicationPermission == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    var view = Mapper.Map<PermissionViewModel>(applicationPermission);
        //    return View(view);
        //}

        //// POST: PermissionsAdmin/Edit/5
        //// 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        //// 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        //[Description("编辑权限，保存")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Controller,Action,Description")] PermissionViewModel view)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //对象映射
        //        var model = Mapper.Map<ApplicationPermission>(view);
        //        //修改实体状态
        //        _db.Entry(model).State = EntityState.Modified;
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(view);
        //}

        // GET: PermissionsAdmin/Delete/5
        [Description("删除权限")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            if (applicationPermission == null)
            {
                return HttpNotFound();
            }
            var view = new PermissionViewModel
            {
                Id = applicationPermission.Id,
                Action = applicationPermission.Action,
                Controller = applicationPermission.Controller,
                Description = applicationPermission.Description,
            };
                //Mapper.Map<PermissionViewModel>(applicationPermission);
            return View(view);
        }

        // POST: PermissionsAdmin/Delete/5
        [Description("删除权限，保存")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationPermission applicationPermission = _db.Permissions.Find(id);
            _db.Permissions.Remove(applicationPermission);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
