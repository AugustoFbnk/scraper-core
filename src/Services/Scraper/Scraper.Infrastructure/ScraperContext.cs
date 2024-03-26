using Microsoft.EntityFrameworkCore;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Infrastructure.EntityConfiguration;

namespace Scraper.Infrastructure
{
    public class ScraperContext : DbContext
    {
        public DbSet<ScrapeRequest> ScrapeRequest { get; set; }
        public DbSet<ScrapeUrlFound> ScrapeUrlFound { get; set; }

        public ScraperContext(DbContextOptions<ScraperContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ScrapeRequestEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ScrapeUrlFoundEntityConfiguration());
        }
    }
}
