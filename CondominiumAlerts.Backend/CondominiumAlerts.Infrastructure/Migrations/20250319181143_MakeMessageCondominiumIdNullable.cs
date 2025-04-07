using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CondominiumAlerts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMessageCondominiumIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "CondominiumId",
                table: "Messages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "CondominiumId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
