using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_Registration_Portal.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookingOn",
                table: "Bookings",
                newName: "BookedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookedOn",
                table: "Bookings",
                newName: "BookingOn");
        }
    }
}
