using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserSupervision.Migrations
{
    /// <inheritdoc />
    public partial class AddinvoiceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "UserBillStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "UserBillStatuses");
        }
    }
}
