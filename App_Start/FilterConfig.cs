using System.Web;
using System.Web.Mvc;

namespace _25DH110574_Tien_KTRABE
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
