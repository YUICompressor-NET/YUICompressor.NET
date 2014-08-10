using System.Web.Mvc;

namespace SampleWebSite.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static bool IsInDebugMode(this HtmlHelper helper)
        {
#if DEBUG
            return true;
#else  
              return false;  
#endif
        }
    }
}