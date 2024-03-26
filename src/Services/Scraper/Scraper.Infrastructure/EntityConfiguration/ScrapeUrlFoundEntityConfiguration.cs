using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Infrastructure.EntityConfiguration
{
    public class ScrapeUrlFoundEntityConfiguration : IEntityTypeConfiguration<ScrapeUrlFound>
    {
        public void Configure(EntityTypeBuilder<ScrapeUrlFound> builder)
        {
            builder.ToTable("scrape_url_found");
            builder.HasKey(x => x.Id);
        }
    }
}
