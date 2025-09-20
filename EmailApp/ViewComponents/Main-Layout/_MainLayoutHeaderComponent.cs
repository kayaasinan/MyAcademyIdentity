using EmailApp.Context;
using EmailApp.Entities;
using EmailApp.Models;
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
            var messages = await _context.Messages.Include(x => x.Sender).Where(x => x.RecieverId == user.Id && !x.IsRead && x.Category == 0 && !x.IsDeleted && !x.IsDraft)
                .OrderByDescending(m => m.SendDate)
                .ThenByDescending(m => m.MessageId)
                .Take(2)
                .ToListAsync();
            var model = new HeaderMessageViewModel
            {
                Email = user.Email,
                Messages = messages
            };

            return View(model);
        }
    }
}
