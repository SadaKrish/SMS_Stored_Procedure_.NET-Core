using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.ErrorResponse
{
    public class ErrorResponse
    {
       
        public bool Success { get; set; } =false;
        public List<string> Messages { get; set; }=new List<string>();

        public string ErrorMessages { 
            get
            {
                var error = "";
                if (Messages.Count > 0)
                { 
                    foreach (var message in Messages) 
                    {
                        error = message.ToString()+"\n";
                    }
                    return error;
                }
                return error;
            }
        }
    }
}
