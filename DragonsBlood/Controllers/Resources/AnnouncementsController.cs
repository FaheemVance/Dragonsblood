using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DragonsBlood.Data;
using DragonsBlood.Data.Session;
using DragonsBlood.Models.Announcements;

namespace DragonsBlood.Controllers.Resources
{
    [Authorize(Roles = "Admin")]
    public class AnnouncementsController : Controller
    {
        private ResourcesDbContext db = new ResourcesDbContext();

        public AnnouncementsController()
        {
            this.SetSession();
        }
        // GET: Announcements
        public ActionResult Index()
        {
            var context = db.Announcements.ToList();
            context.ForEach(a => a.Body = HttpContext.Server.HtmlDecode(a.Body));

            return View(context);
        }

        [Authorize(Roles = "Admin")]
        // GET: Announcements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Announcements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Subject,Body,Type")] Announcement announcement)
        {
            announcement.Body = HttpContext.Server.HtmlEncode(announcement.Body);
            announcement.Stamp = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                db.Announcements.Add(announcement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(announcement);
        }

        // GET: Announcements/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcements.Find(id);
            announcement.Body = HttpContext.Server.HtmlDecode(announcement.Body);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Subject,Body,Type")] Announcement announcement)
        {
            var context = new ResourcesDbContext();
            var announce = context.Announcements.FirstOrDefault(f => f.Id == announcement.Id);

            if(announce != null)
            {
                announce.Body = HttpContext.Server.HtmlEncode(announcement.Body);
                announce.Subject = announcement.Subject;
                announce.Type = announcement.Type;
                announcement.Stamp = DateTime.UtcNow;
            }
            
            if (ModelState.IsValid)
            {
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(announcement);
        }

        // GET: Announcements/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = db.Announcements.Find(id);
            announcement.Body = HttpContext.Server.HtmlDecode(announcement.Body);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Announcement announcement = db.Announcements.Find(id);
            db.Announcements.Remove(announcement);
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
