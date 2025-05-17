using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserSupervision.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToUserBillStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserBillStatuses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserBillStatuses");
        }
    }
}
