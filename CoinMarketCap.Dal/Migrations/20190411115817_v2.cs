using Microsoft.EntityFrameworkCore.Migrations;

namespace CoinMarketCap.Dal.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_QuoteHistories_Timestamp",
                table: "QuoteHistories",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuoteHistories_Timestamp",
                table: "QuoteHistories");
        }
    }
}
