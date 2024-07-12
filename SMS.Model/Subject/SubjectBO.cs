using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Model.Subject
{
    public class SubjectBO
    {
        [Key]
        public long SubjectID { get; set; }
        [Required(ErrorMessage = "Subject code is required"), Display(Name = "Subject Code")]
        public string SubjectCode { get; set; }
        [Required(ErrorMessage = "Subject Name is required"), DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Status")]
        public bool IsEnable { get; set; }
    }
}
