using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mp");

            migrationBuilder.CreateTable(
                name: "instruments",
                schema: "mp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instruments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "portfolios",
                schema: "mp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portfolios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prices",
                schema: "mp",
                columns: table => new
                {
                    InstrumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TsUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Px = table.Column<decimal>(type: "numeric(30,10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prices", x => new { x.InstrumentId, x.TsUtc });
                });

            migrationBuilder.CreateTable(
                name: "positions",
                schema: "mp",
                columns: table => new
                {
                    PortfolioId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstrumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(30,10)", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => new { x.PortfolioId, x.InstrumentId });
                    table.ForeignKey(
                        name: "FK_positions_portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalSchema: "mp",
                        principalTable: "portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_instruments_Symbol",
                schema: "mp",
                table: "instruments",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_positions_InstrumentId",
                schema: "mp",
                table: "positions",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_prices_TsUtc",
                schema: "mp",
                table: "prices",
                column: "TsUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instruments",
                schema: "mp");

            migrationBuilder.DropTable(
                name: "positions",
                schema: "mp");

            migrationBuilder.DropTable(
                name: "prices",
                schema: "mp");

            migrationBuilder.DropTable(
                name: "portfolios",
                schema: "mp");
        }
    }
}
