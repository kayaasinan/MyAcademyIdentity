using EmailApp.Context;
using EmailApp.Entities;
using EmailApp.Enums;
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
            await SetMessageCounts();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages.Include(x => x.Sender).Where(x => x.RecieverId == user.Id && !x.IsDraft && !x.IsDeleted).ToList();
            ViewBag.messageCount = messages.Count;
            return View(messages);
        }
        public async Task<IActionResult> MessageDetail(int id)
        {
            await SetMessageCounts();
            var message = await _context.Messages.Include(x => x.Sender).Include(x => x.Reciever).FirstOrDefaultAsync(x => x.MessageId == id);

            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        public async Task<IActionResult> Sendbox()
        {
            await SetMessageCounts();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var messages = _context.Messages.Include(x => x.Sender).Include(x => x.Reciever).Where(x => x.SenderId == user.Id && !x.IsDeleted && !x.IsDraft).ToList();

            return View(messages);
        }

        public IActionResult SendMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model, string action)
        {
            await SetMessageCounts();
            var sender = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.nameSurname = sender.FirstName + " " + sender.LastName;
            var reciever = await _userManager.FindByEmailAsync(model.RecieverEmail);

            if (action == "send" && reciever == null)
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
                IsDraft = action == "draft"
            };
            _context.Messages.Add(message);
            _context.SaveChanges();
            if (action == "draft")
            {
                return RedirectToAction("DraftBox");
            }
            return RedirectToAction("Index");

        }
        private async Task SetMessageCounts()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                ViewBag.messageCount = 0;
                ViewBag.sentMessageCount = 0;
                ViewBag.draftMessageCount = 0;
                ViewBag.deletedMessageCount = 0;
                ViewBag.importantMessageCount = 0;
                ViewBag.businessMessageCount = 0;
                ViewBag.familyMessageCount = 0;
                return;
            }

            ViewBag.messageCount = await _context.Messages.CountAsync(x => x.RecieverId == user.Id && !x.IsDraft && !x.IsDeleted);

            ViewBag.sentMessageCount = await _context.Messages.CountAsync(x => x.SenderId == user.Id && !x.IsDraft && !x.IsDeleted);

            ViewBag.draftMessageCount = await _context.Messages.CountAsync(x => x.SenderId == user.Id && x.IsDraft && !x.IsDeleted);

            ViewBag.deletedMessageCount = await _context.Messages.CountAsync(x => (x.RecieverId == user.Id || x.SenderId == user.Id) && x.IsDeleted);

            ViewBag.importantMessageCount = await _context.Messages.CountAsync(x => x.RecieverId == user.Id && x.Category == MessageCategory.Important && !x.IsDeleted);

            ViewBag.businessMessageCount = await _context.Messages.CountAsync(x => x.RecieverId == user.Id && x.Category == MessageCategory.Business && !x.IsDeleted);

            ViewBag.familyMessageCount = await _context.Messages.CountAsync(x => x.RecieverId == user.Id && x.Category == MessageCategory.Family && !x.IsDeleted);
        }

        public async Task<IActionResult> DraftBox()
        {
            await SetMessageCounts();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var drafts = _context.Messages.Include(x => x.Sender).Include(x => x.Reciever)
                              .Where(x => x.SenderId == user.Id && x.IsDraft && !x.IsDeleted).ToList();

            return View(drafts);
        }
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var deletedMessage = _context.Messages.FirstOrDefault( x => x.MessageId == id && ((x.IsDraft && x.SenderId == user.Id) || (!x.IsDraft && (x.SenderId == user.Id || x.RecieverId == user.Id))));

            if (deletedMessage == null)
            {
                return NotFound();
            }
            deletedMessage.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("TrashBox");
        }
        public async Task<IActionResult> TrashBox()
        {
            await SetMessageCounts();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var deletedMessages = _context.Messages.Include(m => m.Sender).Include(m => m.Reciever).Where(m => m.IsDeleted && (m.SenderId == user.Id || m.RecieverId == user.Id)).ToList();
            return View(deletedMessages);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteMessagePermanently(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var message = _context.Messages.FirstOrDefault(m => m.MessageId == id && (m.SenderId == user.Id || m.RecieverId == user.Id));
            if (message == null) return NotFound();

            _context.Messages.Remove(message);
            _context.SaveChanges();

            return RedirectToAction("TrashBox");
        }
    }
}
