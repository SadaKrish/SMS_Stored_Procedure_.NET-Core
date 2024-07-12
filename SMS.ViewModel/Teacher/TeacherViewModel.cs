using SMS.Model.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Teacher
{
    public class TeacherViewModel
    {
        public IEnumerable<TeacherBO> TeacherList { get; set; }
        public SearchViewModel SearchView { get; set; }
    }
}
