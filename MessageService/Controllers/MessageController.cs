using MessageService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MessageService.Models.MessageRepository;

namespace MessageService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> SendMessage([FromBody]MessageDto message)
        {
            var guidString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var guid = Guid.Parse(guidString);

            message.SenderId = guid;

            return  await _messageRepository.SendMessageAsync(message).ConfigureAwait(false);
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<MessageResponse>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> ReceiveMessage()
        {
            var guidString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var guid = Guid.Parse(guidString);

            return await _messageRepository.ReceiveMessageAsync(guid).ConfigureAwait(false);
        }
    }
}
