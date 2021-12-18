using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiFamilyPortal.Data.Migrations
{
    public partial class CRMModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrmContactMarkets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactMarkets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmContactRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CoreTeam = table.Column<bool>(type: "bit", nullable: false),
                    SystemDefined = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarketNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmContactAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Primary = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmContactAddresses_CrmContacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRMContactCRMContactMarket",
                columns: table => new
                {
                    ContactsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MarketsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMContactCRMContactMarket", x => new { x.ContactsId, x.MarketsId });
                    table.ForeignKey(
                        name: "FK_CRMContactCRMContactMarket_CrmContactMarkets_MarketsId",
                        column: x => x.MarketsId,
                        principalTable: "CrmContactMarkets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRMContactCRMContactMarket_CrmContacts_ContactsId",
                        column: x => x.ContactsId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CRMContactCRMContactRole",
                columns: table => new
                {
                    ContactsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMContactCRMContactRole", x => new { x.ContactsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_CRMContactCRMContactRole_CrmContactRoles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "CrmContactRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CRMContactCRMContactRole_CrmContacts_ContactsId",
                        column: x => x.ContactsId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmContactEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Primary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmContactEmails_CrmContacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmContactLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmContactLogs_CrmContacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmContactPhones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Primary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmContactPhones_CrmContacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmContactReminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemGenerated = table.Column<bool>(type: "bit", nullable: false),
                    Dismissed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmContactReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmContactReminders_CrmContacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrmNotableDates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recurring = table.Column<bool>(type: "bit", nullable: false),
                    DismissReminders = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmNotableDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmNotableDates_CrmContacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "CrmContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactAddresses_ContactId",
                table: "CrmContactAddresses",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CRMContactCRMContactMarket_MarketsId",
                table: "CRMContactCRMContactMarket",
                column: "MarketsId");

            migrationBuilder.CreateIndex(
                name: "IX_CRMContactCRMContactRole_RolesId",
                table: "CRMContactCRMContactRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactEmails_ContactId",
                table: "CrmContactEmails",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactLogs_ContactId",
                table: "CrmContactLogs",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactMarkets_Name",
                table: "CrmContactMarkets",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactPhones_ContactId",
                table: "CrmContactPhones",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactReminders_ContactId",
                table: "CrmContactReminders",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmContactRoles_Name",
                table: "CrmContactRoles",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CrmNotableDates_ContactId",
                table: "CrmNotableDates",
                column: "ContactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrmContactAddresses");

            migrationBuilder.DropTable(
                name: "CRMContactCRMContactMarket");

            migrationBuilder.DropTable(
                name: "CRMContactCRMContactRole");

            migrationBuilder.DropTable(
                name: "CrmContactEmails");

            migrationBuilder.DropTable(
                name: "CrmContactLogs");

            migrationBuilder.DropTable(
                name: "CrmContactPhones");

            migrationBuilder.DropTable(
                name: "CrmContactReminders");

            migrationBuilder.DropTable(
                name: "CrmNotableDates");

            migrationBuilder.DropTable(
                name: "CrmContactMarkets");

            migrationBuilder.DropTable(
                name: "CrmContactRoles");

            migrationBuilder.DropTable(
                name: "CrmContacts");
        }
    }
}
