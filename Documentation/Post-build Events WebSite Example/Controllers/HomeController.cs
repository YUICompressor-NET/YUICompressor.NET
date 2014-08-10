using System.Web.Mvc;

namespace SampleWebSite.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to the post build event sample!"; 
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
