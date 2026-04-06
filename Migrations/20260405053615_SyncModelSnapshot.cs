using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequisitionManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Requisitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Requisitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HireByDate",
                table: "Requisitions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JdContent",
                table: "Requisitions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Requisitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "HireByDate",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "JdContent",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Requisitions");
        }
    }
}
