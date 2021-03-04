using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models.LoginModels {
    public class UserContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options) {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            string adminRoleName = "admin";
            string studentRoleName = "student";
            string teacherRoleName = "teacher";

            string adminEmail = "denys.kravtsov@nure.ua";
            string adminPassword = "12345678";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role studentRole = new Role { Id = 2, Name = studentRoleName };
            Role teacherRole = new Role { Id = 3, Name = teacherRoleName };

            User adminUser = new User { UserId = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, studentRole, teacherRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
