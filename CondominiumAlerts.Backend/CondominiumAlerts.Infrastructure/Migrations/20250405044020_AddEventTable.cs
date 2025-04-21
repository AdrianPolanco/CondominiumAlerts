using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CondominiumAlerts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages");*/

          /* migrationBuilder.DropTable(
                name: "CondominiumUser");*/

           /* migrationBuilder.AlterColumn<Guid>(
                name: "CondominiumId",
                table: "Messages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");*/

           /* migrationBuilder.CreateTable(
                name: "CondominiumUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondominiumUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CondominiumUsers_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CondominiumUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });*/

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsStarted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsFinished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsToday = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    CondominiumId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Event_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventUser",
                columns: table => new
                {
                    SuscribedToEventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuscribersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser", x => new { x.SuscribedToEventsId, x.SuscribersId });
                    table.ForeignKey(
                        name: "FK_EventUser_Event_SuscribedToEventsId",
                        column: x => x.SuscribedToEventsId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventUser_Users_SuscribersId",
                        column: x => x.SuscribersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

          /*  migrationBuilder.CreateIndex(
                name: "IX_CondominiumUsers_CondominiumId",
                table: "CondominiumUsers",
                column: "CondominiumId");*/

           /* migrationBuilder.CreateIndex(
                name: "IX_CondominiumUsers_UserId",
                table: "CondominiumUsers",
                column: "UserId");*/

            migrationBuilder.CreateIndex(
                name: "IX_Event_CondominiumId",
                table: "Event",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CreatedById",
                table: "Event",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_SuscribersId",
                table: "EventUser",
                column: "SuscribersId");

           /* migrationBuilder.AddForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Condominiums_CondominiumId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "CondominiumUsers");

            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.AlterColumn<Guid>(
                name: "CondominiumId",
                table: "Messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CondominiumUser",
                columns: table => new
                {
                    CondominiumsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondominiumUser", x => new { x.CondominiumsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_CondominiumUser_Condominiums_CondominiumsId",
                        column: x => x.CondominiumsId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CondominiumUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CondominiumUser_UsersId",
                table: "CondominiumUser",
                column: "UsersId");

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
