using SMS.Model.Subject;
using SMS.ViewModel.RepositoryResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject.Interface
{
    public interface ISubjectRepository
    {
        RepositoryResponse<IEnumerable<SubjectBO>> GetSubjects(bool? isEnable);
    }
}
