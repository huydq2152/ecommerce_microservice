using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityServer.Persistence.Migrations
{
    public partial class Create_Permission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e868a04c-10fe-4e5f-8111-5731f693a921");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "f7c6519c-2507-4af5-92af-9bafe288c250");

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Function = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Command = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    RoleId = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "fad491f9-819f-4e39-975c-1edf6782d43d", "7511e32f-1513-48f7-9d57-b4303f336380", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "faf72436-05de-433d-921b-47d4f7f9f4b0", "7791de65-edb6-4232-95e5-108dcf2b036a", "Customer", "CUSTOMER" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId_Function_Command",
                schema: "Identity",
                table: "Permissions",
                columns: new[] { "RoleId", "Function", "Command" },
                unique: true,
                filter: "[Function] IS NOT NULL AND [Command] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "Identity");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "fad491f9-819f-4e39-975c-1edf6782d43d");

            migrationBuilder.DeleteData(
                schema: "Identity",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "faf72436-05de-433d-921b-47d4f7f9f4b0");

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e868a04c-10fe-4e5f-8111-5731f693a921", "82700558-818d-47b9-9c5e-b51208c28f8f", "Customer", "CUSTOMER" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f7c6519c-2507-4af5-92af-9bafe288c250", "ee8dd522-a474-4183-b516-558e8e73bd00", "Admin", "ADMIN" });
        }
    }
}
