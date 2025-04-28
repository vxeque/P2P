using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace p2p.api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "P2PItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceName = table.Column<string>(type: "TEXT", nullable: true),
                    DeviceType = table.Column<string>(type: "TEXT", nullable: true),
                    DeviceIp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_P2PItems", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "P2PItems");
        }
    }
}
