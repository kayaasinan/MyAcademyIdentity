using EmailApp.Context;
using EmailApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailApp.ViewComponents.Main_Layout
{
    public class _MainLayoutHeaderComponent(AppDbContext _context, UserManager<AppUser> _userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = await _context.Messages.Include(x=>x.Sender).Where(x=>x.RecieverId==user.Id)
                .OrderByDescending(m => m.SendDate)
                .Take(2) // son 5 mesaj
                .ToListAsync();
            ViewBag.email= user.Email;
            return View(messages);
        }
    }
}
