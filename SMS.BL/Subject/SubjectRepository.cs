using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.BL.Subject.Interface;
using SMS.Data;
using SMS.Model.Subject;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject
{
    public class SubjectRepository:ISubjectRepository
    {
        private readonly SMS_StoredContext _context;

        public SubjectRepository(SMS_StoredContext context)
        {
            _context = context;
        }
        public RepositoryResponse<IEnumerable<SubjectBO>> GetSubjects(bool? isEnable)
        {
            var response = new RepositoryResponse<IEnumerable<SubjectBO>>();
            try
            {
                var statusParam = new SqlParameter("@IsEnable", isEnable.HasValue ? isEnable.Value : DBNull.Value);

                // Execute the stored procedure
                var subjects = _context.Subjects
                                       .FromSqlRaw("EXEC usp_Get_Subjects @IsEnable", statusParam).ToList();

                response.Data = subjects;
                response.Success = true;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subjects"));

            }
            return response;
        }
    }
}
