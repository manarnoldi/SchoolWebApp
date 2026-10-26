using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Teaching documents: schemes of work, weekly lesson plans and the record of
    // work, with a tracker of who has drafted, submitted and had each approved.
    //
    // - SchemesOfWork / SchemeOfWorkLessons: per teacher x subject x class x term,
    //   one row per week and lesson. Strand / sub-strand link to the curriculum
    //   (how sub-strands get mapped to terms) but the text is the teacher's copy.
    // - LessonPlanWeeks / LessonPlans / LessonPlanSteps: a week's plans for one
    //   scheme, submitted and approved together.
    // - RecordOfWorkEntries / RecordOfWorkChecks: what was taught against each
    //   scheme lesson, and the HOD's weekly "checked" stamp.
    // - Roles HeadOfDepartment and DeputyHeadTeacher.
    // - Approval workflows SchemeOfWork and LessonPlanWeek with one step, the
    //   Head of Department; schools add or change steps under Approval Workflows.
    // - GlobalSettings TeachingDocuments/*: when schemes and lesson plans fall due.
    // - MenuPermissions for the Teaching Documents menu. Only added to roles that
    //   already have menu permissions - a role with none sees the whole menu, and
    //   giving it these alone would hide everything else.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261026100000_TeachingDocumentsModule")]
    public partial class TeachingDocumentsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchemesOfWork",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    References = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumberOfWeeks = table.Column<int>(type: "int", nullable: false),
                    LessonsPerWeek = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemesOfWork", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemesOfWork_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchemesOfWork_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchemesOfWork_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SchemesOfWork_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LessonPlanWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchemeOfWorkId = table.Column<int>(type: "int", nullable: false),
                    WeekNo = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonPlanWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlanWeeks_SchemesOfWork_SchemeOfWorkId",
                        column: x => x.SchemeOfWorkId,
                        principalTable: "SchemesOfWork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecordOfWorkChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchemeOfWorkId = table.Column<int>(type: "int", nullable: false),
                    WeekNo = table.Column<int>(type: "int", nullable: false),
                    CheckedByUserId = table.Column<int>(type: "int", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
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
                    table.PrimaryKey("PK_RecordOfWorkChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordOfWorkChecks_AspNetUsers_CheckedByUserId",
                        column: x => x.CheckedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordOfWorkChecks_SchemesOfWork_SchemeOfWorkId",
                        column: x => x.SchemeOfWorkId,
                        principalTable: "SchemesOfWork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SchemeOfWorkLessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchemeOfWorkId = table.Column<int>(type: "int", nullable: false),
                    WeekNo = table.Column<int>(type: "int", nullable: false),
                    LessonNo = table.Column<int>(type: "int", nullable: false),
                    StrandId = table.Column<int>(type: "int", nullable: true),
                    SubStrandId = table.Column<int>(type: "int", nullable: true),
                    StrandName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubStrandName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LearningOutcomes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LearningExperiences = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KeyInquiryQuestions = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LearningResources = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssessmentMethods = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reflection = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_SchemeOfWorkLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemeOfWorkLessons_SchemesOfWork_SchemeOfWorkId",
                        column: x => x.SchemeOfWorkId,
                        principalTable: "SchemesOfWork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemeOfWorkLessons_Strands_StrandId",
                        column: x => x.StrandId,
                        principalTable: "Strands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SchemeOfWorkLessons_SubStrands_SubStrandId",
                        column: x => x.SubStrandId,
                        principalTable: "SubStrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LessonPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LessonPlanWeekId = table.Column<int>(type: "int", nullable: false),
                    SchemeOfWorkLessonId = table.Column<int>(type: "int", nullable: true),
                    LessonNo = table.Column<int>(type: "int", nullable: false),
                    LessonDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    StartTime = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EndTime = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Roll = table.Column<int>(type: "int", nullable: true),
                    StrandId = table.Column<int>(type: "int", nullable: true),
                    SubStrandId = table.Column<int>(type: "int", nullable: true),
                    StrandName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubStrandName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LearningOutcomes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KeyInquiryQuestions = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LearningResources = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganisationOfLearning = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Introduction = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IntroductionMinutes = table.Column<int>(type: "int", nullable: true),
                    DevelopmentMinutes = table.Column<int>(type: "int", nullable: true),
                    Conclusion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConclusionMinutes = table.Column<int>(type: "int", nullable: true),
                    ExtendedActivities = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Summary = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reflection = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_LessonPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlans_LessonPlanWeeks_LessonPlanWeekId",
                        column: x => x.LessonPlanWeekId,
                        principalTable: "LessonPlanWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonPlans_SchemeOfWorkLessons_SchemeOfWorkLessonId",
                        column: x => x.SchemeOfWorkLessonId,
                        principalTable: "SchemeOfWorkLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LessonPlans_Strands_StrandId",
                        column: x => x.StrandId,
                        principalTable: "Strands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LessonPlans_SubStrands_SubStrandId",
                        column: x => x.SubStrandId,
                        principalTable: "SubStrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecordOfWorkEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchemeOfWorkLessonId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TaughtOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkCovered = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remarks = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_RecordOfWorkEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordOfWorkEntries_SchemeOfWorkLessons_SchemeOfWorkLessonId",
                        column: x => x.SchemeOfWorkLessonId,
                        principalTable: "SchemeOfWorkLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LessonPlanSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LessonPlanId = table.Column<int>(type: "int", nullable: false),
                    StepNo = table.Column<int>(type: "int", nullable: false),
                    Activity = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_LessonPlanSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonPlanSteps_LessonPlans_LessonPlanId",
                        column: x => x.LessonPlanId,
                        principalTable: "LessonPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_LessonPlanWeekId",
                table: "LessonPlans",
                column: "LessonPlanWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_SchemeOfWorkLessonId",
                table: "LessonPlans",
                column: "SchemeOfWorkLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_StrandId",
                table: "LessonPlans",
                column: "StrandId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlans_SubStrandId",
                table: "LessonPlans",
                column: "SubStrandId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanSteps_LessonPlanId",
                table: "LessonPlanSteps",
                column: "LessonPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonPlanWeeks_SchemeOfWorkId_WeekNo",
                table: "LessonPlanWeeks",
                columns: new[] { "SchemeOfWorkId", "WeekNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecordOfWorkChecks_CheckedByUserId",
                table: "RecordOfWorkChecks",
                column: "CheckedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordOfWorkChecks_SchemeOfWorkId_WeekNo",
                table: "RecordOfWorkChecks",
                columns: new[] { "SchemeOfWorkId", "WeekNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecordOfWorkEntries_SchemeOfWorkLessonId",
                table: "RecordOfWorkEntries",
                column: "SchemeOfWorkLessonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemeOfWorkLessons_SchemeOfWorkId_WeekNo_LessonNo",
                table: "SchemeOfWorkLessons",
                columns: new[] { "SchemeOfWorkId", "WeekNo", "LessonNo" });

            migrationBuilder.CreateIndex(
                name: "IX_SchemeOfWorkLessons_StrandId",
                table: "SchemeOfWorkLessons",
                column: "StrandId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemeOfWorkLessons_SubStrandId",
                table: "SchemeOfWorkLessons",
                column: "SubStrandId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemesOfWork_SchoolClassId",
                table: "SchemesOfWork",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemesOfWork_SessionId",
                table: "SchemesOfWork",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemesOfWork_StaffDetailsId_SubjectId_SchoolClassId_Session~",
                table: "SchemesOfWork",
                columns: new[] { "StaffDetailsId", "SubjectId", "SchoolClassId", "SessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemesOfWork_SubjectId",
                table: "SchemesOfWork",
                column: "SubjectId");


            SeedRolesAndWorkflows(migrationBuilder);
            SeedSettingsAndMenus(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM MenuPermissions WHERE MenuPath LIKE '/teaching/%';");
            migrationBuilder.Sql("DELETE FROM GlobalSettings WHERE Module = 'TeachingDocuments';");
            migrationBuilder.Sql(@"
                DELETE a FROM ApprovalStepActions a JOIN ApprovalRequests r ON r.Id = a.ApprovalRequestId
                WHERE r.EntityType IN ('SchemeOfWork', 'LessonPlanWeek');");
            migrationBuilder.Sql("DELETE FROM ApprovalRequests WHERE EntityType IN ('SchemeOfWork', 'LessonPlanWeek');");
            migrationBuilder.Sql(@"
                DELETE s FROM ApprovalWorkflowSteps s JOIN ApprovalWorkflows w ON w.Id = s.ApprovalWorkflowId
                WHERE w.FormKey IN ('SchemeOfWork', 'LessonPlanWeek');");
            migrationBuilder.Sql("DELETE FROM ApprovalWorkflows WHERE FormKey IN ('SchemeOfWork', 'LessonPlanWeek');");
            // Roles: only if no other workflow step still uses them.
            migrationBuilder.Sql(@"
                DELETE ur FROM AspNetUserRoles ur JOIN AspNetRoles r ON r.Id = ur.RoleId
                WHERE r.NormalizedName IN ('HEADOFDEPARTMENT', 'DEPUTYHEADTEACHER')
                  AND NOT EXISTS (SELECT 1 FROM ApprovalWorkflowSteps s WHERE s.RoleId = r.Id);");
            migrationBuilder.Sql(@"
                DELETE r FROM AspNetRoles r
                WHERE r.NormalizedName IN ('HEADOFDEPARTMENT', 'DEPUTYHEADTEACHER')
                  AND NOT EXISTS (SELECT 1 FROM ApprovalWorkflowSteps s WHERE s.RoleId = r.Id);");

            migrationBuilder.DropTable(
                name: "LessonPlanSteps");

            migrationBuilder.DropTable(
                name: "RecordOfWorkChecks");

            migrationBuilder.DropTable(
                name: "RecordOfWorkEntries");

            migrationBuilder.DropTable(
                name: "LessonPlans");

            migrationBuilder.DropTable(
                name: "LessonPlanWeeks");

            migrationBuilder.DropTable(
                name: "SchemeOfWorkLessons");

            migrationBuilder.DropTable(
                name: "SchemesOfWork");
        }

        // Quotes a value for a SQL string literal.
        private static string Q(string value) => value.Replace("'", "''");

        private static void SeedRolesAndWorkflows(MigrationBuilder migrationBuilder)
        {
            foreach (var role in new[] { "HeadOfDepartment", "DeputyHeadTeacher" })
            {
                migrationBuilder.Sql($@"
                    INSERT INTO AspNetRoles (Name, NormalizedName, ConcurrencyStamp, Created, CreatedBy, Modified, ModifiedBy)
                    SELECT '{role}', '{role.ToUpperInvariant()}', UUID(), NOW(), 'admin', NOW(), 'admin' FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = '{role.ToUpperInvariant()}');");
            }

            var workflows = new[]
            {
                ("SchemeOfWork", "Scheme of Work Approval", "Schemes of work, submitted once per term for each subject and class."),
                ("LessonPlanWeek", "Lesson Plan Approval", "A week's lesson plans for one subject and class.")
            };
            foreach (var (formKey, name, description) in workflows)
            {
                migrationBuilder.Sql($@"
                    INSERT INTO ApprovalWorkflows (Name, FormKey, Description, IsMakerChecker, IsActive, Created, CreatedBy, Modified, ModifiedBy)
                    SELECT '{Q(name)}', '{formKey}', '{Q(description)}', 1, 1, NOW(), 'admin', NOW(), 'admin' FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM ApprovalWorkflows WHERE FormKey = '{formKey}');");
            }

            // One step, the Head of Department, on each workflow that has no steps yet.
            migrationBuilder.Sql(@"
                INSERT INTO ApprovalWorkflowSteps (ApprovalWorkflowId, `Rank`, Name, RoleId, IsFinal,
                    NotifyNextApprover, NotifyPreviousApprover, NotifyApplicant, Created, CreatedBy, Modified, ModifiedBy)
                SELECT w.Id, 1, 'Head of Department', r.Id, 1, 1, 0, 1, NOW(), 'admin', NOW(), 'admin'
                FROM ApprovalWorkflows w
                JOIN AspNetRoles r ON r.NormalizedName = 'HEADOFDEPARTMENT'
                WHERE w.FormKey IN ('SchemeOfWork', 'LessonPlanWeek')
                  AND NOT EXISTS (SELECT 1 FROM ApprovalWorkflowSteps s WHERE s.ApprovalWorkflowId = w.Id);");
        }

        private static void SeedSettingsAndMenus(MigrationBuilder migrationBuilder)
        {
            var settings = new[]
            {
                ("SchemeDueDays", "7", "Schemes of work are due this many days after the term starts."),
                ("LessonPlanDueDaysBefore", "0", "Lesson plans for a week are due this many days before the week starts (0 = on its Monday)."),
                ("LessonPlanDueTime", "08:00", "Time of day lesson plans are due (24-hour HH:mm).")
            };
            foreach (var (key, value, description) in settings)
            {
                migrationBuilder.Sql($@"
                    INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy, Modified, ModifiedBy)
                    SELECT 'TeachingDocuments', '{key}', '{value}', '{Q(description)}', NOW(), 'admin', NOW(), 'admin' FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'TeachingDocuments' AND SettingKey = '{key}');");
            }

            var teacherMenus = new[]
            {
                ("/teaching/schemes", "Teaching Documents > Schemes of Work"),
                ("/teaching/lesson-plans", "Teaching Documents > Lesson Plans"),
                ("/teaching/record-of-work", "Teaching Documents > Record of Work"),
                ("/teaching/tracker", "Teaching Documents > Tracker")
            };
            // Permission flag: whole-school view of every teacher's documents.
            var adminMenu = ("/teaching/admin", "Teaching Documents > Whole-school Access (Admin)");

            var grants = new (string Role, bool IsNewRole, bool WholeSchool)[]
            {
                ("Teacher", false, false),
                ("HeadOfDepartment", true, false),
                ("HeadTeacher", false, true),
                ("DeputyHeadTeacher", true, true),
                ("Administrator", false, true)
            };
            foreach (var (role, isNewRole, wholeSchool) in grants)
            {
                var menus = wholeSchool ? teacherMenus.Append(adminMenu) : teacherMenus;
                foreach (var (path, menuName) in menus)
                {
                    var onlyIfConfigured = isNewRole ? "" : $"AND EXISTS (SELECT 1 FROM MenuPermissions WHERE RoleId = '{role}')";
                    migrationBuilder.Sql($@"
                        INSERT INTO MenuPermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
                        SELECT '{role}', '{path}', '{Q(menuName)}', NOW(), 'admin', NOW(), 'admin' FROM DUAL
                        WHERE NOT EXISTS (SELECT 1 FROM MenuPermissions WHERE RoleId = '{role}' AND MenuPath = '{path}')
                        {onlyIfConfigured};");
                }
            }
        }
    }
}
