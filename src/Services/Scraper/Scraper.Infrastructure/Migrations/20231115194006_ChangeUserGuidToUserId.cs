using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scraper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserGuidToUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scrape_request",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PushNotificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SearchText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scrape_request", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "scrape_url_found",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScrapeRequestId = table.Column<long>(type: "bigint", nullable: false),
                    UrlFound = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyWorkds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scrape_url_found", x => x.Id);
                    table.ForeignKey(
                        name: "FK_scrape_url_found_scrape_request_ScrapeRequestId",
                        column: x => x.ScrapeRequestId,
                        principalTable: "scrape_request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_scrape_url_found_ScrapeRequestId",
                table: "scrape_url_found",
                column: "ScrapeRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scrape_url_found");

            migrationBuilder.DropTable(
                name: "scrape_request");
        }
    }
}
