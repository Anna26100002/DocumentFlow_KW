using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.Controllers
{
    public class ProfileController : Microsoft.AspNetCore.Mvc.Controller
    {

        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationContext db;
        public ProfileController(ApplicationContext db, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            this.db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: DataController
        public async Task<IActionResult> Index()
        {
            var id = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        // GET: DataController/Details/5
        public Microsoft.AspNetCore.Mvc.ActionResult Details(int id)
        {
            return View();
        }

        // GET: DataController/Create
        public Microsoft.AspNetCore.Mvc.ActionResult Create()
        {
            return View();
        }

        // POST: DataController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Microsoft.AspNetCore.Mvc.ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DataController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var Fio = user.Fio;
            var Position = user.Position;
            var FioPosition = Fio + " (" + Position + ")";

            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                Login = user.Login,
                Year = user.Year,
                Fio = user.Fio,
                Position = user.Position,
                Chief = user.Chief,
                FioUsers = db.Users.ToList(),
                FioPosition = FioPosition
            };
            return View(model);
        }

        // POST: DataController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Login = model.Login;
                    user.UserName = model.Login;
                    user.Year = model.Year;
                    user.Fio = model.Fio;
                    user.Position = model.Position;
                    user.Chief = model.Chief;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Login = user.Login };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result =
                await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }

        // GET: DataController/Delete/5
        public Microsoft.AspNetCore.Mvc.ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DataController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Microsoft.AspNetCore.Mvc.ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
