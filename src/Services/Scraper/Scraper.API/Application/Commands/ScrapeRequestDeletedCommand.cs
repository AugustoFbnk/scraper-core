using MediatR;

namespace Scraper.API.Application.Commands
{
    public class ScrapeRequestDeletedCommand : IRequest
    {
        public ScrapeRequestDeletedCommand(long id, string userId)
        {
            Id = id;
            UserId = userId;
        }

        /// <summary>
        /// Tenant(user) that has requested the scrappy
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Request Id
        /// </summary>
        public long Id { get; set; }
    }
}
