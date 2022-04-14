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

        protected ResponseBase(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
