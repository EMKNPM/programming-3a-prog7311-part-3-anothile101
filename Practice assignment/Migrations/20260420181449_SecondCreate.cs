using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practice_assignment.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContactDetails", "Name" },
                values: new object[] { "info@globalheights.com | +27 31 000 0001", "Global Heights Ltd" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Euro Cargo Depo");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContactDetails", "Name", "Region" },
                values: new object[] { "ops@aseshipping.com | +1 310 000 0003", "Asendia Shipping Co", "America" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ContactDetails", "Name" },
                values: new object[] { "info@globalfreight.com | +27 31 000 0001", "Global Freight Ltd" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Euro Cargo GmbH");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ContactDetails", "Name", "Region" },
                values: new object[] { "ops@pacshipping.com | +1 310 000 0003", "Pacific Shipping Co", "Americas" });
        }
    }
}
