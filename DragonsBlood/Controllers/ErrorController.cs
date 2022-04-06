using System.Web.Mvc;

namespace DragonsBlood.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View("Error");
        }
    }
}