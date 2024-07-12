using Microsoft.EntityFrameworkCore;
using SMS.Model.Log;
using SMS.Model.Student;
using SMS.Model.Subject;
using SMS.Model.Teacher;
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

    }
}
