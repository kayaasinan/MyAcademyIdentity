using Microsoft.AspNetCore.Mvc;
using X.PagedList;


namespace EmailApp.ViewComponents.Main_Layout
{
    public class _MainLayoutPagedComponent:ViewComponent
    {
        public IViewComponentResult Invoke(IPagedList model, string actionName)
        {
            ViewData["ActionName"] = actionName;
            return View(model);
        }

    }
}
