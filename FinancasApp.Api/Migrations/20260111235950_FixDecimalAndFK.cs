using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancasApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixDecimalAndFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Invoices_CurrentInvoiceId1",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_CurrentInvoiceId1",
                table: "CreditCards");

            migrationBuilder.DropColumn(
                name: "CurrentInvoiceId1",
                table: "CreditCards");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_CurrentInvoiceId",
                table: "CreditCards",
                column: "CurrentInvoiceId",
                unique: true,
                filter: "[CurrentInvoiceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Invoices_CurrentInvoiceId",
                table: "CreditCards",
                column: "CurrentInvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCards_Invoices_CurrentInvoiceId",
                table: "CreditCards");

            migrationBuilder.DropIndex(
                name: "IX_CreditCards_CurrentInvoiceId",
                table: "CreditCards");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentInvoiceId1",
                table: "CreditCards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCards_CurrentInvoiceId1",
                table: "CreditCards",
                column: "CurrentInvoiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCards_Invoices_CurrentInvoiceId1",
                table: "CreditCards",
                column: "CurrentInvoiceId1",
                principalTable: "Invoices",
                principalColumn: "Id");
        }
    }
}
