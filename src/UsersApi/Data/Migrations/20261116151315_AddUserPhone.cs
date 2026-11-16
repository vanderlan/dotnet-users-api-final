// EX04 - Novo campo no usuário - Aqui é a segunda migration, gerada pelo comando: Up adiciona a coluna Phone e Down remove (a tabela e os dados continuam)
// Comando: dotnet ef migrations add AddUserPhone --project src/UsersApi --output-dir Data/Migrations
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsersApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");
        }
    }
}
