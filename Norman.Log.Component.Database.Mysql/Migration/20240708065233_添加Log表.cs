using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Norman.Log.Component.Database.Mysql.Migration
{
    public partial class 添加Log表 : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    LoggerName = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Layer = table.Column<int>(nullable: false),
                    Module = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true),
                    Context = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log");
        }
    }
}
