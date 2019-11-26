using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class ToDoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ToDoes
        public ActionResult Index()
        {
            return View();
           
        }

        private IEnumerable<ToDo> GetToDos() {
            string currentUserId = User.Identity.GetUserId();                //to get user id and show list made by that user only

            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            IEnumerable<ToDo> myToDoes = db.ToDos.ToList().Where(x => x.User == currentUser);

            int completeCount = 0;
            foreach (ToDo toDo in myToDoes) {
                if (toDo.IsDone)
                    completeCount++;
            }

            ViewBag.Percent = Math.Round(100f * ((float)completeCount / (float)myToDoes.Count()));

            return myToDoes;

        }

        public ActionResult BuildToDoTable() {
            //string currentUserId = User.Identity.GetUserId();                //to get user id and show list made by that user only

            //ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            return PartialView("_ToDoTable",GetToDos());
        }


        // GET: ToDoes/Details/5
        public ActionResult Details(int? id)             //?why not make it non nullable at first
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);     //find can return null
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // GET: ToDoes/Create          gives create page
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,IsDone")] ToDo toDo)   //bind is simply defining what attr to post to server
        {
            if (ModelState.IsValid)
            {//the following three lines is written as we dont have User information, we are retrieving the user and then adding finally
                string currentUserId = User.Identity.GetUserId();  //identity gives user id 
                ApplicationUser currentUser = db.Users.FirstOrDefault
                    (x => x.Id == currentUserId);        //looking for user with certain id
                toDo.User = currentUser;
                db.ToDos.Add(toDo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toDo);
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AJAXCreate([Bind(Include = "Id,Description,IsDone")] ToDo toDo)   //bind is simply defining what attr to post to server
        {
            if (ModelState.IsValid)
            {//the following three lines is written as we dont have User information, we are retrieving the user and then adding finally
                string currentUserId = User.Identity.GetUserId();  //identity gives user id 
                ApplicationUser currentUser = db.Users.FirstOrDefault
                    (x => x.Id == currentUserId);        //looking for user with certain id
                toDo.User = currentUser;
                toDo.IsDone = false;
                db.ToDos.Add(toDo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView("_ToDoTable", GetToDos());
        }

        // GET: ToDoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) //if id was not given
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id); //locate the todo with that id and return it with the view
            if (toDo == null)
            {
                return HttpNotFound();
            }
            string currentUserId = User.Identity.GetUserId();  //identity gives user id 
            ApplicationUser currentUser = db.Users.FirstOrDefault
                (x => x.Id == currentUserId);

            if (toDo.User != currentUser)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(toDo);
        }


        [HttpPost]
        public ActionResult AJAXEdit(int? id, bool value)
        {
            if (id == null) //if id was not given
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id); //locate the todo with that id and return it with the view
            if (toDo == null)
            {
                return HttpNotFound();
            }
            else
            {
                toDo.IsDone = value;
                db.Entry(toDo).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_ToDoTable", GetToDos());


            }
        }


        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toDo).State = EntityState.Modified;           //saving modified
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)    //if id not given
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDo toDo = db.ToDos.Find(id);
            if (toDo == null)
            {
                return HttpNotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToDo toDo = db.ToDos.Find(id);
            db.ToDos.Remove(toDo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
