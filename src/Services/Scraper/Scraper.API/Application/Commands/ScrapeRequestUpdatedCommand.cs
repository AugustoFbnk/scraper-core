using MediatR;
using System;

namespace Scraper.API.Application.Commands
{
    public class ScrapeRequestUpdatedCommand : IRequest
    {
        public ScrapeRequestUpdatedCommand(long id, string userId, string[] searchText)
        {
            Id = id;
            UserId = userId;
            SearchText = searchText;
        }

        /// <summary>
        /// Scrape request ID
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Tenant(user) that has requested the scrappy
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Texts to scrappy in url
        /// </summary>
        public string[] SearchText { get; }

        public override string ToString()
        {
            return $"Id {Id} User {UserId} SearchText {string.Join(',', SearchText)} ";
        }
    }
}
