namespace MessageService.Models
{
    public partial class MessageRepository
    {
        public class MessageDto
        {
            public Guid? SenderId { get; set; }
            public Guid RecipientId { get; set; }
            public string Message { get; set; }
        }
    }
}
