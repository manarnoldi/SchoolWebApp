﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolWebApp.Core.Constants;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Settings;

namespace Project.Infrastructure.Data
{
    public class ApplicationDbContextConfigurations
    {
        public ApplicationDbContextConfigurations()
        {
        }
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<AppRole>().ToTable("Roles");

            // Add any additional entity configurations here
        }

        public static void SeedData(ModelBuilder modelBuilder)
        {
            //Seed Roles
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "717d9b15-a428-440c-b26b-08d3bbb68b02",
                Name = Authorization.Roles.Administrator.ToString(),
                NormalizedName = Authorization.Roles.Administrator.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                Name = Authorization.Roles.HeadTeacher.ToString(),
                NormalizedName = Authorization.Roles.HeadTeacher.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "48c50c3a-9958-453b-b649-4e21af131322",
                Name = Authorization.Roles.Teacher.ToString(),
                NormalizedName = Authorization.Roles.Teacher.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "448df289-142c-4959-a912-60733515e1b4",
                Name = Authorization.Roles.Student.ToString(),
                NormalizedName = Authorization.Roles.Student.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                Name = Authorization.Roles.Parent.ToString(),
                NormalizedName = Authorization.Roles.Parent.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "cd12b44b-103b-48df-8887-a2bf42e0651e",
                Name = Authorization.Roles.Accounts.ToString(),
                NormalizedName = Authorization.Roles.Accounts.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = "97942bee-ef12-4425-8225-4f293d0f36dd",
                Name = Authorization.Roles.Visitor.ToString(),
                NormalizedName = Authorization.Roles.Visitor.ToString().ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            });

            var hasher = new PasswordHasher<AppUser>();

            //Seed Default User
            var defaultUser = new AppUser
            {
                Id = "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                UserName = Authorization.default_username,
                Email = Authorization.default_email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                FirstName = "SchoolSoft",
                LastName = "Administrator",
                PhoneNumber = "+254724920000",
                NormalizedUserName = Authorization.default_username.ToUpper(),
                NormalizedEmail = Authorization.default_email.ToUpper(),
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            };
            defaultUser.PasswordHash = hasher.HashPassword(defaultUser, Authorization.default_password);
            modelBuilder.Entity<AppUser>().HasData(defaultUser);

            //Seed admin user to admin role
            var userRole = new IdentityUserRole<string> { RoleId = "717d9b15-a428-440c-b26b-08d3bbb68b02", UserId = "7e67d486-af3e-49f1-a109-a2b864b8e0ec" };

            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.RoleId, r.UserId });
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRole);

            //Seed school levels
            SchoolLevel schoolLevel1 = new SchoolLevel()
            {
                Id = 1,
                Name = "Primary",
                Description = "A level for primary schools",
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            };
            SchoolLevel schoolLevel2 = new SchoolLevel()
            {
                Id = 2,
                Name = "Secondary",
                Description = "A level for secondary schools",
                Created = DateTime.Now,
                CreatedBy = "admin",
                Modified = DateTime.Now,
                ModifiedBy = "admin"
            };

            modelBuilder.Entity<SchoolLevel>().HasData(schoolLevel1);
            modelBuilder.Entity<SchoolLevel>().HasData(schoolLevel2);

        }

    }
}
