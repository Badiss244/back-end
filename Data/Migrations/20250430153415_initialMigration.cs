using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filiales",
                columns: table => new
                {
                    IdFiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filiales", x => x.IdFiliale);
                });

            migrationBuilder.CreateTable(
                name: "Parametres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Maintenance = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SxDefinitions",
                columns: table => new
                {
                    IdSDefinition = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameJaponaise = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SxDefinitions", x => x.IdSDefinition);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Factories",
                columns: table => new
                {
                    IdFactory = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FKfiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factories", x => x.IdFactory);
                    table.ForeignKey(
                        name: "FK_Factories_Filiales_FKfiliale",
                        column: x => x.FKfiliale,
                        principalTable: "Filiales",
                        principalColumn: "IdFiliale",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "critereDefinitions",
                columns: table => new
                {
                    IdCritereDefinition = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FKsxDefinition = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_critereDefinitions", x => x.IdCritereDefinition);
                    table.ForeignKey(
                        name: "FK_critereDefinitions_SxDefinitions_FKsxDefinition",
                        column: x => x.FKsxDefinition,
                        principalTable: "SxDefinitions",
                        principalColumn: "IdSDefinition",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    First_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FKfactory = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Factories_FKfactory",
                        column: x => x.FKfactory,
                        principalTable: "Factories",
                        principalColumn: "IdFactory",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sx",
                columns: table => new
                {
                    IdSx = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberS = table.Column<int>(type: "int", nullable: false),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameJaponaise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FKfactory = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sx", x => x.IdSx);
                    table.ForeignKey(
                        name: "FK_Sx_Factories_FKfactory",
                        column: x => x.FKfactory,
                        principalTable: "Factories",
                        principalColumn: "IdFactory",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                columns: table => new
                {
                    IdAudit = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsOld = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FKfactory = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FKauditor = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.IdAudit);
                    table.ForeignKey(
                        name: "FK_Audits_AspNetUsers_FKauditor",
                        column: x => x.FKauditor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audits_Factories_FKfactory",
                        column: x => x.FKfactory,
                        principalTable: "Factories",
                        principalColumn: "IdFactory",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Nofications",
                columns: table => new
                {
                    IdNofication = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleDes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdDes = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FKappuser = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nofications", x => x.IdNofication);
                    table.ForeignKey(
                        name: "FK_Nofications_AspNetUsers_FKappuser",
                        column: x => x.FKappuser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rapports",
                columns: table => new
                {
                    IdRapport = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    FKauditor = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FKfactory = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    X = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rapports", x => x.IdRapport);
                    table.ForeignKey(
                        name: "FK_Rapports_AspNetUsers_FKauditor",
                        column: x => x.FKauditor,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rapports_Factories_FKfactory",
                        column: x => x.FKfactory,
                        principalTable: "Factories",
                        principalColumn: "IdFactory",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Critaires",
                columns: table => new
                {
                    IdCritaire = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    FKsx = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Key = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Critaires", x => x.IdCritaire);
                    table.ForeignKey(
                        name: "FK_Critaires_Sx_FKsx",
                        column: x => x.FKsx,
                        principalTable: "Sx",
                        principalColumn: "IdSx",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evidence",
                columns: table => new
                {
                    IdEvidence = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FKrapport = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidence", x => x.IdEvidence);
                    table.ForeignKey(
                        name: "FK_Evidence_Rapports_FKrapport",
                        column: x => x.FKrapport,
                        principalTable: "Rapports",
                        principalColumn: "IdRapport",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanActions",
                columns: table => new
                {
                    IdPlanAction = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    FKfactory = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FKqualitym = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FKrapport = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanActions", x => x.IdPlanAction);
                    table.ForeignKey(
                        name: "FK_PlanActions_AspNetUsers_FKqualitym",
                        column: x => x.FKqualitym,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanActions_Factories_FKfactory",
                        column: x => x.FKfactory,
                        principalTable: "Factories",
                        principalColumn: "IdFactory",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanActions_Rapports_FKrapport",
                        column: x => x.FKrapport,
                        principalTable: "Rapports",
                        principalColumn: "IdRapport");
                });

            migrationBuilder.CreateTable(
                name: "Taches",
                columns: table => new
                {
                    IdTache = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    FKplanaction = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Pictures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commantaire = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taches", x => x.IdTache);
                    table.ForeignKey(
                        name: "FK_Taches_PlanActions_FKplanaction",
                        column: x => x.FKplanaction,
                        principalTable: "PlanActions",
                        principalColumn: "IdPlanAction",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Parametres",
                columns: new[] { "Id", "Maintenance" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), false });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FKfactory",
                table: "AspNetUsers",
                column: "FKfactory",
                unique: true,
                filter: "[FKfactory] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_FKauditor",
                table: "Audits",
                column: "FKauditor");

            migrationBuilder.CreateIndex(
                name: "IX_Audits_FKfactory",
                table: "Audits",
                column: "FKfactory");

            migrationBuilder.CreateIndex(
                name: "IX_Critaires_FKsx",
                table: "Critaires",
                column: "FKsx");

            migrationBuilder.CreateIndex(
                name: "IX_critereDefinitions_FKsxDefinition",
                table: "critereDefinitions",
                column: "FKsxDefinition");

            migrationBuilder.CreateIndex(
                name: "IX_Evidence_FKrapport",
                table: "Evidence",
                column: "FKrapport");

            migrationBuilder.CreateIndex(
                name: "IX_Factories_FKfiliale",
                table: "Factories",
                column: "FKfiliale");

            migrationBuilder.CreateIndex(
                name: "IX_Nofications_FKappuser",
                table: "Nofications",
                column: "FKappuser");

            migrationBuilder.CreateIndex(
                name: "IX_PlanActions_FKfactory",
                table: "PlanActions",
                column: "FKfactory");

            migrationBuilder.CreateIndex(
                name: "IX_PlanActions_FKqualitym",
                table: "PlanActions",
                column: "FKqualitym");

            migrationBuilder.CreateIndex(
                name: "IX_PlanActions_FKrapport",
                table: "PlanActions",
                column: "FKrapport");

            migrationBuilder.CreateIndex(
                name: "IX_Rapports_FKauditor",
                table: "Rapports",
                column: "FKauditor");

            migrationBuilder.CreateIndex(
                name: "IX_Rapports_FKfactory",
                table: "Rapports",
                column: "FKfactory");

            migrationBuilder.CreateIndex(
                name: "IX_Sx_FKfactory",
                table: "Sx",
                column: "FKfactory");

            migrationBuilder.CreateIndex(
                name: "IX_Taches_FKplanaction",
                table: "Taches",
                column: "FKplanaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Audits");

            migrationBuilder.DropTable(
                name: "Critaires");

            migrationBuilder.DropTable(
                name: "critereDefinitions");

            migrationBuilder.DropTable(
                name: "Evidence");

            migrationBuilder.DropTable(
                name: "Nofications");

            migrationBuilder.DropTable(
                name: "Parametres");

            migrationBuilder.DropTable(
                name: "Taches");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Sx");

            migrationBuilder.DropTable(
                name: "SxDefinitions");

            migrationBuilder.DropTable(
                name: "PlanActions");

            migrationBuilder.DropTable(
                name: "Rapports");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Factories");

            migrationBuilder.DropTable(
                name: "Filiales");
        }
    }
}
