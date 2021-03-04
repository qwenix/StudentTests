using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StudentTestsWeb.Models.Entities;

namespace StudentTestsWeb.Models {
    public partial class StudentTestsContext : DbContext {

        public StudentTestsContext(DbContextOptions<StudentTestsContext> options)
            : base(options) {
            Database.EnsureCreated();
        }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Discipline> Disciplines { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Test> Tests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Answer>(entity => {
                entity.HasKey(e => e.AnswerId);

                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_Answers_ToQuestions");
            });

            modelBuilder.Entity<Discipline>(entity => {
                entity.HasKey(e => e.Abbr);

                entity.Property(e => e.Abbr)
                    .HasMaxLength(10)
                    .ValueGeneratedNever();

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.Disciplines)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("FK_Disciplines_ToTeachers");
            });

            modelBuilder.Entity<Question>(entity => {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TestId)
                    .HasConstraintName("FK_Questions_ToTests");
            });

            modelBuilder.Entity<Result>(entity => {
                entity.HasKey(e => new { e.StudentId, e.TestId });

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("FK_Results_ToStudents");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.TestId)
                    .HasConstraintName("FK_Results_ToTests");
            });

            modelBuilder.Entity<Student>(entity => {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.FatherName).IsRequired();

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.Group).IsRequired();

                entity.Property(e => e.LastName).IsRequired();

                entity.Property(e => e.Mail).IsRequired();

                entity.Property(e => e.Password).IsRequired();
            });

            modelBuilder.Entity<Teacher>(entity => {
                entity.HasKey(e => e.TeacherId);

                entity.Property(e => e.Department).IsRequired();

                entity.Property(e => e.FatherName).IsRequired();

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.LastName).IsRequired();

                entity.Property(e => e.Mail).IsRequired();

                entity.Property(e => e.Position).IsRequired();
            });

            modelBuilder.Entity<Test>(entity => {
                entity.HasKey(e => e.TestId);

                entity.Property(e => e.Abbr)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Topic).IsRequired();

                entity.HasOne(d => d.AbbrNavigation)
                    .WithMany(p => p.Tests)
                    .HasForeignKey(d => d.Abbr)
                    .HasConstraintName("FK_Tests_ToDisciplines");
            });
        }
    }
}
