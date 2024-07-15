using SMS.ViewModel.AllocationViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class StudentAllocationGroupedViewModel
    {
        public string StudentRegNo { get; set; }
        public string DisplayName { get; set; }

        public bool IsEnable { get; set; }
        public long StudentAllocationID { get; set; }
        public List<SubjectAllocationViewModel> Subjects { get; set; } = new List<SubjectAllocationViewModel>();
    }
}
