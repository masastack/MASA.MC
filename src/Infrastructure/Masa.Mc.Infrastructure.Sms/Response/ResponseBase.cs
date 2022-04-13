using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Mc.Infrastructure.Sms.Response
{
    public class ResponseBase
    {
        public bool Success { get; }

        public string Message { get; } 

        public string Json { get; set; }

        protected ResponseBase(bool success, string message,string json)
        {
            Success = success;
            Message = message;
            Json= json;
        }
    }
}
