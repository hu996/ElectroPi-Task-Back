using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplicatePreventionIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId_Title",
                table: "Tasks",
                columns: new[] { "ProjectId", "Title" },
                unique: true,
                filter: "[Status] <> 'Done'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectId_Title",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Name",
                table: "Projects");
        }
    }
}
