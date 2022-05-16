using DocumentFlow_KW.Models;
using DocumentFlow_KW.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentFlow_KW.Controllers
{
    public class DocumentController : Controller
    {
        UserManager<User> _userManager;
        private readonly ApplicationContext db;
        IWebHostEnvironment _appEnvironment;

        public DocumentController(ApplicationContext db, UserManager<User> userManager, IWebHostEnvironment appEnvironment) //Внедрение зависимостей для внедрения контекста                                                                                   
        {                                                                               //БД в контроллер
            this.db = db;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }


        // GET: DocumentController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DocumentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DocumentController/Success/5
        [HttpGet]
        public ActionResult Success(Document model) //Отображение документа
        {
            //var document = db.Documents.Single(xx => xx.Id == model.Id);
            return View(model);
        }
        // GET: DocumentController/Create
        [HttpGet]
        public async Task<ActionResult> CreateAsync()
        {
            // получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            var Fio = user2.Fio;
            var Position = user2.Position;
            var FioPosition = Fio + " (" + Position + ")";

            DocumentViewModel document = new DocumentViewModel
            {
                Fio = FioPosition,
            };
            DocumentUsersViewModel documentUsers = new DocumentUsersViewModel
            {
                FioUsers = db.Users.ToList(),
                DocumentViewModel = document,
            };
            return View(documentUsers);
        }

        // POST: DocumentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(DocumentUsersViewModel model)
        {
            //получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(id);
            var Fio = user.Fio;
            var Position = user.Position;
            var FioPosition = Fio + " (" + Position + ")";
            //Dictionary<string, string> ExecutorsStatus = new Dictionary<string, string>();
            Document document = new Document()
            {
                Id = model.DocumentViewModel.Id,
                Topic = model.DocumentViewModel.Topic,
                CreationDate = Convert.ToDateTime(DateTime.Now.ToString("dd MM yyyy HH:mm")),
                EndDate = Convert.ToDateTime(model.DocumentViewModel.EndDate.ToString("dd MM yyyy HH:mm")),
                //User = user,
                //Fio = user.Fio,
                Fio = FioPosition,
                Executor = model.DocumentViewModel.Executor,
                //FileData = model.DocumentViewModel.FileData,
                Priority = model.DocumentViewModel.Priority,
                Description = model.DocumentViewModel.Description,
                Status = "Не согласован",
            };
            //if (model.DocumentViewModel.FileData != null)
            //{
            //    byte[] dat = null;
            //    считываем переданный файл в массив байтов
            //    using (BinaryReader binaryReader = new BinaryReader(model.DocumentViewModel.FileData.OpenReadStream()))
            //    {
            //        dat = binaryReader.ReadBytes((int)model.DocumentViewModel.FileData.Length);
            //    }
            //    установка массива байтов
            //    document.FileData = dat;
            //    document.Type = "Ok";
            //}
            IFormFile uploadedFile = model.DocumentViewModel.FileData;
            if (model.DocumentViewModel.FileData != null)
            {
                string path = "/Files/" + model.DocumentViewModel.FileData.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                DocumentViewModel file = new DocumentViewModel { FileName = uploadedFile.FileName, FilePath = path };
                db.Files.Add(file);
                db.SaveChanges();
            }

                //добавляем документ в БД
                db.Documents.Add(document);
            await db.SaveChangesAsync();
            return RedirectToAction("Success", "Document", document);
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

            var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0}", ExecutorPosition).ToList();

            return View(documents);
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

            var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Fio = {0}", AuthorPosition).ToList();
            return View(documents);
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
            List<ViewModels.Document> documents = new List<ViewModels.Document>();
            List<ViewModels.Document> document = new List<ViewModels.Document>();
            foreach (var employee in employees)
            {
                var Fio = employee.Fio;
                var Position2 = employee.Position;
                var FioPosition2 = Fio + " (" + Position2 + ")";
                document = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0}", FioPosition2).ToList();
                documents.AddRange(document);
            }

            return View(documents);
        }
        // GET: DocumentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DocumentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // POST: DocumentController/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var document = db.Documents.Single(xx => xx.Id == id);
            if (document != null)
            {
                db.Documents.Remove(document);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Outgoing");
        }
    }
}
