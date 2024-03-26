using Essentials;
using Essentials.Web.Extensions;
using MediatR;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.API.Application.Commands
{
    public class ScrapeRequestCreatedCommandHandler : IRequestHandler<ScrapeRequestCreatedCommand>
    {
        private readonly IScrapeRequestRepository _scrapeRequestRepository;

        public ScrapeRequestCreatedCommandHandler(IScrapeRequestRepository scrapeRequestRepository)
        {
            _scrapeRequestRepository = scrapeRequestRepository;
        }

        public async Task Handle(ScrapeRequestCreatedCommand command, CancellationToken cancellationToken)
        {
            var searchText = string.Join(SC.WORD_DELIMITER, command.SearchText.ToList());
            var request = new ScrapeRequest(command.UserId,
             command.Url.FormatUrl(),
             searchText,
             command.PushNotificationToken);
            await _scrapeRequestRepository.CreateRequestAsync(request);
        }
    }
}
