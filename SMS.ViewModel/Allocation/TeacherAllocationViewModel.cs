using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class TeacherAllocationViewModel
    {
        public long SubjectAllocationID { get; set; }
        public string TeacherRegNo { get; set; }
        public string DisplayName { get; set; }
        public bool IsAllocated { get; set; }
        public long StudentAllocationID { get; set; }
    }
}
