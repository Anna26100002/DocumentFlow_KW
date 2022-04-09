using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserManager<User> _userManager;
        private readonly ApplicationContext db;
        public HomeController(ILogger<HomeController> logger, ApplicationContext db, UserManager<User> userManager)
        {
            _logger = logger;
            this.db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var model = new Document
            {
                //Users = db.Users.ToList()
            };

            // получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            //var Executor = user2.Fio;
            //model.Executor = Executor;
            //db.Documents.ToList();
            //model = user.Fio;
            //return View(db.Documents.ToList());
            return View();
        }

        public IActionResult Start()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
