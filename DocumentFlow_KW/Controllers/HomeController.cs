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
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace DocumentFlow_KW.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
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
            var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0}", ExecutorPosition).ToList();
            TasksDocuments tasksDocuments = new TasksDocuments()
            {
                Tasks = tasks,
                Documents = documents,
            };

            return View(tasksDocuments);
        }

        
        public async Task<IActionResult> DetailsTaskAsync(int id)
        {
            ViewModels.Task task = await db.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            if (task.Completed == false)
            {
                return View(task);
            }
            else
            {
                return RedirectToAction("CompletedTask", "Home", task);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DetailsTaskAsync(ViewModels.Task model)
        {
            return View(model);
        }

        public async Task<IActionResult> CompletedTask(int Id)
        {
            var task = await db.Tasks.FindAsync(Id);
            if (task != null)
            {
                task.Completed = true;
                if (task.TimeCompleted == null)
                {
                    TimeSpan doingInterval = DateTime.Now.Subtract(task.CreationDate);
                    TimeSpan planInterval = task.EndDate.Subtract(task.CreationDate);

                    task.TimeCompleted = Convert.ToString(doingInterval);
                    //double minutes = TimeSpan.Parse(document.TimeCompleted).TotalMinutes - TimeSpan.Parse(document.EndDate.ToString("dd.MM.yyyy ")).TotalMinutes;
                    //if (minutes > 0)
                    if (planInterval < doingInterval)
                    {
                        //document.KPI = Convert.ToString(TimeSpan.Parse(document.TimeCompleted) - TimeSpan.Parse(Convert.ToString(document.EndDate)));
                        task.KPI = Convert.ToString(doingInterval.Subtract(planInterval));
                    }
                    //document.KPI = document.

                    db.Attach(task).State = EntityState.Modified;
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        if (!db.Tasks.Any(e => e.Id == task.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                //var result = await db.Update(document);
                
            }                      
            //document.Completed = true;
            //return RedirectToAction("Details", document1);
            return View(task);
        }

        public async Task<IActionResult> DetailsDocumentAsync(int id)
        {
            Document document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            if (document.Status == "Не согласован")
            {
                return View(document);
            }
            if (document.Status == "Отказано")
            {
                return RedirectToAction("DeniedDocument", "Home", document);
            }
            else
            {
                return RedirectToAction("CompletedDocument", "Home", document);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DetailsDocumentAsync(ViewModels.Document model)
        {
            return View(model);
        }

        public async Task<IActionResult> CompletedDocument(int Id)
        {
            var document = await db.Documents.FindAsync(Id);
            if (document != null)
            {
                document.Status = "Согласован";
                if (document.TimeCompleted == null)
                {
                    TimeSpan doingInterval = DateTime.Now.Subtract(document.CreationDate);
                    TimeSpan planInterval = document.EndDate.Subtract(document.CreationDate);

                    document.TimeCompleted = Convert.ToString(doingInterval);
                    //double minutes = TimeSpan.Parse(document.TimeCompleted).TotalMinutes - TimeSpan.Parse(document.EndDate.ToString("dd.MM.yyyy ")).TotalMinutes;
                    //if (minutes > 0)
                    if (planInterval < doingInterval)
                    {
                        //document.KPI = Convert.ToString(TimeSpan.Parse(document.TimeCompleted) - TimeSpan.Parse(Convert.ToString(document.EndDate)));
                        document.KPI = Convert.ToString(doingInterval.Subtract(planInterval));
                    }
                    //document.KPI = document.

                    db.Attach(document).State = EntityState.Modified;
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        if (!db.Tasks.Any(e => e.Id == document.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                //var result = await db.Update(document);

            }
            //document.Completed = true;
            //return RedirectToAction("Details", document1);
            return View(document);
        }

        public async Task<IActionResult> ReasonDeniedDocument(int Id)
        {
            var document = await db.Documents.FindAsync(Id);
            if (document != null)
            {
               
                return View(document);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReasonDeniedDocument (Document model)
        {
            var document = await db.Documents.FindAsync(model.Id);
            if (document != null)
            {
                document.Comment = model.Comment;
                db.Attach(document).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!db.Tasks.Any(e => e.Id == document.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("DeniedDocument", "Home", document);
            }
            else
            {
                return NotFound();
            }
        }


        public async Task<IActionResult> DeniedDocument(int Id)
        {
            var document = await db.Documents.FindAsync(Id);
            if (document != null)
            {
                document.Status = "Отказано";
                //document.Comment = Comment;
                if (document.TimeCompleted == null)
                {
                    TimeSpan doingInterval = DateTime.Now.Subtract(document.CreationDate);
                    TimeSpan planInterval = document.EndDate.Subtract(document.CreationDate);

                    document.TimeCompleted = Convert.ToString(doingInterval);
                    //double minutes = TimeSpan.Parse(document.TimeCompleted).TotalMinutes - TimeSpan.Parse(document.EndDate.ToString("dd.MM.yyyy ")).TotalMinutes;
                    //if (minutes > 0)
                    if (planInterval < doingInterval)
                    {
                        //document.KPI = Convert.ToString(TimeSpan.Parse(document.TimeCompleted) - TimeSpan.Parse(Convert.ToString(document.EndDate)));
                        document.KPI = Convert.ToString(doingInterval.Subtract(planInterval));
                    }
                    //document.KPI = document.
                    
                    db.Attach(document).State = EntityState.Modified;
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        if (!db.Tasks.Any(e => e.Id == document.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                //var result = await db.Update(document);

            }
            //document.Completed = true;
            //return RedirectToAction("Details", document1);
            return View(document);
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
