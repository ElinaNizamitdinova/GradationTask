namespace MessageService.Models
{
    public partial class MessageRepository
    {
        public class MessageResponse
        {
            public Guid SenderId { get; set; }
            public string Message { get; set; }
        }
    }
}
