using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.Controllers
{
    public class TaskController : Microsoft.AspNetCore.Mvc.Controller
    {
        UserManager<User> _userManager;
        //RoleManager<IdentityRole> _roleManager;
        //private ApplicationContext db = new ApplicationContext();
        private readonly ApplicationContext db;
        public TaskController(ApplicationContext db, UserManager<User> userManager) //Внедрение зависимостей для внедрения контекста                                                                                   
        {                                                                               //БД в контроллер
            this.db = db;
            _userManager = userManager;
        }


        // GET: TaskController
        public ActionResult Index()
        {
            //model.Users = db.Users.ToList();

            //// получаем id текущего пользователя
            //var id = _userManager.GetUserId(User);
            //User user = await _userManager.FindByIdAsync(id);
            //var Fio = user.Fio;
            //model.Fio = Fio;
            ////model = user.Fio;
            return View();
        }


        public async Task<ActionResult> SuccessAsync(ViewModels.Task model) //Отображение задачи
        {
            //Document document = await db.Documents.FindAsync(Id);

            //if (document == null)
            //{
            //    return NotFound();
            //}
            var task = db.Tasks.Single(xx => xx.Id == model.Id);

            //return View(model);
            return View(model);
        }

        // GET: TaskController/Create
        [HttpGet]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateAsync()
        {
            // получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            var Fio = user2.Fio;
            var Position = user2.Position;
            var FioPosition = Fio + " (" + Position + ")";
            //Document document = new Document();
            //Document document = new Document
            //{
            //    FioUsers = db.Users.ToList(),
            //    Fio = Fio2,
            //};
            ViewModels.Task task = new ViewModels.Task
            {
                Fio = FioPosition, 
            };
            TaskUsers taskUsers = new TaskUsers
            {
                FioUsers = db.Users.ToList(),
                Task = task,
            };

            //document.Fio = Fio;
            //model = user.Fio;
            //return View(model);
            return View(taskUsers);
        }

        // POST: TaskController/Create
        [HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> CreateAsync(TaskUsers model)
        {
            //if (ModelState.IsValid)
            //{
            //получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(id);
            var Fio = user.Fio;
            var Position = user.Position;
            var FioPosition = Fio + " (" + Position + ")";
            //user.Documents.Add(model.Documents);
            ViewModels.Task task = new ViewModels.Task()
            {
                Id = model.Task.Id,
                Type = model.Task.Type,
                Topic = model.Task.Topic,
                CreationDate = Convert.ToDateTime(DateTime.Now.ToString("dd MM yyyy HH:mm")),
                EndDate = Convert.ToDateTime(model.Task.EndDate.ToString("dd MM yyyy HH:mm")),
                //User = user,
                //Fio = user.Fio,
                Fio = FioPosition,
                Executor = model.Task.Executor,
                Priority = model.Task.Priority,
                Description = model.Task.Description,
                Completed = false,
            };

            //}
            //добавляем документ в БД
            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            //return View(model);
            //return View();
            return RedirectToAction("Success", "Task", task);

        }

        public async Task<IActionResult> IncomingAsync()
        {
            //получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            if (user2 == null)
            {
                return RedirectToAction("Start", "Home");
            }
            var Executor2 = user2.Fio;
            var Position = user2.Position;
            var ExecutorPosition = Executor2 + " (" + Position + ")";

            var tasks = db.Tasks.FromSqlRaw("SELECT * FROM Tasks WHERE Executor = {0}", ExecutorPosition).ToList();

            return View(tasks);
        }

        public async Task<IActionResult> OutgoingAsync()
        {
            //получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            if (user2 == null)
            {
                return RedirectToAction("Start", "Home");
            }
            var Author = user2.Fio;
            var Position = user2.Position;
            var AuthorPosition = Author + " (" + Position + ")";

            var tasks = db.Tasks.FromSqlRaw("SELECT * FROM Tasks WHERE Fio = {0}", AuthorPosition).ToList();
            return View(tasks);
        }

        public async Task<IActionResult> EmployeesAsync()
        {
            //получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            if (user2 == null)
            {
                return RedirectToAction("Start", "Home");
            }
            var Chief = user2.Fio;
            var Position = user2.Position;
            var ChiefPosition = Chief + " (" + Position + ")";
            var employees = db.Users.FromSqlRaw("SELECT * FROM AspNetUsers WHERE Chief = {0}", ChiefPosition).ToList();
            List<ViewModels.Task> tasks = new List<ViewModels.Task>();
            List<ViewModels.Task> task = new List<ViewModels.Task>();
            foreach (var employee in employees)
            {
                var Fio = employee.Fio;
                var Position2 = employee.Position;
                var FioPosition2 = Fio + " (" + Position2 + ")";
                task = db.Tasks.FromSqlRaw("SELECT * FROM Tasks WHERE Executor = {0}", FioPosition2).ToList();
                tasks.AddRange(task);
            }

            return View(tasks);
        }
        // GET: TaskController/Edit/5
        public Microsoft.AspNetCore.Mvc.ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Microsoft.AspNetCore.Mvc.ActionResult Edit(int id, IFormCollection collection)
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


        // POST: TaskController/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var task = db.Tasks.Single(xx => xx.Id == id);
            if (task != null)
            {
                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Outgoing");
        }
    }
}
