using Microsoft.AspNetCore.Mvc;

namespace EmailApp.ViewComponents.Main_Layout
{
    public class _MainLayoutSidebarComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
