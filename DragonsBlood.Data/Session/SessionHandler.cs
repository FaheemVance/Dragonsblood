using System.Web.Mvc;
using System.Linq;
using System.Web;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models;

namespace DragonsBlood.Data.Session
{
    public static class SessionHandler
    {
        public static void SetSession(this Controller controller)
        {
            ResourcesModel model = null;

            if (string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                model = new ResourcesModel()
                {
                    AlertCount = 0
                };
            }
            else
            {
                if (HttpContext.Current.User.IsRestricted())
                {
                        model = new ResourcesModel
                        {
                            AlertCount = 0
                        };
                }
                else
                {
                    using (var context = new ApplicationDbContext())
                    {
                        model = new ResourcesModel
                        {
                            AlertCount = context.Alerts.Count(a => !a.Retaliated)
                        };
                    }
                }
            }


            HttpContext.Current.Session.Remove("Alerts");
            HttpContext.Current.Session.Add("Alerts", model);
        }
    }
}