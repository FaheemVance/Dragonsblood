using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DragonsBlood.Chat.Hubs;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Data.Session;
using DragonsBlood.Data.Types;
using DragonsBlood.Models.AlertModels;
using DragonsBlood.Models.CustomModels;
using DragonsBlood.Models.PageModels;
using Microsoft.AspNet.SignalR;

namespace DragonsBlood.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Admin, Member, Moderator")]
    public class HomeController : Controller
    {
        public IHubContext Hub => GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
        public HomeController()
        {
            this.SetSession();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(GetIndexModel());
        }

        public ActionResult Chat()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult News()
        {
            using (var context = new ResourcesDbContext())
            {
                if (!User.Identity.IsAuthenticated || !User.IsRestricted())
                {
                    return
                        View(
                            context.Announcements.Where(a => a.Type == AnnounceType.News)
                                .OrderByDescending(a => a.Stamp)
                                .ToList());
                }

                return View(context.Announcements.OrderByDescending(a => a.Stamp).ToList());
            }
        }

        public ActionResult Alerts()
        {
            using (var context = new ApplicationDbContext())
            {
                var alerts =
                    context.Alerts.Include(a => a.Coordinates).OrderByDescending(a => a.TimeStamp)
                        .Where(a => !a.Retaliated)
                        .ToList()
                        .Concat(
                            context.Alerts.Include(a => a.Coordinates).OrderByDescending(a => a.TimeStamp)
                                .Where(a => a.Retaliated)
                                .Take(20)
                                .ToList());
                return View(alerts);
            }
        }

        public ActionResult Attack(string Id)
        {
            if(string.IsNullOrEmpty(Id))
                return RedirectToAction("Alerts");

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var alertId = int.Parse(Id);
                    var alert = context.Alerts.Include(a => a.Coordinates).FirstOrDefault(a => a.Id == alertId);

                    if(alert == null || alert.Retaliated)
                        return RedirectToAction("Alerts");

                    alert.Retaliated = true;
                    alert.CompletedBy = User.DisplayName();
                    context.SaveChanges();
                }
                UpdateAlerts();
                return RedirectToAction("Alerts");
            }
            catch (Exception)
            {
                return RedirectToAction("Alerts");
            }
        }

        public ActionResult ReOpen(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return RedirectToAction("Alerts");

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var alertId = int.Parse(Id);
                    var alert = context.Alerts.Include(a => a.Coordinates).FirstOrDefault(a => a.Id == alertId);

                    if (alert == null || !alert.Retaliated)
                        return RedirectToAction("Alerts");

                    alert.Retaliated = false;
                    alert.CompletedBy = "";
                    context.SaveChanges();
                }
                UpdateAlerts();
                return RedirectToAction("Alerts");
            }
            catch (Exception)
            {
                return RedirectToAction("Alerts");
            }
        }

        public ActionResult CreateAlert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAlert(Alert model)
        {
            if (!ModelState.IsValid)
                return View();

            if (string.IsNullOrEmpty(model.Attacker) || model.Coordinates == null)
                return View();

            var alert = new Alert();
            alert.Attacker = model.Attacker;
            alert.Coordinates = model.Coordinates;
            alert.TimeStamp = DateTime.UtcNow;
            alert.ShortKingdom = model.ShortKingdom;
            alert.Kingdom = GetKingdomFromShort(model.ShortKingdom);

            using (var context = new ApplicationDbContext())
            {
                context.Alerts.Add(alert);
                context.SaveChanges();

                var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                var alertCount = context.Alerts.Count(a => !a.Retaliated);
                hub.Clients.All.updateAlerts(alertCount);
            }

            return RedirectToAction("Alerts");
        }

        private IndexModel GetIndexModel()
        {
            List<Alert> Alerts = new List<Alert>();

            using(var appContext = new ApplicationDbContext())
            {
                Alerts = appContext.Alerts.Include(a => a.Coordinates).OrderByDescending(a => a.TimeStamp)
                    .Where(a => !a.Retaliated)
                    .Take(3)
                    .ToList();
            }

            using (var context = new ResourcesDbContext())
            {
                var isAuthenticated = User.Identity.IsAuthenticated && !string.IsNullOrEmpty(User.Identity.Name);
                var model = new IndexModel
                {
                    Documents = context.Documents.ToList(),
                    LatestAnnouncement =
                        isAuthenticated
                            ? context.Announcements.OrderByDescending(a => a.Stamp).FirstOrDefault()
                            : context.Announcements.Where(a => a.Type == AnnounceType.News)
                                .OrderByDescending(a => a.Stamp)
                                .FirstOrDefault(),
                    Alerts = Alerts
                };

                return model;
            }
        }

        private void UpdateAlerts()
        {
            using (var context = new ApplicationDbContext())
            {
                var alertCount = context.Alerts.Count(a => !a.Retaliated);

                Hub.Clients.All.updateAlerts(alertCount);
            }
        }

        private Kingdoms GetKingdomFromShort(ShortKingdom s)
        {
            switch (s)
            {
                case ShortKingdom.High:
                    return Kingdoms.HighKingdom;
                case ShortKingdom.Ice:
                    return Kingdoms.IceStormMountains;
                case ShortKingdom.Dark:
                    return Kingdoms.DarkMarshes;
                default:
                    return Kingdoms.HighKingdom;
            }
        }
    }
}