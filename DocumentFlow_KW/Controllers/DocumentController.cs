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
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

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

        //public JsonResult Upload()
        //{
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        HttpPostedFileBase file = Request.Files[i]; //Uploaded file
        //                                                    //Use the following properties to get file name, size and MIMEType
        //        int fileSize = file.ContentLength;
        //        string fileName = file.FileName;
        //        string mimeType = file.ContentType;
        //        System.IO.Stream fileContent = file.InputStream;
        //        //To save file, use SaveAs method
        //        file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
        //    }
        //    return Json("Uploaded " + Request.Files.Count + " files");
        //}
        [HttpPost]
        public IActionResult UploadFiles(IFormFile file)
        {
            long size = 0;
            var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            filename = _appEnvironment.WebRootPath + $@"\{filename}";
            size += file.Length;
            using (FileStream fs = System.IO.File.Create(filename))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            return View();
        }

        // GET: DocumentController/Success/5
        [HttpGet]
        public ActionResult Success(Document model) //Отображение документа
        {
            var notTermDocs = db.Documents.Where(p => p.EndDate < DateTime.Now).Where(r => r.TimeCompleted == null).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }
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
            //добавляем документ в БД
            db.Documents.Add(document);
            await db.SaveChangesAsync();
            
            
            long size = 0;
            if (model.DocumentViewModel.FileData != null)
            {
                int Id = document.Id;
                int count = Convert.ToString(document.Id).Length;
                document.Count = count + 1;
                var filename = ContentDispositionHeaderValue.Parse(model.DocumentViewModel.FileData.ContentDisposition).FileName.Trim('"');
                filename = _appEnvironment.WebRootPath + $@"\Files\{Id + "_" + filename}";
                size += model.DocumentViewModel.FileData.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    model.DocumentViewModel.FileData.CopyTo(fs);
                    fs.Flush();
                }
                string filename2 = Id + "_" + filename;
                document.FileName = filename2;
                db.Documents.Update(document);
                db.SaveChanges();
            }
            //ViewBag.Message = $"{files.Count} file(s) / 
            //          { size}
            //bytes uploaded successfully!";
            //if (model.DocumentViewModel.FileData != null)
            //{
            //    byte[] dat = null;
            //    //считываем переданный файл в массив байтов
            //    using (BinaryReader binaryReader = new BinaryReader(model.DocumentViewModel.FileData.OpenReadStream()))
            //    {
            //        dat = binaryReader.ReadBytes((int)model.DocumentViewModel.FileData.Length);
            //    }
            //    //установка массива байтов
            //    document.FileData = dat;
            //    document.Type = "Ok";
            //}

            //IFormFile uploadedFile = model.DocumentViewModel.FileData;
            //if (model.DocumentViewModel.FileData != null)
            //{
            //    string path = "/Files/" + model.DocumentViewModel.FileData.FileName;
            //    //сохраняем файл в папку Files в каталоге wwwroot
            //    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
            //    {
            //        await uploadedFile.CopyToAsync(fileStream);
            //    }
            //    DocumentViewModel file = new DocumentViewModel { FileName = uploadedFile.FileName, FilePath = path };
            //    db.Files.Add(file);
            //    db.SaveChanges();
            //}


            return RedirectToAction("Success", "Document", document);
        }

        [HttpGet]
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

            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }
            var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", ExecutorPosition).ToList();

            DocumentsViewModel documentsView = new DocumentsViewModel
            {
                Documents = documents,
                Count = documents.Count,
            };

            return View(documentsView);
        }

        [HttpPost]
        public async Task<IActionResult> IncomingAsync(DocumentsViewModel model)
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

            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }
            //var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", ExecutorPosition).ToList();

            DocumentsViewModel documentsView = new DocumentsViewModel
            {
                //Documents = model.Documents,
            };
            if (model.Search != null && model.Search2 != null)
            {
                documentsView.Search = model.Search.ToLower();
                documentsView.Search2 = model.Search2.ToLower();
                var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(t => t.Fio.ToLower().Contains(documentsView.Search2)).Where(c => c.Executor == ExecutorPosition).OrderByDescending(Id => Id.Id).ToList();
                documentsView.Documents = selectedDocument;
                documentsView.Count = selectedDocument.Count;
            }
            else if (model.Search != null)
            {
                documentsView.Search = model.Search.ToLower();
                var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(c => c.Executor == ExecutorPosition).OrderByDescending(Id => Id.Id).ToList();
                documentsView.Documents = selectedDocument;
                documentsView.Count = selectedDocument.Count;
            }
            else if (model.Search2 != null)
            {
                documentsView.Search2 = model.Search2.ToLower();
                var selectedDocument = db.Documents.Where(t => t.Fio.ToLower().Contains(documentsView.Search2)).Where(c => c.Executor == ExecutorPosition).OrderByDescending(Id => Id.Id).ToList();
                documentsView.Documents = selectedDocument;
                documentsView.Count = selectedDocument.Count;
            }
            else
            {
                return RedirectToAction("Incoming", "Document");
            }
            documentsView.Search = "";
            documentsView.Search2 = "";
            return View(documentsView);

            //var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(c => c.Executor == ExecutorPosition).OrderByDescending(Id => Id.Id).ToList();
        }

        [HttpGet]
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

            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }

            var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Fio = {0} ORDER BY Id DESC", AuthorPosition).ToList();
            DocumentsViewModel documentsView = new DocumentsViewModel
            {
                Documents = documents,
                Count = documents.Count,
            };

            return View(documentsView);
        }

        [HttpPost]
        public async Task<IActionResult> OutgoingAsync(DocumentsViewModel model)
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

            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }

            //var documents = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Fio = {0} ORDER BY Id DESC", AuthorPosition).ToList();
            DocumentsViewModel documentsView = new DocumentsViewModel
            {
                //Documents = model.Documents,
            };
            if (model.Search != null && model.Search2 != null)
            {
                documentsView.Search = model.Search.ToLower();
                documentsView.Search2 = model.Search2.ToLower();
                var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(t => t.Executor.ToLower().Contains(documentsView.Search2)).Where(c => c.Fio == AuthorPosition).OrderByDescending(Id => Id.Id).ToList();
                documentsView.Documents = selectedDocument;
                documentsView.Count = selectedDocument.Count;
            }
            else if (model.Search != null)
            {
                documentsView.Search = model.Search.ToLower();
                var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(c => c.Fio == AuthorPosition).OrderByDescending(Id => Id.Id).ToList();
                documentsView.Documents = selectedDocument;
                documentsView.Count = selectedDocument.Count;
            }
            else if (model.Search2 != null)
            {
                documentsView.Search2 = model.Search2.ToLower();
                var selectedDocument = db.Documents.Where(t => t.Executor.ToLower().Contains(documentsView.Search2)).Where(c => c.Fio == AuthorPosition).OrderByDescending(Id => Id.Id).ToList();
                documentsView.Documents = selectedDocument;
                documentsView.Count = selectedDocument.Count;
            }
            else
            {
                return RedirectToAction("Outgoing", "Document");
            }
            documentsView.Search = "";
            documentsView.Search2 = "";
            return View(documentsView);
            //var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(c => c.Fio == AuthorPosition).OrderByDescending(Id => Id.Id).ToList();

        }

        [HttpGet]
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
            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }
            foreach (var employee in employees)
            {
                var Fio = employee.Fio;
                var Position2 = employee.Position;
                var FioPosition2 = Fio + " (" + Position2 + ")";

                document = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", FioPosition2).ToList();
                documents.AddRange(document);
            }
            DocumentsViewModel documentsView = new DocumentsViewModel
            {
                Documents = documents,
                Count = documents.Count,
            };

            return View(documentsView);
        }
        // GET: DocumentController/Edit/5

        [HttpPost]
        public async Task<IActionResult> EmployeesAsync(DocumentsViewModel model)
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
            List<Document> selectedDocuments = new List<Document>();
            List<Document> selectedDocument = new List<Document>();
            List<Document> document = new List<Document>();
            var notTermDocs = db.Documents.Where(r => r.TimeCompleted == null).Where(p => p.EndDate < DateTime.Now).ToList();
            foreach (var docs in notTermDocs)
            {
                docs.Term = "Просрочен";
                db.Documents.Update(docs);
                db.SaveChanges();
            }
            DocumentsViewModel documentsView = new DocumentsViewModel
            {
                //Documents = model.Documents,
            };
            if (model.Search != null && model.Search2 != null)
            {
                documentsView.Search = model.Search.ToLower();
                documentsView.Search2 = model.Search2.ToLower();
                foreach (var employee in employees)
                {
                    var Fio = employee.Fio;
                    var Position2 = employee.Position;
                    var FioPosition2 = Fio + " (" + Position2 + ")";

                    //var selectedDocument = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", FioPosition2).ToList();
                    selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(t => t.Executor.ToLower().Contains(documentsView.Search2)).Where(c => c.Executor == FioPosition2).OrderByDescending(Id => Id.Id).ToList();
                    selectedDocuments.AddRange(selectedDocument);
                }
            }
            else if (model.Search != null)
            {
                documentsView.Search = model.Search.ToLower();
                foreach (var employee in employees)
                {
                    var Fio = employee.Fio;
                    var Position2 = employee.Position;
                    var FioPosition2 = Fio + " (" + Position2 + ")";

                    //var selectedDocument = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", FioPosition2).ToList();
                    selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(c => c.Executor == FioPosition2).OrderByDescending(Id => Id.Id).ToList();
                    selectedDocuments.AddRange(selectedDocument);
                }
            }
            else if (model.Search2 != null)
            {
                documentsView.Search2 = model.Search2.ToLower();
                foreach (var employee in employees)
                {
                    var Fio = employee.Fio;
                    var Position2 = employee.Position;
                    var FioPosition2 = Fio + " (" + Position2 + ")";

                    //var selectedDocument = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", FioPosition2).ToList();
                    selectedDocument = db.Documents.Where(t => t.Executor.ToLower().Contains(documentsView.Search2)).Where(c => c.Executor == FioPosition2).OrderByDescending(Id => Id.Id).ToList();
                    selectedDocuments.AddRange(selectedDocument);
                }
            }
            else
            {
                return RedirectToAction("Employees", "Document");
            }

            //foreach (var employee in employees)
            //{
            //    var Fio = employee.Fio;
            //    var Position2 = employee.Position;
            //    var FioPosition2 = Fio + " (" + Position2 + ")";

            //    //var selectedDocument = db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE Executor = {0} ORDER BY Id DESC", FioPosition2).ToList();
            //    var selectedDocument = db.Documents.Where(t => t.Topic.ToLower().Contains(documentsView.Search)).Where(c => c.Executor == FioPosition2).OrderByDescending(Id => Id.Id).ToList();
            //    selectedDocuments.AddRange(selectedDocument);
            //}
            documentsView.Documents = selectedDocuments;
            documentsView.Search = "";
            documentsView.Search2 = "";
            documentsView.Count = selectedDocuments.Count;
            return View(documentsView);
        }
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
