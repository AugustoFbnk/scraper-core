namespace Scraper.Abstractions.Domain.DomainServices.Services
{
    public interface IFcmService
    {
        Task SendPushNotification(string token, string title, string body);
    }
}
