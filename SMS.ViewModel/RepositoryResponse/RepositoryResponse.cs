using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SMS.ViewModel.RepositoryResponse
{
    public class RepositoryResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        [JsonIgnore]
        public List<string> Messages { get; set; }= new List<string>();

    }
}
