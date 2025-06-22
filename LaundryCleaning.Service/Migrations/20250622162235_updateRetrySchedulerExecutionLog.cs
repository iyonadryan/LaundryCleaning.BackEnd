using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LaundryCleaning.Service.Migrations
{
    /// <inheritdoc />
    public partial class updateRetrySchedulerExecutionLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "_schedulerLog",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "_schedulerLog");
        }
    }
}
