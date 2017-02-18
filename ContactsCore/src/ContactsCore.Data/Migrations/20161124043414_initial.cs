using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContactsCore.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsurePostgresExtension("uuid-ossp");

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    uid = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    created = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    modified = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    isdeleted = table.Column<bool>(nullable: false, defaultValue: false),

                    firstname = table.Column<string>(maxLength: 50, nullable: false),                    
                    lastname = table.Column<string>(maxLength: 50, nullable: false),                    
                    
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.id);
                    table.UniqueConstraint("AK_contacts_uid", x => x.uid);
                });

            migrationBuilder.CreateTable(
                name: "contactdetails",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    uid = table.Column<Guid>(nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    created = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    modified = table.Column<DateTime>(nullable: false, defaultValueSql: "(now() at time zone 'utc')"),
                    isdeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    
                    description = table.Column<string>(maxLength: 50, nullable: false),                    
                    value = table.Column<string>(maxLength: 50, nullable: false),
                    type = table.Column<int>(nullable: false),

                    contactid = table.Column<long>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contactdetails", x => x.id);
                    table.UniqueConstraint("AK_contactdetails_uid", x => x.uid);
                    table.ForeignKey(
                        name: "FK_contactdetails_contacts_contactid",
                        column: x => x.contactid,
                        principalTable: "contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contactdetails_contactid",
                table: "contactdetails",
                column: "contactid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contactdetails");

            migrationBuilder.DropTable(
                name: "contacts");
        }
    }
}
