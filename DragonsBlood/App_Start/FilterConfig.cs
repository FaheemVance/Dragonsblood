using System.Web.Mvc;
using DragonsBlood.Filters;

namespace DragonsBlood
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SecurityFilterAttribute());
        }
    }
}
