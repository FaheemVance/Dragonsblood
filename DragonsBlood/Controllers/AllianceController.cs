using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using DragonsBlood.Data;
using DragonsBlood.Data.Allies;
using DragonsBlood.Data.Session;

namespace DragonsBlood.Controllers
{
    public class AllianceController : Controller
    {
        private ResourcesDbContext _db = new ResourcesDbContext();

        public ActionResult Index()
        {
            this.SetSession();
            var alliances = _db.Alliances;
            return View(alliances);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] [HttpPost] [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,State")] Alliance alliance)
        {
            if (ModelState.IsValid)
            {
                _db.Alliances.Add(alliance);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(alliance);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alliance alliance = _db.Alliances.Find(id);
            if (alliance == null)
            {
                return HttpNotFound();
            }
            return View(alliance);
        }

        [Authorize(Roles = "Admin")] [HttpPost] [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,State")] Alliance alliance)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(alliance).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(alliance);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Alliance alliance = _db.Alliances.Find(id);
            if (alliance == null)
            {
                return HttpNotFound();
            }
            return View(alliance);
        }

        [Authorize(Roles = "Admin")] [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Alliance alliance = _db.Alliances.Find(id);
            _db.Alliances.Remove(alliance);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}