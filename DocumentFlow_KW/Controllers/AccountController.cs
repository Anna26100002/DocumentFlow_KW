using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.Controllers
{
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext db;
        public AccountController(ApplicationContext db, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.db = db;
            _userManager = userManager; //Для управления пользователя
            _signInManager = signInManager; //Для его аутентификации, установки и удаления его куки
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Login = model.Login, UserName = model.Login, Year = model.Year, Fio = model.Fio, Position = model.Position};

                // добавляем пользователя в БД
                var result = await _userManager.CreateAsync(user, model.Password);
                KPI KPI = new KPI
                {
                    user = user.Fio,
                };
                db.KPI.Add(KPI);
                db.SaveChanges();
                if (result.Succeeded)
                {
                    // установка аутентифицированных куки для добавления пользователя
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home"); //Выполняем переадресацию на главную страницу
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);//Добавляем к состоянию модели
                                                                                  //все возникшие при добавлении ошибки
                    }
                }
            }
            return View(model);
        }

        //получаем адрес для возврата в виде параметра returnUrl и передаем его в модель LoginViewModel
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        //получаем данные из представления в виде модели LoginViewModel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Аутентификация пользователя, (не сохраняет куки на долгое время)
                var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        //Возвращаем пользователя на предыдущее место
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        //Возвращаем на главную страницу
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        //Выход пользователя из приложения
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
