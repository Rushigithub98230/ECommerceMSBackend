using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EComMSSharedLibrary.ComonResponseModel
{
    public class ApiResponse<T> where T : class
    {
        public bool Success { get;  set; }
        public string? AccessToken { get;  set; }
        public string? Message { get;  set; }
        public T? Data { get;  set; }
        public int StatusCode { get;  set; }
        public IEnumerable<string> Errors { get;  set; } = Enumerable.Empty<string>();

         

        public static ApiResponse<T> Create(T? data = null, string? message = "", int statusCode = 200, IEnumerable<string>? errors = null, string? accessToken = null)
        {
            return new ApiResponse<T>
            {
                Success = statusCode >= 200 && statusCode < 300, 
                Data = data,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? Enumerable.Empty<string>(),
                AccessToken = accessToken
            };
        }
    }


}
