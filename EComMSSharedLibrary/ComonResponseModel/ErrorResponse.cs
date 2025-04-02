using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EComMSSharedLibrary.ComonResponseModel
{
   public class ErrorResponse
    {
        public HttpStatusCode Status { get; set; }
        public string? Message { get; set; }
        public string? TraceId { get; set; }
    }
}
