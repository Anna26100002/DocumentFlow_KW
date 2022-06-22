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
using System.IO;

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
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Start", "Home");
            }
            
            var Executor = user.Fio;
            var Position = user.Position;
            var ExecutorPosition = Executor + " (" + Position + ")";

            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочена";
                db.Documents.Update(docs);
                db.SaveChanges();
            }
            var notTermTasks = db.Tasks.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var tasts in notTermTasks)
            {
                tasts.Term = "Просрочена";
                db.Tasks.Update(tasts);
                db.SaveChanges();
            }

            var kpi = db.KPI.FirstOrDefault(i => i.user == user.Fio);
            if (kpi == null)
            {
                KPI KPI = new KPI
                {
                    user = user.Fio,
                };
                db.KPI.Add(KPI);
                db.SaveChanges();
                kpi = db.KPI.FirstOrDefault(i => i.user == user.Fio);
            }
            double KPI1 = 0;
            double KPI2 = 0;
            double KPI3 = 0;
            double KPI4 = 0;
            double KPI5 = 0;
            double KPI6 = 0;
            double TasksPriorityInTerm = db.Tasks.Where((p => p.Executor == ExecutorPosition)).Where(r => r.Priority == "Высокий").Where(s => s.Term == "В срок").Count();
            double TasksPriority = db.Tasks.Where((p => p.Executor == ExecutorPosition)).Where(r => r.Priority == "Высокий").Count();
            
            if (TasksPriority != 0)
            {
                KPI1 = Math.Round((TasksPriorityInTerm / TasksPriority) * 100);
            }
            else
            {
                KPI1 = 50;
            }

            double DocumentsPriorityInTerm = db.Documents.Where(p => p.Executor == ExecutorPosition).Where(r => r.Priority == "Высокий").Where(s => s.Term == "В срок").Count();
            double DocumentsPriority = db.Documents.Where((p => p.Executor == ExecutorPosition)).Where(r => r.Priority == "Высокий").Count();
            if (DocumentsPriority != 0)
            {
                KPI2 = Math.Round((DocumentsPriorityInTerm / DocumentsPriority) * 100);
            }
            else
            {
                KPI2 = 50;
            }

            double TasksInTerm = db.Tasks.Where(p => p.Executor == ExecutorPosition).Where(s => s.Term == "В срок").Count();
            double Tasks = db.Tasks.Where(p => p.Executor == ExecutorPosition).Count();
            if (Tasks != 0)
            {
                KPI3 = Math.Round((TasksInTerm / Tasks) * 100);
            }
            else
            {
                KPI3 = 50;
            }

            double DocumentsInTerm = db.Documents.Where(p => p.Executor == ExecutorPosition).Where(s => s.Term == "В срок").Count();
            double Documents = db.Documents.Where(p => p.Executor == ExecutorPosition).Count();
            if (Documents != 0)
            {
                KPI4 = Math.Round((DocumentsInTerm/Documents) * 100);
            }
            else
            {
                KPI4 = 50;
            }

            double TasksNotTerm = db.Tasks.Where(p => p.Executor == ExecutorPosition).Where(s => s.Completed == true).Count();
            if (Tasks != 0)
            {
                KPI5 = Math.Round((TasksNotTerm / Tasks) * 100);
            }
            else
            {
                KPI5 = 50;
            }

            double DocumentsNotTerm = db.Documents.Where(p => p.Executor == ExecutorPosition).Where(s => s.Status != "Не согласован").Count();
            if (Documents != 0)
            {
                KPI6 = Math.Round((DocumentsNotTerm / Documents) * 100);
            }
            else
            {
                KPI6 = 50;
            }
            double KPIgeneral = Math.Round(0.1 * (2.5 * KPI1 + 2.5 * KPI2 + 1.5 * KPI3 + 1.5 * KPI4 + KPI5 + KPI6));

                kpi.TasksPriorityInTerm = Convert.ToInt32(TasksPriorityInTerm);
                kpi.TasksPriority = Convert.ToInt32(TasksPriority);
                kpi.KPI1 = KPI1;
                kpi.DocumentsPriorityInTerm = Convert.ToInt32(DocumentsPriorityInTerm);
                kpi.DocumentsPriority = Convert.ToInt32(DocumentsPriority);
                kpi.KPI2 = KPI2;
                kpi.TasksInTerm = Convert.ToInt32(TasksInTerm);
                kpi.KPI3 = KPI3;
                kpi.DocumentsInTerm = Convert.ToInt32(DocumentsInTerm);
                kpi.KPI4 = KPI4;
                kpi.TasksNotTerm = Convert.ToInt32(TasksNotTerm);
                kpi.KPI5 = KPI5;
                kpi.DocumentsNotTerm = Convert.ToInt32(DocumentsNotTerm);
                kpi.KPI6 = KPI6;
                kpi.Tasks = Convert.ToInt32(Tasks);
                kpi.Documents = Convert.ToInt32(Documents);
                kpi.KPIgeneral = Convert.ToInt32(KPIgeneral);

                user.KPI = kpi.KPIgeneral;
                db.Attach(user).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!db.Users.Any(e => e.Id == user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            db.Attach(kpi).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!db.KPI.Any(e => e.user == Executor))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var tasks = db.Tasks.FromSqlRaw("SELECT * FROM Tasks WHERE Executor = {0} ORDER BY Id DESC", ExecutorPosition).ToList();
            var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", ExecutorPosition).ToList();
            TasksDocuments tasksDocuments = new TasksDocuments()
            {
                Tasks = tasks,
                Documents = documents,
                Kpi = kpi,
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

                    if (planInterval < doingInterval)
                    {
                        task.KPI = Convert.ToString(doingInterval.Subtract(planInterval));
                        task.Term = "Просрочена";
                    }
                    else
                    {
                        task.Term = "В срок";
                    }

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
            }                      
            return View(task);
        }

        public async Task<IActionResult> DetailsDocumentAsync(int id)
        {
            //получаем id текущего пользователя
            var userId = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(userId);
            string userName = user.Fio;

            Document document = await db.Documents.FindAsync(id);
            string numberId = Convert.ToString(id) + "_";
            string dirName = "C:\\4Kurs_2Sem\\Project\\DocumentFlow_KW\\DocumentFlow_KW\\wwwroot\\Files";
            document.UserName = userName;
            if (document.FileName != null)
            {
                // если папка существует
                if (Directory.Exists(dirName) == true)
                {
                    string[] files = Directory.GetFiles(dirName);
                    foreach (string file in files)
                    {
                        if (file.Contains(numberId) == true)
                        {
                            document.FileName = Path.GetFileName(file).Remove(0, document.Count.Value);
                        }
                    }
                }
            }
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
        public IActionResult DetailsDocument(ViewModels.Document model)
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
                    if (planInterval < doingInterval)
                    {
                        document.KPI = Convert.ToString(doingInterval.Subtract(planInterval));
                        document.Term = "Просрочен";
                    }
                    else
                    {
                        document.Term = "В срок";
                    }

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
                string numberId = Convert.ToString(Id) + "_";
                string dirName = "C:\\4Kurs_2Sem\\Project\\DocumentFlow_KW\\DocumentFlow_KW\\wwwroot\\Files";
                if (document.FileName != null)
                {
                    // если папка существует
                    if (Directory.Exists(dirName) == true)
                    {
                        string[] files = Directory.GetFiles(dirName);
                        foreach (string file in files)
                        {
                            if (file.Contains(numberId) == true)
                            {
                                document.FileName = Path.GetFileName(file).Remove(0, document.Count.Value);
                            }
                        }
                    }
                }

            }
            return View(document);
        }

        public async Task<IActionResult> ReasonDeniedDocument(int Id)
        {
            var document = await db.Documents.FindAsync(Id);
            string numberId = Convert.ToString(Id) + "_";
            string dirName = "C:\\4Kurs_2Sem\\Project\\DocumentFlow_KW\\DocumentFlow_KW\\wwwroot\\Files";
            if (document.FileName != null)
            {
                // если папка существует
                if (Directory.Exists(dirName) == true)
                {
                    string[] files = Directory.GetFiles(dirName);
                    foreach (string file in files)
                    {
                        if (file.Contains(numberId) == true)
                        {
                            document.FileName = Path.GetFileName(file).Remove(0, document.Count.Value);
                        }
                    }
                }
            }
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
                string numberId = Convert.ToString(model.Id) + "_";
                string dirName = "C:\\4Kurs_2Sem\\Project\\DocumentFlow_KW\\DocumentFlow_KW\\wwwroot\\Files";
                if (document.FileName != null)
                {
                    // если папка существует
                    if (Directory.Exists(dirName) == true)
                    {
                        string[] files = Directory.GetFiles(dirName);
                        foreach (string file in files)
                        {
                            if (file.Contains(numberId) == true)
                            {
                                document.FileName = Path.GetFileName(file).Remove(0, document.Count.Value);
                            }
                        }
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
                if (document.TimeCompleted == null)
                {
                    TimeSpan doingInterval = DateTime.Now.Subtract(document.CreationDate);
                    TimeSpan planInterval = document.EndDate.Subtract(document.CreationDate);

                    document.TimeCompleted = Convert.ToString(doingInterval);
                    if (planInterval < doingInterval)
                    {
                        document.KPI = Convert.ToString(doingInterval.Subtract(planInterval));
                        document.Term = "Просрочен";
                    }
                    else
                    {
                        document.Term = "В срок";
                    }
                    
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

            }
            string numberId = Convert.ToString(Id) + "_";
            string dirName = "C:\\4Kurs_2Sem\\Project\\DocumentFlow_KW\\DocumentFlow_KW\\wwwroot\\Files";
            if (document.FileName != null)
            {
                // если папка существует
                if (Directory.Exists(dirName) == true)
                {
                    string[] files = Directory.GetFiles(dirName);
                    foreach (string file in files)
                    {
                        if (file.Contains(numberId) == true)
                        {
                            document.FileName = Path.GetFileName(file).Remove(0, document.Count.Value);
                        }
                    }
                }
            }
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
