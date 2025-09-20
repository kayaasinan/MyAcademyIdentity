using Microsoft.AspNetCore.Mvc;

namespace EmailApp.ViewComponents.Main_Layout
{
    public class _MainLayoutEmptyPageComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var emptyPageMessage = "Görüntülenecek mesaj bulunamadı";
            return View("Default", emptyPageMessage);
        }
    }
}
