using SMS.Model.Student;
using SMS.Model.Student_Subject_Teacher_Allocation;
using SMS.Model.Subject;
using SMS.Model.Teacher;
using SMS.Model.Teacher_Subject_Allocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class AllocationViewModel
    {
        public IEnumerable<Teacher_Subject_AllocationBO> Teacher_Subject_AllocationList { get; set; }
        public IEnumerable<Student_Subject_Teacher_AllocationBO> Student_Subject_Teacher_AllocationList { get; set; }

        public IEnumerable<StudentBO> StudentList { get; set; }
        public IEnumerable<TeacherBO> TeacherList { get; set; }
        public IEnumerable<SubjectBO> SubjectList { get; set; }
    }
}
