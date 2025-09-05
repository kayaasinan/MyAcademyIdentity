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
    }
}
