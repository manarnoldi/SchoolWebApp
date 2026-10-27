using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ElectionPollingAndResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassLeadershipRoleId",
                table: "ElectionPositions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponsibilityId",
                table: "ElectionPositions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsElected",
                table: "ElectionCandidates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LeadershipRecordedAt",
                table: "ElectionCandidates",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TieBreak",
                table: "ElectionCandidates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "ElectionCandidates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ElectionBallotChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ElectionId = table.Column<int>(type: "int", nullable: false),
                    BallotId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ElectionContestId = table.Column<int>(type: "int", nullable: false),
                    ElectionCandidateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionBallotChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionBallotChoices_ElectionCandidates_ElectionCandidateId",
                        column: x => x.ElectionCandidateId,
                        principalTable: "ElectionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElectionBallotChoices_ElectionContests_ElectionContestId",
                        column: x => x.ElectionContestId,
                        principalTable: "ElectionContests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ElectionStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ElectionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OfficerUserId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionStations_AspNetUsers_OfficerUserId",
                        column: x => x.OfficerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ElectionStations_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ElectionStationClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ElectionStationId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionStationClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionStationClasses_ElectionStations_ElectionStationId",
                        column: x => x.ElectionStationId,
                        principalTable: "ElectionStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionStationClasses_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ElectionVoteCasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ElectionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ElectionStationId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionVoteCasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionVoteCasts_ElectionStations_ElectionStationId",
                        column: x => x.ElectionStationId,
                        principalTable: "ElectionStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElectionVoteCasts_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ElectionVoteCasts_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");































            migrationBuilder.CreateIndex(
                name: "IX_ElectionPositions_ClassLeadershipRoleId",
                table: "ElectionPositions",
                column: "ClassLeadershipRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionPositions_ResponsibilityId",
                table: "ElectionPositions",
                column: "ResponsibilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionBallotChoices_BallotId",
                table: "ElectionBallotChoices",
                column: "BallotId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionBallotChoices_ElectionCandidateId",
                table: "ElectionBallotChoices",
                column: "ElectionCandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionBallotChoices_ElectionContestId",
                table: "ElectionBallotChoices",
                column: "ElectionContestId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionBallotChoices_ElectionId",
                table: "ElectionBallotChoices",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionStationClasses_ElectionStationId_SchoolClassId",
                table: "ElectionStationClasses",
                columns: new[] { "ElectionStationId", "SchoolClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectionStationClasses_SchoolClassId",
                table: "ElectionStationClasses",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionStations_ElectionId",
                table: "ElectionStations",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionStations_OfficerUserId",
                table: "ElectionStations",
                column: "OfficerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVoteCasts_ElectionId_StudentId",
                table: "ElectionVoteCasts",
                columns: new[] { "ElectionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVoteCasts_ElectionStationId",
                table: "ElectionVoteCasts",
                column: "ElectionStationId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionVoteCasts_StudentId",
                table: "ElectionVoteCasts",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionPositions_ClassLeadershipRoles_ClassLeadershipRoleId",
                table: "ElectionPositions",
                column: "ClassLeadershipRoleId",
                principalTable: "ClassLeadershipRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionPositions_Responsibilities_ResponsibilityId",
                table: "ElectionPositions",
                column: "ResponsibilityId",
                principalTable: "Responsibilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            ElectionsSetup.GrantPollingAndResultsMenus(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElectionPositions_ClassLeadershipRoles_ClassLeadershipRoleId",
                table: "ElectionPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionPositions_Responsibilities_ResponsibilityId",
                table: "ElectionPositions");

            migrationBuilder.DropTable(
                name: "ElectionBallotChoices");

            migrationBuilder.DropTable(
                name: "ElectionStationClasses");

            migrationBuilder.DropTable(
                name: "ElectionVoteCasts");

            migrationBuilder.DropTable(
                name: "ElectionStations");

            migrationBuilder.DropIndex(
                name: "IX_ElectionPositions_ClassLeadershipRoleId",
                table: "ElectionPositions");

            migrationBuilder.DropIndex(
                name: "IX_ElectionPositions_ResponsibilityId",
                table: "ElectionPositions");

            migrationBuilder.DropColumn(
                name: "ClassLeadershipRoleId",
                table: "ElectionPositions");

            migrationBuilder.DropColumn(
                name: "ResponsibilityId",
                table: "ElectionPositions");

            migrationBuilder.DropColumn(
                name: "IsElected",
                table: "ElectionCandidates");

            migrationBuilder.DropColumn(
                name: "LeadershipRecordedAt",
                table: "ElectionCandidates");

            migrationBuilder.DropColumn(
                name: "TieBreak",
                table: "ElectionCandidates");

            migrationBuilder.DropColumn(
                name: "Votes",
                table: "ElectionCandidates");






























        }
    }
}
