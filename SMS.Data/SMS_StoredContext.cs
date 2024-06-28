using Microsoft.EntityFrameworkCore;
using SMS.Model.Student;
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
    }
}
