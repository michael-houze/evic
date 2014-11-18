using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MVC_DATABASE.Models;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Entity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using MVC_DATABASE.Models.ViewModels;
using System.Net;
using System.IO;

namespace MVC_DATABASE.Controllers
{
    public class FAQController : Controller
    {
        private EVICEntities db = new EVICEntities();

        private FAQCreate faqcreate = new FAQCreate();

      
        //
        // GET: /FAQ/
        public ActionResult Index()
        {
            faqcreate.FAQList = new List <FAQ>();
            faqcreate.FAQList = db.FAQs.ToList<FAQ>();

            return View(faqcreate);
        }

        //
        ////GET: /FAQ/EditFaq
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> EditFAQ(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            faqcreate.FAQ = await db.FAQs.FindAsync(id);

            if (faqcreate.FAQ == null)
            {
                return HttpNotFound();
            }
            else
            {
                var editfaq = (from v in db.FAQs
                               where v.Id == id
                               select v).FirstOrDefault();
            }

            return View(faqcreate);
        }

        //
        //POST: /FAQ/EditFAQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Administrator")]
        public async Task<ActionResult>EditFAQ(FAQCreate model)
        {
                FAQ faq = await db.FAQs.FindAsync(model.FAQ.Id);

                var edit = (from v in db.FAQs
                            where v.Id == faq.Id
                            select v).FirstOrDefault();
                if(model.FAQ.QUESTION !=null)
                {
                    edit.QUESTION = model.FAQ.QUESTION;
                }
                if(model.FAQ.ANSWER !=null)
                {
                    edit.ANSWER = model.FAQ.ANSWER;
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
        }

        //
        ////GET: /FAQ/DeleteFaq
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteFAQ (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            faqcreate.FAQ = db.FAQs.Find(id);

            if (faqcreate.FAQ == null)
            {
                return HttpNotFound();
            }
            else
            {
                var editfaq = (from v in db.FAQs
                               where v.Id == id
                               select v).FirstOrDefault();
            }

            return View(faqcreate);
        }

        //
        //POST: /FAQ/DeleteFAQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult>DeleteFAQ(int id)
        {
            FAQ faq = await db.FAQs.FindAsync(id);

            var deleteFAQ = (from v in db.FAQs
                             where v.Id == faq.Id
                             select v).FirstOrDefault();
            
            db.FAQs.Remove(deleteFAQ);


            await db.SaveChangesAsync();

            return RedirectToAction("Index");


        }
        //
        //GET: /FAQ/AddFAQ
        [Authorize(Roles = "Administrator")]
        public ActionResult AddFAQ()
        {
            return View();
        }

        //
        //POST: /FAQ/AddFAQ
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult>AddFAQ(FAQCreate model)
        {

            var faq = new FAQ { QUESTION = model.FAQ.QUESTION, ANSWER = model.FAQ.ANSWER };
            db.FAQs.Add(faq);
            
            await db.SaveChangesAsync();

            return View();
        }
	}
}