using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recommendit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VectorDouble",
                table: "ShowInfos",
                type: "varchar(max)",
                maxLength: 8500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 8500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VectorDouble",
                table: "ShowInfos",
                type: "nvarchar(max)",
                maxLength: 8500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldMaxLength: 8500);
        }
    }
}
