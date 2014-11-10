using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_DATABASE.Models;

namespace MVC_DATABASE.Controllers
{
    public class CONTRACTsController : Controller
    {
        private EVICEntities db = new EVICEntities();
        
        // GET: CONTRACTs
        public async Task<ActionResult> Index()
        {
            var cONTRACTs = db.CONTRACTs.Include(c => c.AspNetUser).Include(c => c.TEMPLATE);
            return View(await cONTRACTs.ToListAsync());
        }

        // GET: CONTRACTs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            if (cONTRACT == null)
            {
                return HttpNotFound();
            }
            return View(cONTRACT);
        }

        // GET: CONTRACTs/Create
        public ActionResult Create()
        {
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE");
            return View();
        }

        // POST: CONTRACTs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CONTRACTID,Id,TEMPLATEID,RFPID,CONTRACT_PATH")] CONTRACT cONTRACT)
        {
            if (ModelState.IsValid)
            {
                db.CONTRACTs.Add(cONTRACT);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", cONTRACT.Id);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", cONTRACT.TEMPLATEID);
            return View(cONTRACT);
        }

        // GET: CONTRACTs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            if (cONTRACT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", cONTRACT.Id);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", cONTRACT.TEMPLATEID);
            return View(cONTRACT);
        }

        // POST: CONTRACTs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CONTRACTID,Id,TEMPLATEID,RFPID,CONTRACT_PATH")] CONTRACT cONTRACT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cONTRACT).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.AspNetUsers, "Id", "Email", cONTRACT.Id);
            ViewBag.TEMPLATEID = new SelectList(db.TEMPLATEs, "TEMPLATEID", "TYPE", cONTRACT.TEMPLATEID);
            return View(cONTRACT);
        }

        // GET: CONTRACTs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            if (cONTRACT == null)
            {
                return HttpNotFound();
            }
            return View(cONTRACT);
        }

        // POST: CONTRACTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CONTRACT cONTRACT = await db.CONTRACTs.FindAsync(id);
            db.CONTRACTs.Remove(cONTRACT);
            await db.SaveChangesAsync();
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
