using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCore.Migrations
{
    public partial class AddPersonGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "Person",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_GroupID",
                table: "Person",
                column: "GroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Group_GroupID",
                table: "Person",
                column: "GroupID",
                principalTable: "Group",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_Group_GroupID",
                table: "Person");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Person_GroupID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "Person");
        }
    }
}
