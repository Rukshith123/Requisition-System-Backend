using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequisitionManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Requisitions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Requisitions");
        }
    }
}
