using Microsoft.EntityFrameworkCore;
using SMS.Model.Log;
using SMS.Model.Student;
using SMS.Model.Student_Subject_Teacher_Allocation;
using SMS.Model.Subject;
using SMS.Model.Teacher;
using SMS.Model.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data
{
    public class SMS_StoredContext:DbContext
    {
        public SMS_StoredContext(DbContextOptions<SMS_StoredContext> options)
          : base(options)
        {
        }

        public DbSet<StudentBO> Students { get; set; }
        public DbSet<LogBO> Logs { get; set; }

        public DbSet<TeacherBO> Teachers { get; set; }
        public DbSet<SubjectBO> Subjects { get; set; }

        public DbSet<Teacher_Subject_AllocationBO> Teacher_Subject_Allocations { get; set; }
        public DbSet<Student_Subject_Teacher_AllocationBO>student_Subject_Teacher_Allocations { get; set; }

       // public DbSet<EnabledTeacherViewModel> EnabledTeachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          //  modelBuilder.Entity<TeacherBO>().ToTable("Teacher");
            modelBuilder.Entity<EnabledTeacherViewModel>().HasNoKey(); // ViewModel does not need a key
        }

    }
}
