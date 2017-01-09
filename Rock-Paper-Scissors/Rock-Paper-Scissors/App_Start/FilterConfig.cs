using System.Web;
using System.Web.Mvc;

namespace Rock_Paper_Scissors
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
