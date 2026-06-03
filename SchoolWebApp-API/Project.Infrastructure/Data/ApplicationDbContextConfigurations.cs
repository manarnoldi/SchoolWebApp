using Azure;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SchoolWebApp.Core.Constants;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace Project.Infrastructure.Data
{
    public class ApplicationDbContextConfigurations
    {
        public ApplicationDbContextConfigurations()
        { }
        public static void Configure(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<AppUser>().ToTable("Users");
            //modelBuilder.Entity<AppRole>().ToTable("Roles");
            //modelBuilder.Entity<IdentityUserRole<int>>().HasKey(k => new { k.UserId, k.RoleId });

            // Add any additional entity configurations here

            modelBuilder.Entity<Student>()
                .HasMany(e => e.Parents)
                .WithMany(e => e.Students)
                .UsingEntity<StudentParent>();

            modelBuilder.Entity<SpecificOutcome>()
                .HasMany(e => e.Competencies)
                .WithMany(e => e.SpecificOutcomes)
                .UsingEntity(j => j.ToTable("SpecificOutcomeCompetencies"));
            // Responsibility-SocialSkill many-to-many removed - merged into single Responsibility table with Category

            // Finance - decimal precision
            modelBuilder.Entity<FeeStructureItem>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<StudentInvoice>().Property(p => p.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<StudentInvoice>().Property(p => p.PaidAmount).HasPrecision(18, 2);
            modelBuilder.Entity<StudentInvoice>().Property(p => p.DiscountAmount).HasPrecision(18, 2);
            modelBuilder.Entity<StudentInvoiceItem>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<StudentInvoiceItem>().Property(p => p.Discount).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<ExpenseLine>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<JournalLine>().Property(p => p.Debit).HasPrecision(18, 2);
            modelBuilder.Entity<JournalLine>().Property(p => p.Credit).HasPrecision(18, 2);

            // Finance - FK behavior
            modelBuilder.Entity<Account>()
                .HasOne(a => a.ParentAccount)
                .WithMany()
                .HasForeignKey(a => a.ParentAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Approvals — cascade disabled; callers must remove children first.
            modelBuilder.Entity<ApprovalWorkflowStep>()
                .HasOne(s => s.ApprovalWorkflow)
                .WithMany(w => w.Steps)
                .HasForeignKey(s => s.ApprovalWorkflowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalWorkflowStep>()
                .HasOne(s => s.Role)
                .WithMany()
                .HasForeignKey(s => s.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalRequest>()
                .HasOne(r => r.ApprovalWorkflow)
                .WithMany()
                .HasForeignKey(r => r.ApprovalWorkflowId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalRequest>()
                .HasOne(r => r.SubmittedBy)
                .WithMany()
                .HasForeignKey(r => r.SubmittedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalRequest>()
                .HasIndex(r => new { r.EntityType, r.EntityId });

            modelBuilder.Entity<ApprovalStepAction>()
                .HasOne(a => a.ApprovalRequest)
                .WithMany(r => r.Actions)
                .HasForeignKey(a => a.ApprovalRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalStepAction>()
                .HasOne(a => a.AssignedTo)
                .WithMany()
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalStepAction>()
                .HasOne(a => a.ActionedBy)
                .WithMany()
                .HasForeignKey(a => a.ActionedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // An ExamResult belongs to a StudentSubject (the allocation). Cascade
            // so removing the allocation removes its results - a result can never
            // outlive the allocation. (Cascade convention is removed globally, so
            // this must be set explicitly.)
            modelBuilder.Entity<ExamResult>()
                .HasOne(er => er.StudentSubject)
                .WithMany()
                .HasForeignKey(er => er.StudentSubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sponsorships — decimal precision + FK rules
            modelBuilder.Entity<Sponsorship>().Property(p => p.FixedAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Sponsorship>().Property(p => p.Percentage).HasPrecision(5, 2);
            modelBuilder.Entity<SponsorPayment>().Property(p => p.Amount).HasPrecision(18, 2);

            modelBuilder.Entity<Sponsor>()
                .HasOne(s => s.ReceivableAccount)
                .WithMany()
                .HasForeignKey(s => s.ReceivableAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sponsorship>()
                .HasOne(s => s.Sponsor)
                .WithMany()
                .HasForeignKey(s => s.SponsorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SponsorshipFeeCategory>()
                .HasOne(s => s.Sponsorship)
                .WithMany(x => x.FeeCategories)
                .HasForeignKey(s => s.SponsorshipId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentInvoiceItem>()
                .HasOne(i => i.Sponsorship)
                .WithMany()
                .HasForeignKey(i => i.SponsorshipId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SponsorPayment>()
                .HasOne(p => p.Sponsor)
                .WithMany()
                .HasForeignKey(p => p.SponsorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void SeedData(ModelBuilder modelBuilder)
        {
            //Seed Roles
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 1,
                Name = Authorization.Roles.Administrator.ToString(),
                NormalizedName = Authorization.Roles.Administrator.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 2,
                Name = Authorization.Roles.HeadTeacher.ToString(),
                NormalizedName = Authorization.Roles.HeadTeacher.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 3,
                Name = Authorization.Roles.Teacher.ToString(),
                NormalizedName = Authorization.Roles.Teacher.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 4,
                Name = Authorization.Roles.Student.ToString(),
                NormalizedName = Authorization.Roles.Student.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 5,
                Name = Authorization.Roles.Parent.ToString(),
                NormalizedName = Authorization.Roles.Parent.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 6,
                Name = Authorization.Roles.Accounts.ToString(),
                NormalizedName = Authorization.Roles.Accounts.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 7,
                Name = Authorization.Roles.Others.ToString(),
                NormalizedName = Authorization.Roles.Others.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = 8,
                Name = Authorization.Roles.SuperAdministrator.ToString(),
                NormalizedName = Authorization.Roles.SuperAdministrator.ToString().ToUpper(),
                Created = DateTime.Now, CreatedBy = "admin", Modified = DateTime.Now, ModifiedBy = "admin"
            });
            // Auto-assign the default admin user to the SuperAdministrator role
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int> { RoleId = 8, UserId = 1 });


            //Add staff category
            modelBuilder.Entity<StaffCategory>().HasData(new StaffCategory
            {
                Id = 1,
                Name = "Non-teaching",
                Code = "SC001",
                Description = "",
                Rank = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            //Add designation
            modelBuilder.Entity<Designation>().HasData(new Designation
            {
                Id = 1,
                Name = "Supplier",
                Description = "",
                Rank = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            //Add employment type
            modelBuilder.Entity<EmploymentType>().HasData(new EmploymentType
            {
                Id = 1,
                Name = "Contract",
                Description = "",
                Rank = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            //Add nationality
            modelBuilder.Entity<Nationality>().HasData(new Nationality
            {
                Id = 1,
                Name = "Kenyan",
                Description = "",
                Rank = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            //Add religion
            modelBuilder.Entity<Religion>().HasData(new Religion
            {
                Id = 1,
                Name = "Christian",
                Description = "",
                Rank = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            //Add gender
            modelBuilder.Entity<Gender>().HasData(new Gender
            {
                Id = 1,
                Name = "Male",
                Description = "",
                Rank = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            //Add person
            modelBuilder.Entity<StaffDetails>().HasData(new StaffDetails
            {
                Id = 1,
                IdNumber = "Admin",
                NhifNo = "Admin",
                NssfNo = "Admin",
                KraPinNo = "Admin",
                EmploymentDate = DateTime.Now,
                EndofEmploymentDate = DateTime.Now,
                CurrentlyEmployed = true,
                StaffCategoryId = 1,
                DesignationId = 1,
                EmploymentTypeId = 1,
                FullName = "Admin",
                UPI = "Admin",
                DateOfBirth = DateTime.Now,
                Address = "Admin",
                PhoneNumber = "+254724920000",
                Email = Authorization.default_email,
                Status = Status.Active,
                OtherDetails = "Admin",
                NationalityId = 1,
                ReligionId = 1,
                GenderId = 1,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            var hasher = new PasswordHasher<AppUser>();
            //Seed Default User
            var defaultUser = new AppUser
            {
                Id = 1,
                UserName = Authorization.default_username,
                Email = Authorization.default_email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                FirstName = "SchoolSoft",
                LastName = "Administrator",
                PhoneNumber = "+254724920000",
                NormalizedUserName = Authorization.default_username.ToUpper(),
                NormalizedEmail = Authorization.default_email.ToUpper(),
                // Seeded admin keeps its shipped password — no forced change.
                MustChangePassword = false,
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            };
            defaultUser.PasswordHash = hasher.HashPassword(defaultUser, Authorization.default_password);
            modelBuilder.Entity<AppUser>().HasData(defaultUser);

            //Seed admin user to admin role
            var userRole = new IdentityUserRole<int> { RoleId = 1, UserId = 1 };
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(userRole);

            ////Seed school levels
            //EducationLevel schoolLevel1 = new EducationLevel()
            //{
            //    Id = 1,
            //    Name = "Primary",
            //    Description = "A level for primary schools",
            //    Created = DateTime.Now,
            //    CreatedBy = "admin",
            //    Modified = DateTime.Now,
            //    ModifiedBy = "admin"
            //};
            //EducationLevel schoolLevel2 = new EducationLevel()
            //{
            //    Id = 2,
            //    Name = "Secondary",
            //    Description = "A level for secondary schools",
            //    Created = DateTime.Now,
            //    CreatedBy = "admin",
            //    Modified = DateTime.Now,
            //    ModifiedBy = "admin"
            //};

            //modelBuilder.Entity<EducationLevel>().HasData(schoolLevel1);
            //modelBuilder.Entity<EducationLevel>().HasData(schoolLevel2);

            // Seed default approval workflows (2-step: Reviewer + Approver, Administrator role, maker-checker on)
            var seedTime = DateTime.Now;
            var formDefaults = new (int id, string formKey, string name)[]
            {
                (1, "BudgetAmendment", "Budget Amendment Approval"),
                (2, "Expense", "Expense Approval"),
                (3, "JournalEntry", "Journal Entry Approval"),
                (4, "CreditDebitNote", "Credit/Debit Note Approval"),
                (5, "Budget", "Budget Approval"),
            };
            foreach (var f in formDefaults)
            {
                modelBuilder.Entity<ApprovalWorkflow>().HasData(new
                {
                    Id = f.id,
                    Name = f.name,
                    FormKey = f.formKey,
                    Description = (string?)$"Default {f.name.ToLower()} workflow",
                    IsMakerChecker = true,
                    IsActive = true,
                    Created = seedTime,
                    CreatedBy = (string?)"admin",
                    Modified = seedTime,
                    ModifiedBy = (string?)"admin"
                });

                // Step 1: Reviewer (rank 1)
                modelBuilder.Entity<ApprovalWorkflowStep>().HasData(new
                {
                    Id = (f.id - 1) * 2 + 1,
                    ApprovalWorkflowId = f.id,
                    Rank = 1,
                    Name = "Reviewer",
                    RoleId = 1, // Administrator
                    IsFinal = false,
                    NotifyNextApprover = true,
                    NotifyPreviousApprover = false,
                    NotifyApplicant = true,
                    Created = seedTime,
                    CreatedBy = (string?)"admin",
                    Modified = seedTime,
                    ModifiedBy = (string?)"admin"
                });
                // Step 2: Approver (rank 2, final)
                modelBuilder.Entity<ApprovalWorkflowStep>().HasData(new
                {
                    Id = (f.id - 1) * 2 + 2,
                    ApprovalWorkflowId = f.id,
                    Rank = 2,
                    Name = "Approver",
                    RoleId = 1, // Administrator
                    IsFinal = true,
                    NotifyNextApprover = false,
                    NotifyPreviousApprover = true,
                    NotifyApplicant = true,
                    Created = seedTime,
                    CreatedBy = (string?)"admin",
                    Modified = seedTime,
                    ModifiedBy = (string?)"admin"
                });
            }
        }

    }
}
