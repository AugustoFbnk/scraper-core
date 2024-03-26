using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Infrastructure.EntityConfiguration
{
    public class ScrapeRequestEntityConfiguration : IEntityTypeConfiguration<ScrapeRequest>
    {
        public void Configure(EntityTypeBuilder<ScrapeRequest> builder)
        {
            builder.ToTable("scrape_request");
            builder.HasKey(x => x.Id);
        }
    }
}
