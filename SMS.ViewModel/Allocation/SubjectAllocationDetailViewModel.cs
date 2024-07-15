using SMS.ViewModel.AllocationViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class SubjectAllocationDetailViewModel
    {
        [DisplayName("Subject Code")]
        public string SubjectCode { get; set; }

        [DisplayName("Subject Name")]
        public string SubjectName { get; set; }

        public long SubjectID { get; set; }

        [DisplayName("Teachers")]
        public List<TeacherAllocationViewModel> Teachers { get; set; }

        public SearchViewModel SearchView { get; set; }
    }
}
