using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 業務報告システム.Data.Migrations
{
    public partial class addProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_ApplicationUserId",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.RenameTable(
                name: "Project",
                newName: "project");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ApplicationUserId",
                table: "project",
                newName: "IX_project_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_project",
                table: "project",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_project_AspNetUsers_ApplicationUserId",
                table: "project",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_AspNetUsers_ApplicationUserId",
                table: "project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_project",
                table: "project");

            migrationBuilder.RenameTable(
                name: "project",
                newName: "Project");

            migrationBuilder.RenameIndex(
                name: "IX_project_ApplicationUserId",
                table: "Project",
                newName: "IX_Project_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_ApplicationUserId",
                table: "Project",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
