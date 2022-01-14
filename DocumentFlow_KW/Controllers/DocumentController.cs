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
    public class DocumentController : Controller
    {
        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        //private ApplicationContext db = new ApplicationContext();
        private readonly ApplicationContext db;
        public DocumentController(ApplicationContext db, UserManager<User> userManager) //Внедрение зависимостей для внедрения контекста                                                                                   
        {                                                                               //БД в контроллер
            this.db = db;
            _userManager = userManager;
        }


        // GET: DocumentController
        public async Task<ActionResult> IndexAsync()
        {
            //var model = new DocumentUsers();
            //model.Users = db.Users.ToList();

            //// получаем id текущего пользователя
            //var id = _userManager.GetUserId(User);
            //User user = await _userManager.FindByIdAsync(id);
            //var Fio = user.Fio;
            //model.Fio = Fio;
            ////model = user.Fio;
            return View();
        }


        // GET: DocumentController/Details/5
        [HttpPost]
        public async Task<ActionResult> DetailsAsync(DocumentUsers model) //Отображение задачи
        //public ActionResult Details() //Отображение задачи
        {
            Document document = await db.Documents.FindAsync(model.Documents.Id);
            if (document == null)
            {
                return NotFound();
            }
            DocumentUsers documentUsers = new DocumentUsers { Documents = document, User = document.User, Fio = document.Executor };
            return View(model);
            //return View();
        }

        // GET: DocumentController/Create
        public async Task<ActionResult> CreateAsync()
        {
            var model = new DocumentUsers
            {
                Users = db.Users.ToList()
            };

            // получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user2 = await _userManager.FindByIdAsync(id);
            var Fio = user2.Fio;
            model.Fio = Fio;
            //model = user.Fio;
            return View(model);
        }

        // POST: DocumentController/Create
        [HttpPost]
        public async Task<ActionResult> CreateAsync(DocumentUsers model)
        {
            //if (ModelState.IsValid)
            //{
                //получаем id текущего пользователя
            var id = _userManager.GetUserId(User);
            User user = await _userManager.FindByIdAsync(id);
            Document document = new Document
            {
                Id = model.Documents.Id,
                Type = model.Documents.Type,
                Topic = model.Documents.Topic,
                CreationDate = model.Documents.CreationDate,
                User = user,
                Executor = model.Documents.Executor,
                Priority = model.Documents.Priority,
                Description = model.Documents.Description,
                Completed = false,
            };
            //добавляем документ в БД
            db.Add(document);
            await db.SaveChangesAsync();
            return View(model);
            //return RedirectToAction("Details", "Document");

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
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: DocumentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DocumentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
