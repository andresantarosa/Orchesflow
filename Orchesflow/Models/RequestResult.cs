using System.Collections.Generic;

namespace Orchesflow.Models
{
    public class RequestResult<TResponse>
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public TResponse Data { get; set; }
    }
}
