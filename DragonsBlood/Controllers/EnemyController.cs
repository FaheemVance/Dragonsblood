using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using DragonsBlood.Data;
using DragonsBlood.Data.Enemies;
using DragonsBlood.Data.Session;

namespace DragonsBlood.Controllers
{
    public class EnemyController : Controller
    {
        private ResourcesDbContext _db = new ResourcesDbContext();
        public ActionResult Index()
        {
            this.SetSession();
            var enemies = _db.Enemies;
            return View(enemies);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")] [HttpPost] [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,State")] Enemy enemy)
        {
            if (ModelState.IsValid)
            {
                _db.Enemies.Add(enemy);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(enemy);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enemy enemy = _db.Enemies.Find(id);
            if (enemy == null)
            {
                return HttpNotFound();
            }
            return View(enemy);
        }

        [Authorize(Roles = "Admin")] [HttpPost] [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,State")] Enemy enemy)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(enemy).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enemy);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enemy enemy = _db.Enemies.Find(id);
            if (enemy == null)
            {
                return HttpNotFound();
            }
            return View(enemy);
        }

        [Authorize(Roles = "Admin")] [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enemy enemy = _db.Enemies.Find(id);
            _db.Enemies.Remove(enemy);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}