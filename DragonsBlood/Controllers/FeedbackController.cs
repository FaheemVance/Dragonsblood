using System;
using System.Linq;
using System.Web.Mvc;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Data.Feedback;

namespace DragonsBlood.Controllers
{
    [Authorize(Roles = "Admin, Member")]
    public class FeedbackController : Controller
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            using (var context = new ResourcesDbContext())
            {
                return View(context.Feedback.ToList());
            }
        }

        [HttpPost]
        public ActionResult SubmitFeedback(string feedback)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction("Index", "Home");

            if (!string.IsNullOrEmpty(feedback))
            {
                var feedbackItem = new FeedbackItem
                {
                    Creator = User.DisplayName(),
                    Comment = feedback,
                    TimeStamp = DateTime.UtcNow
                };
                using (var context = new ResourcesDbContext())
                {
                    context.Feedback.Add(feedbackItem);
                    context.SaveChanges();
                }

                return Content("Feedback has been submitted. Thank You");
            }

            return Content("Something went wrong, please inform DodgyMaster.");
        }
    }
}
