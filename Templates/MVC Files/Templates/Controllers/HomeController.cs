using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using %(ProjectName)%.Models;

namespace %(ProjectName)%.Controllers
{
    public class HomeController : Controller
    {
        private %(SQLDatabaseName)%Entities db = new %(SQLDatabaseName)%Entities();

        // GET: /Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Home/Index
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include="%(Page1Columns)%")] %(ProjectName)%.Models.%(ProjectName)% submission)
        {
            if (ModelState.IsValid)
            {
                db.%(SQLMainTableName)%.Add(submission);
                db.SaveChanges();
                return RedirectToAction("Confirmation");
            }

            return View(submission);
        }

        // GET: /Home/Confirmation
        public ActionResult Confirmation()
        {
            return View();
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
