using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemGym.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeSalesHistoryUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar la columna UserId como nullable si no existe
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "SalesHistories",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SalesHistories");
        }
    }
}
