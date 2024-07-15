using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Model.Teacher_Subject_Allocation
{
    public class Teacher_Subject_AllocationBO
    {
        [Key]
        public long? SubjectAllocationID { get; set; }
        [Required(ErrorMessage = "Teacher is required")]
        public long TeacherID { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public long SubjectID { get; set; }
       
    }
}
