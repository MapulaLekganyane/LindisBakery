using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LindisBakery.Migrations
{
    /// <inheritdoc />
    public partial class Updaty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "ImageUrl", "IsAvailable", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Muffins", "Juicy beef patty with fresh lettuce, tomato, and special sauce", "/images/packet.jpeg", true, "Blueberry Muffins", 8.99m },
                    { 2, "Scones", "Classic pizza with fresh mozzarella, tomatoes, and basil", "/images/packet.jpeg", true, "Dumplings", 12.99m },
                    { 3, "Dumplings", "Fresh romaine lettuce with Caesar dressing, croutons and parmesan", "/images/blueberry.jpeg", true, "Chocolate Muffins", 7.99m },
                    { 4, "Cupcakes", "Rich chocolate cake with chocolate ganache", "/images/blueberry.jpeg", true, "Vanilla Muffins", 5.99m },
                    { 5, "Muffins", "Cold brewed coffee with ice and milk", "/images/blueberry.jpeg", true, "Chocolate Mint Muffins", 3.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
