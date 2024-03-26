﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Scraper.Infrastructure;

#nullable disable

namespace Scraper.Infrastructure.Migrations
{
    [DbContext(typeof(ScraperContext))]
    partial class ScraperContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ScraperAggregate.ScrapeRequest", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("PushNotificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SearchText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("scrape_request", (string)null);
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ScraperAggregate.ScrapeUrlFound", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("KeyWorkds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ScrapeRequestId")
                        .HasColumnType("bigint");

                    b.Property<string>("UrlFound")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ScrapeRequestId");

                    b.ToTable("scrape_url_found", (string)null);
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ScraperAggregate.ScrapeUrlFound", b =>
                {
                    b.HasOne("Scraper.Domain.AggregatesModel.ScraperAggregate.ScrapeRequest", null)
                        .WithMany("UrlFound")
                        .HasForeignKey("ScrapeRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ScraperAggregate.ScrapeRequest", b =>
                {
                    b.Navigation("UrlFound");
                });
#pragma warning restore 612, 618
        }
    }
}
