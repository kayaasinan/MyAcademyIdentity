using EmailApp.Entities;

namespace EmailApp.Models
{
    public class HeaderMessageViewModel
    {
        public string Email { get; set; }
        public List<Message> Messages { get; set; }
    }
}
