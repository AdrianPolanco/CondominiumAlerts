using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CondominiumAlerts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalEventIdFieldToNotificationss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Event_EventId",
                table: "Notifications");*/
           migrationBuilder.AddColumn<Guid>(
               name: "EventId",
               table: "Notifications",
               type: "uuid",
               nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Event_EventId",
                table: "Notifications",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Event_EventId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Event_EventId",
                table: "Notifications",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id");
        }
    }
}
