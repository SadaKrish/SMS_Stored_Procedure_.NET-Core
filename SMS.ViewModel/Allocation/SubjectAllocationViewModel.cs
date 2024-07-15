using SMS.ViewModel.Allocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.ViewModel.AllocationViewModel
{
    public class SubjectAllocationViewModel
    {
        
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
       // public bool IsAllocated { get; set; }
        public long SubjectID { get; set; }
        public List<TeacherAllocationViewModel> TeacherAllocations { get; set; } = new List<TeacherAllocationViewModel>();
    }
}
