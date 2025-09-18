using EmailApp.Enums;

namespace EmailApp.Entities
{
    public class Message
    {
        public int RecieverId { get; set; }
        public AppUser Reciever { get; set; }
        public int SenderId { get; set; }
        public AppUser Sender { get; set; }
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsDraft { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsImportant { get; set; }
        public bool IsRead { get; set; }
        public MessageCategory Category { get; set; }=MessageCategory.Default;
    }
}
