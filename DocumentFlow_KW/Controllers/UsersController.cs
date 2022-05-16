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
    public class UsersController : Microsoft.AspNetCore.Mvc.Controller
    {

        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationContext db;

        public UsersController(ApplicationContext db, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            this.db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: UsersController

        //[HttpPost]
        public IActionResult Index()
        {
            return RedirectToAction("IsAdmin");
 
        }

        //[HttpPost]
        public async Task<IActionResult> IsAdmin()
        {
            // получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();

                if(userRoles.Contains("admin"))
                {
                    return View(_userManager.Users.ToList());
                }
                else
                {
                    return RedirectToAction("NotAdmin");
                }
            }
            return NotFound();
            //return RedirectToAction("NotAdmin");
        }

        //[HttpGet]
        public IActionResult NotAdmin()
        {
            return View(_userManager.Users.ToList());
        }



            // GET: UsersController/Create
            public Microsoft.AspNetCore.Mvc.ActionResult Create() => View();

        // POST: UsersController/Create
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Login = model.Login, UserName = model.Login, Year = model.Year, Fio = model.Fio, Position = model.Position };
                var result = await _userManager.CreateAsync(user, model.Password);
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
            return View(model);
        }

        // GET: UsersController/Edit/5
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

            EditUserViewModel model = new EditUserViewModel {
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

        // POST: UsersController/Edit/5
        [HttpPost]
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


        // POST: UsersController/Delete/5
        [HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
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
    }
}
