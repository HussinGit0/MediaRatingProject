using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingProject.API.Requests
{
    public class ParsedRequestDTO
    {       
        public bool IsSuccessful { get; set; }
        public string HttpMethod { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
        public string Token { get; set; }  

        public ParsedRequestDTO() { }
    }
}
