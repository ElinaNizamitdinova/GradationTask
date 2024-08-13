using MessageService.Interfaces;
using MessageService.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostGreDbContext.Models;

namespace MessageService.Models
{
    public partial class MessageRepository : IMessageRepository
    {
        private readonly string _objectName = nameof(MessageRepository);

        private readonly ILogger _logger;
        private readonly GradationTaskContext _context;

        public MessageRepository(GradationTaskContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger(_objectName);
        }

        public async Task<IActionResult> ReceiveMessageAsync(Guid id)
        {
            _logger.LogInformation($"Try to add receive message");

            try
            {
                var messages = await _context.MessageDbs
                    .Where(x => !x.IsReceived && x.RecipientId == id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                messages.ForEach(x => x.IsReceived = true);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                var result = messages.Select(x => new MessageResponse
                {
                    SenderId = x.SenderId,
                    Message = x.MessageText,
                });

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to receive message");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }

        public async Task<IActionResult> SendMessageAsync(MessageDto message)
        {
            _logger.LogInformation($"Try to add send message");

            try
            {
                var msg = new MessageDb
                {
                    SenderId = message.SenderId.Value,
                    RecipientId = message.RecipientId,
                    MessageText = message.Message,
                    IsReceived = false,
                };

                await _context.MessageDbs.AddAsync(msg).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                return new OkResult();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex.Message} while trying to send message");
                return new BadRequestObjectResult(ExceptionUtils.GetMostInnerException(ex).Message);
            }
        }
    }
}
