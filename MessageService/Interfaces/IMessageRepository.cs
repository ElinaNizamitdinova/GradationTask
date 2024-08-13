using Microsoft.AspNetCore.Mvc;
using static MessageService.Models.MessageRepository;

namespace MessageService.Interfaces
{
    public interface IMessageRepository
    {
        Task<IActionResult> SendMessageAsync(MessageDto message);
        Task<IActionResult> ReceiveMessageAsync(Guid id);
    }
}
