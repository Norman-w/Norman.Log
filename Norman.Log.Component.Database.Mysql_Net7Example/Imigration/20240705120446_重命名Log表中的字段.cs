using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Norman.Log.Component.Database.Mysql.Imigration
{
    /// <inheritdoc />
    public partial class 重命名Log表中的字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //这个方法是在迁移时调用的,将Log表中的LogType字段重命名为Type,将LogLevel字段重命名为Level 
            migrationBuilder.RenameColumn(
                name: "LogType",
                table: "Logs",
                newName: "Type");
            migrationBuilder.RenameColumn(
                name: "LogLevel",
                table: "Logs",
                newName: "Level");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //这个方法是在回滚迁移时调用的
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Logs",
                newName: "LogType");
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Logs",
                newName: "LogLevel");
        }
    }
}
