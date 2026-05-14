using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SystemGym.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullableFinVigencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FinVigencia ya es nullable en PostgreSQL (timestamp with time zone permite NULL por defecto)
            // UserId ya existe en SalesHistories (agregado previamente fuera de migración)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
