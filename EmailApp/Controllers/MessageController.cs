using EmailApp.Context;
using EmailApp.Entities;
using EmailApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace EmailApp.Controllers
{
    [Authorize]
    public class MessageController(AppDbContext _context, UserManager<AppUser> _userManager) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Include(x => x.Sender).Where(x => x.RecieverId == user.Id).ToList();
            ViewBag.messageCount = messages.Count;
            return View(messages);
        }
        public IActionResult MessageDetail(int id)
        {
            var message = _context.Messages.Include(x => x.Sender).Include(x => x.Reciever).FirstOrDefault(x => x.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        public IActionResult SendMessage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            var sender = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.nameSurname = sender.FirstName + " " + sender.LastName;
            var reciever = await _userManager.FindByEmailAsync(model.RecieverEmail);

            if (reciever == null)
            {
                ModelState.AddModelError("", "Alıcı bulunamadı!");
                return View(model);
            }

            var message = new Message
            {
                Body = model.Body,
                Subject = model.Subject,
                RecieverId = reciever.Id,
                SenderId = sender.Id,
                SendDate = DateTime.Now,
            };
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
