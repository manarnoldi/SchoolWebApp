using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Timetable, first part: how the school day is set up and the rules the
    // generator follows, plus the timetables themselves.
    //
    // - BellSchedules / BellSlots / BellScheduleLevels: the day (lessons, breaks,
    //   lunch) for a group of grades; each grade follows one schedule.
    // - TimetableFixedSlots: slots taken before generating - an activity such as
    //   games, or a subject such as PPI on Friday morning.
    // - TimetableSubjectRules: leave out, morning / afternoon, doubles, per day.
    // - TimetableOptionGroups / TimetableOptionGroupSubjects: subjects a class
    //   takes at the same time (CRE / IRE, Senior School electives).
    // - TeacherTimetableRules / TeacherUnavailabilities: limits and times off.
    // - SchoolTimetables / TimetableLessons: a timetable version and its cards.
    // - MenuPermissions for the Timetable menu. Only added to roles that already
    //   have menu permissions - a role with none sees the whole menu, and giving
    //   it these alone would hide everything else.
    //
    // Scaffolded, with the unrelated UpdateData drift removed (see the note on
    // UniquePayrollTypeCodes).
    public partial class TimetableFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BellSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Days = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BellSchedules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SchoolTimetables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PublishedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GeneratedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    GenerationReport = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTimetables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolTimetables_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeacherTimetableRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    MaxLessonsPerDay = table.Column<int>(type: "int", nullable: true),
                    MaxConsecutiveLessons = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherTimetableRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherTimetableRules_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeacherUnavailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    Reason = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherUnavailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherUnavailabilities_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimetableOptionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LearningLevelId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableOptionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableOptionGroups_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimetableSubjectRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    LearningLevelId = table.Column<int>(type: "int", nullable: true),
                    Excluded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Placement = table.Column<int>(type: "int", nullable: false),
                    DoubleLessons = table.Column<int>(type: "int", nullable: false),
                    MaxPerDay = table.Column<int>(type: "int", nullable: false),
                    AvoidLastLesson = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableSubjectRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableSubjectRules_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimetableSubjectRules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BellScheduleLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BellScheduleId = table.Column<int>(type: "int", nullable: false),
                    LearningLevelId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BellScheduleLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BellScheduleLevels_BellSchedules_BellScheduleId",
                        column: x => x.BellScheduleId,
                        principalTable: "BellSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BellScheduleLevels_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BellSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BellScheduleId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time(6)", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Part = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BellSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BellSlots_BellSchedules_BellScheduleId",
                        column: x => x.BellScheduleId,
                        principalTable: "BellSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimetableOptionGroupSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TimetableOptionGroupId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableOptionGroupSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableOptionGroupSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimetableOptionGroupSubjects_TimetableOptionGroups_Timetable~",
                        column: x => x.TimetableOptionGroupId,
                        principalTable: "TimetableOptionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimetableFixedSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BellScheduleId = table.Column<int>(type: "int", nullable: false),
                    BellSlotId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    LearningLevelId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    Label = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableFixedSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableFixedSlots_BellSchedules_BellScheduleId",
                        column: x => x.BellScheduleId,
                        principalTable: "BellSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimetableFixedSlots_BellSlots_BellSlotId",
                        column: x => x.BellSlotId,
                        principalTable: "BellSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableFixedSlots_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableFixedSlots_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TimetableLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchoolTimetableId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: true),
                    Day = table.Column<int>(type: "int", nullable: true),
                    BellSlotId = table.Column<int>(type: "int", nullable: true),
                    Length = table.Column<int>(type: "int", nullable: false),
                    LinkNo = table.Column<int>(type: "int", nullable: true),
                    IsLocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsManual = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetableLessons_BellSlots_BellSlotId",
                        column: x => x.BellSlotId,
                        principalTable: "BellSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableLessons_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TimetableLessons_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableLessons_SchoolTimetables_SchoolTimetableId",
                        column: x => x.SchoolTimetableId,
                        principalTable: "SchoolTimetables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimetableLessons_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BellScheduleLevels_BellScheduleId",
                table: "BellScheduleLevels",
                column: "BellScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_BellScheduleLevels_LearningLevelId",
                table: "BellScheduleLevels",
                column: "LearningLevelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BellSlots_BellScheduleId",
                table: "BellSlots",
                column: "BellScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTimetables_SessionId",
                table: "SchoolTimetables",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetableRules_StaffDetailsId",
                table: "TeacherTimetableRules",
                column: "StaffDetailsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherUnavailabilities_StaffDetailsId",
                table: "TeacherUnavailabilities",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableFixedSlots_BellScheduleId",
                table: "TimetableFixedSlots",
                column: "BellScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableFixedSlots_BellSlotId",
                table: "TimetableFixedSlots",
                column: "BellSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableFixedSlots_LearningLevelId",
                table: "TimetableFixedSlots",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableFixedSlots_SubjectId",
                table: "TimetableFixedSlots",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableLessons_BellSlotId",
                table: "TimetableLessons",
                column: "BellSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableLessons_SchoolClassId",
                table: "TimetableLessons",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableLessons_SchoolTimetableId_SchoolClassId",
                table: "TimetableLessons",
                columns: new[] { "SchoolTimetableId", "SchoolClassId" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableLessons_SchoolTimetableId_StaffDetailsId",
                table: "TimetableLessons",
                columns: new[] { "SchoolTimetableId", "StaffDetailsId" });

            migrationBuilder.CreateIndex(
                name: "IX_TimetableLessons_StaffDetailsId",
                table: "TimetableLessons",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableLessons_SubjectId",
                table: "TimetableLessons",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableOptionGroups_LearningLevelId",
                table: "TimetableOptionGroups",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableOptionGroupSubjects_SubjectId",
                table: "TimetableOptionGroupSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableOptionGroupSubjects_TimetableOptionGroupId_SubjectId",
                table: "TimetableOptionGroupSubjects",
                columns: new[] { "TimetableOptionGroupId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubjectRules_LearningLevelId",
                table: "TimetableSubjectRules",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetableSubjectRules_SubjectId_LearningLevelId",
                table: "TimetableSubjectRules",
                columns: new[] { "SubjectId", "LearningLevelId" },
                unique: true);

            SeedMenus(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MenuPermissions WHERE MenuPath LIKE '/timetable/%';");

            migrationBuilder.DropTable(
                name: "BellScheduleLevels");

            migrationBuilder.DropTable(
                name: "TeacherTimetableRules");

            migrationBuilder.DropTable(
                name: "TeacherUnavailabilities");

            migrationBuilder.DropTable(
                name: "TimetableFixedSlots");

            migrationBuilder.DropTable(
                name: "TimetableLessons");

            migrationBuilder.DropTable(
                name: "TimetableOptionGroupSubjects");

            migrationBuilder.DropTable(
                name: "TimetableSubjectRules");

            migrationBuilder.DropTable(
                name: "BellSlots");

            migrationBuilder.DropTable(
                name: "SchoolTimetables");

            migrationBuilder.DropTable(
                name: "TimetableOptionGroups");

            migrationBuilder.DropTable(
                name: "BellSchedules");
        }

        private static void SeedMenus(MigrationBuilder migrationBuilder)
        {
            // Setting up and building timetables: the school leadership.
            var manage = new[]
            {
                ("/timetable/timetables", "Timetable > Timetables"),
                ("/timetable/setup", "Timetable > Setup")
            };
            // Published timetables, read-only, and each teacher's own.
            var view = new[]
            {
                ("/timetable/view", "Timetable > View Timetables"),
                ("/timetable/my", "Timetable > My Timetable")
            };
            var grants = new (string Role, bool Manage)[]
            {
                ("Teacher", false),
                ("HeadOfDepartment", false),
                ("HeadTeacher", true),
                ("DeputyHeadTeacher", true),
                ("Administrator", true)
            };
            foreach (var (role, canManage) in grants)
            {
                var menus = canManage ? manage.Concat(view) : view;
                foreach (var (path, menuName) in menus)
                {
                    migrationBuilder.Sql($@"
                        INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                        SELECT '{role}', '{path}', '{menuName}', NOW(), 'admin', NOW(), 'admin' FROM DUAL
                        WHERE NOT EXISTS (SELECT 1 FROM MenuPermissions WHERE RoleId = '{role}' AND MenuPath = '{path}')
                          AND EXISTS (SELECT 1 FROM MenuPermissions WHERE RoleId = '{role}');");
                }
            }
        }
    }
}
